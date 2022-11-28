// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dolittle.SDK.Analyzers;

/// <summary>
/// Analyzer for reporting code block diagnostics.
/// It reports diagnostics for all redundant methods which have an empty method body and are not virtual/override.
/// </summary>
/// <remarks>
/// For analyzers that requires analyzing symbols or syntax nodes across a code block, see <see cref="CodeBlockStartedAnalyzer"/>.
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class AnnotationIdentityAnalyzer : DiagnosticAnalyzer
{
    const string Title = "Missing identity in attribute";
    const string InvalidTitle = "Invalid identity in attribute";
    const string MessageFormat = "Attribute '{0}' is missing an identity";
    const string InvalidMessageFormat = "Attribute '{0}' {1}: '{2}' is not a valid Guid";

    const string Description = "Add a Guid identity";

    internal static readonly DiagnosticDescriptor MissingIdentityRule =
        new(
            DiagnosticIds.AnnotationMissingIdentityRuleId,
            Title,
            MessageFormat,
            DiagnosticCategories.Sdk,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: Description);

    internal static readonly DiagnosticDescriptor InvalidIdentityRule =
        new(
            DiagnosticIds.AnnotationInvalidIdentityRuleId,
            InvalidTitle,
            InvalidMessageFormat,
            DiagnosticCategories.Sdk,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(MissingIdentityRule, InvalidIdentityRule);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(CheckHasIdentity, ImmutableArray.Create(SyntaxKind.Attribute));
    }

    void CheckHasIdentity(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not AttributeSyntax attribute) return;
        if (context.SemanticModel.GetSymbolInfo(attribute).Symbol is not IMethodSymbol symbol) return;
        if (!symbol.IsDolittleType()) return;

        switch (symbol.ToDisplayString())
        {
            case "Dolittle.SDK.Events.EventTypeAttribute.EventTypeAttribute(string, uint, string)":
            case "Dolittle.SDK.Events.Handling.EventHandlerAttribute.EventHandlerAttribute(string, bool, string, string)":
            case "Dolittle.SDK.Aggregates.AggregateRootAttribute.AggregateRootAttribute(string, string)":
                CheckAttributeIdentity(attribute, symbol, context);
                return;

            default:
                return;
        }
    }

    void CheckAttributeIdentity(AttributeSyntax attribute, IMethodSymbol symbol, SyntaxNodeAnalysisContext context)
    {
        var identityParameter = symbol.Parameters[0];
        if (attribute.TryGetArgumentValue(identityParameter, out var id))
        {
            var text = id.GetText();
            if (!Guid.TryParse(text.ToString().Trim('"'), out _))
            {
                var properties = ImmutableDictionary<string, string?>.Empty.Add("identityParameter", identityParameter.Name);
                context.ReportDiagnostic(Diagnostic.Create(InvalidIdentityRule, attribute.GetLocation(), properties,
                    attribute.Name.ToString(), identityParameter.Name, text.ToString()));
            }
        }
        else
        {
            context.ReportDiagnostic(Diagnostic.Create(MissingIdentityRule, attribute.GetLocation(), attribute.Name.ToString()));
        }
    }

    // void SemanticAnalysis(SemanticModelAnalysisContext context)
    // {
    //     context.SemanticModel.
    //     
    //     Console.WriteLine(runId+": " + context);
    // }

    // static void CodeBlockAction(CodeBlockAnalysisContext codeBlockContext)
    // {
    //     // We only care about method bodies.
    //     if (codeBlockContext.OwningSymbol is)
    //     {
    //         return;
    //     }
    //
    //     // Report diagnostic for void non-virtual methods with empty method bodies.
    //     var method = (IMethodSymbol)codeBlockContext.OwningSymbol;
    //     var block = (BlockSyntax)codeBlockContext.CodeBlock.ChildNodes().FirstOrDefault(n => n.Kind() == SyntaxKind.Block);
    //     if (method.ReturnsVoid && !method.IsVirtual && block != null && block.Statements.Count == 0)
    //     {
    //         var tree = block.SyntaxTree;
    //         var location = method.Locations.First(l => tree.Equals(l.SourceTree));
    //         var diagnostic = Diagnostic.Create(Rule, location, method.Name);
    //         codeBlockContext.ReportDiagnostic(diagnostic);
    //     }
    // }
}
