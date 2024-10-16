// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dolittle.SDK.Analyzers;

/// <summary>
/// Analyzer for <see cref="DescriptorRules.Events.Redaction.RedactablePersonalDataAttribute"/>.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class RedactablePropertyAnalyzer : DiagnosticAnalyzer
{
    static readonly DiagnosticDescriptor _rule = DescriptorRules.NonNullableRedactableField;

    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [_rule];

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze |
                                               GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeProperty, SyntaxKind.PropertyDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzeRecordDeclaration, SyntaxKind.RecordDeclaration);
    }

    private static void AnalyzeProperty(SyntaxNodeAnalysisContext context)
    {
        var propertyDeclaration = (PropertyDeclarationSyntax)context.Node;
        var propertySymbol = context.SemanticModel.GetDeclaredSymbol(propertyDeclaration);

        if (propertySymbol == null) return;

        var hasAttribute = propertySymbol.GetAttributes()
            .Any(attr => attr.AttributeClass?.Name == "RedactablePersonalDataAttribute");

        if (!hasAttribute) return;

        if (propertySymbol.Type.NullableAnnotation != NullableAnnotation.Annotated)
        {
            context.ReportDiagnostic(Diagnostic.Create(_rule, propertyDeclaration.GetLocation(), propertySymbol.Name));
        }
    }
    
    private static void AnalyzeRecordDeclaration(SyntaxNodeAnalysisContext context)
    {
        var recordDeclaration = (RecordDeclarationSyntax)context.Node;
            
        // Check if it's a record with a primary constructor
        if (recordDeclaration.ParameterList == null) return;

        foreach (var parameter in recordDeclaration.ParameterList.Parameters)
        {
            var isPersonalDataAnnotated = parameter.AttributeLists
                .SelectMany(list => list.Attributes)
                .Any(attr => attr.Name.ToString().StartsWith("RedactablePersonalData"));
            if (!isPersonalDataAnnotated)
            {
                continue;
            }
            
            var parameterSymbol = context.SemanticModel.GetDeclaredSymbol(parameter);
            if (!IsNullable(parameterSymbol))
            {
                context.ReportDiagnostic(Diagnostic.Create(_rule, parameter.GetLocation(), parameterSymbol.Name));
            }

        }
    }

    static bool IsNullable(IParameterSymbol? parameterSymbol)
    {
        return parameterSymbol?.Type.NullableAnnotation == NullableAnnotation.Annotated;
    }
}
