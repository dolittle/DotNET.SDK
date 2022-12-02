﻿// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dolittle.SDK.Analyzers;

/// <summary>
/// Attribute analyzer for Dolittle SDK.
/// Ensures that all identities are valid Guids
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class AttributeIdentityAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(DescriptorRules.InvalidIdentity);

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


        switch (symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))
        {
            case "EventTypeAttribute":
            case "EventHandlerAttribute":
            case "AggregateRootAttribute":
            case "ProjectionAttribute":
            case "EmbeddingAttribute":
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
            context.ReportDiagnostic(Diagnostic.Create(DescriptorRules.InvalidIdentity, attribute.GetLocation(), properties,
                attribute.Name.ToString(), identityParameter.Name, identityText));
        }
    }
}