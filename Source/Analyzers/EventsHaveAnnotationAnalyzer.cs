// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dolittle.SDK.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class EventsHaveAnnotationAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(DescriptorRules.Events.MissingAttribute);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        // Analyze all calls to ICommitEvents.Commit, CommitEvent and CommitPublicEvent
        // and check that the parameter object  has the EventType attribute
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.InvocationExpression);
    }

    // Analyze all calls to ICommitEvents.Commit, CommitEvent and CommitPublicEvent
    // and check that the parameter object  has the EventType attribute
    static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;
        if (invocation.Expression is not MemberAccessExpressionSyntax { Name: IdentifierNameSyntax identifierName }) return;
        var methodName = identifierName.ToString();
        if (methodName != "CommitEvent" && methodName != "CommitPublicEvent") return;

        if (context.SemanticModel.GetSymbolInfo(invocation).Symbol is not IMethodSymbol methodSymbol) return;
        if (!methodSymbol.ContainingTypeHasInterface(DolittleTypes.ICommitEventsInterface)) return;

        var argument = invocation.ArgumentList.Arguments[0];

        var typeInfo = context.SemanticModel.GetTypeInfo(argument.Expression);
        if (typeInfo.Type is null) return;
        if (typeInfo.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).Equals("object")) return;

        // Check if the argument to the invocation argument has the Dolittle.SDK.Events.EventTypeAttribute attribute
        if (typeInfo.Type.HasEventTypeAttribute()) return; // All good

        context.ReportDiagnostic(Diagnostic.Create(DescriptorRules.Events.MissingAttribute, invocation.GetLocation(),
            typeInfo.Type.ToTargetClassAndAttributeProps(DolittleTypes.EventTypeAttribute), typeInfo.Type.Name));
    }
}
