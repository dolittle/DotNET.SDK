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
/// Annotation analyzer for Dolittle SDK.
/// Ensures that all identities are valid Guids
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class AnnotationIdentityAnalyzer : DiagnosticAnalyzer
{
    const string Title = "Invalid identity in attribute";
    const string MessageFormat = "Attribute '{0}' {1}: '{2}' is not a valid Guid";
    const string Description = "Add a Guid identity";


    internal static readonly DiagnosticDescriptor InvalidIdentityRule =
        new(
            DiagnosticIds.AnnotationInvalidIdentityRuleId,
            Title,
            MessageFormat,
            DiagnosticCategories.Sdk,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(InvalidIdentityRule);

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
            case "Dolittle.SDK.Events.EventTypeAttribute.EventTypeAttribute(string, uint, string?)":
            case "Dolittle.SDK.Events.Handling.EventHandlerAttribute.EventHandlerAttribute(string, bool, string?, string?)":
            case "Dolittle.SDK.Aggregates.AggregateRootAttribute.AggregateRootAttribute(string, string?)":
            case "Dolittle.SDK.Projections.ProjectionAttribute.ProjectionAttribute(string, string?, string?)":
            case "Dolittle.SDK.Embeddings.EmbeddingAttribute.EmbeddingAttribute(string)":
                CheckAttributeIdentity(attribute, symbol, context);
                return;

            default:
                return;
        }
    }

    void CheckAttributeIdentity(AttributeSyntax attribute, IMethodSymbol symbol, SyntaxNodeAnalysisContext context)
    {
        var identityParameter = symbol.Parameters[0];
        if (!attribute.TryGetArgumentValue(identityParameter, out var id)) return;
        var identityText = id.GetText().ToString();

        if (!Guid.TryParse(identityText.Trim('"'), out _))
        {
            var properties = ImmutableDictionary<string, string?>.Empty.Add("identityParameter", identityParameter.Name);
            context.ReportDiagnostic(Diagnostic.Create(InvalidIdentityRule, attribute.GetLocation(), properties,
                attribute.Name.ToString(), identityParameter.Name, identityText));
        }
    }
}
