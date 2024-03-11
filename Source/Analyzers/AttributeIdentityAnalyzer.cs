// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
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
    readonly ConcurrentDictionary<(string type, Guid id), AttributeSyntax> _identities = new();

    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(DescriptorRules.InvalidIdentity, DescriptorRules.DuplicateIdentity, DescriptorRules.MissingBaseClass);

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        _identities.Clear();
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(CheckAttribute, ImmutableArray.Create(SyntaxKind.Attribute));
    }

    void CheckAttribute(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not AttributeSyntax attribute) return;
        if (context.SemanticModel.GetSymbolInfo(attribute).Symbol is not IMethodSymbol symbol) return;
        if (!symbol.IsDolittleType()) return;


        var className = symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        switch (className)
        {
            case "EventTypeAttribute":
            case "EventHandlerAttribute":
                CheckAttributeIdentity(attribute, symbol, context);
                break;
            case "AggregateRootAttribute":
                CheckAttributeIdentity(attribute, symbol, context);
                CheckHasBaseClass(context, DolittleTypes.AggregateRootBaseClass);
                break;

            case "ProjectionAttribute":
                CheckAttributeIdentity(attribute, symbol, context);
                CheckHasBaseClass(context, DolittleTypes.ReadModelClass);
                break;
        }
    }

    void CheckHasBaseClass(SyntaxNodeAnalysisContext context, string expectedBaseClass)
    {
        if (context.Node.FirstAncestorOrSelf<ClassDeclarationSyntax>() is not { } classDeclaration) return;

        if (classDeclaration.BaseList is null || classDeclaration.BaseList.Types.Count == 0 || !TypeExtends(classDeclaration, expectedBaseClass, context))
        {
            var className = classDeclaration.Identifier.ToString();
            context.ReportDiagnostic(Diagnostic.Create(DescriptorRules.MissingBaseClass, classDeclaration.GetLocation(), className, expectedBaseClass));
        }
    }

    /// <summary>
    /// Checks if the type is in the hierarchy of the expected base class
    /// </summary>
    /// <param name="type"></param>
    /// <param name="expectedBaseClass"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    static bool TypeExtends(ClassDeclarationSyntax type, string expectedBaseClass, SyntaxNodeAnalysisContext context)
    {
        var typeSymbol = context.SemanticModel.GetDeclaredSymbol(type);
        var baseClassType = context.SemanticModel.Compilation.GetTypeByMetadataName(expectedBaseClass);
        
        return TypeExtends(typeSymbol, baseClassType);
    }

    static bool TypeExtends(INamedTypeSymbol? typeSymbol, INamedTypeSymbol? baseClassType)
    {
        while (typeSymbol != null)
        {
            if (typeSymbol.Equals(baseClassType, SymbolEqualityComparer.Default))
            {
                return true;
            }

            typeSymbol = typeSymbol.BaseType;
        }

        return false;
    }

    void CheckAttributeIdentity(AttributeSyntax attribute, IMethodSymbol symbol, SyntaxNodeAnalysisContext context)
    {
        var identityParameter = symbol.Parameters[0];
        if (!attribute.TryGetArgumentValue(identityParameter, out var id)) return;
        var identityText = id.GetText().ToString();
        var attributeName = attribute.Name.ToString();

        if (!Guid.TryParse(identityText.Trim('"'), out var identifier))
        {
            var properties = ImmutableDictionary<string, string?>.Empty.Add("identityParameter", identityParameter.Name);
            context.ReportDiagnostic(Diagnostic.Create(DescriptorRules.InvalidIdentity, attribute.GetLocation(), properties,
                attributeName, identityParameter.Name, identityText));
        }
        else
        {
            var key = (attributeName, identifier);
            if (!_identities.TryAdd(key, attribute))
            {
                // Only reports secondary sightings, not the first one
                ReportDuplicateIdentity(attribute, context, identifier);
            }
        }
    }

    static void ReportDuplicateIdentity(AttributeSyntax attribute, SyntaxNodeAnalysisContext context, Guid identifier) =>
        context.ReportDiagnostic(
            Diagnostic.Create(DescriptorRules.DuplicateIdentity, attribute.GetLocation(), attribute.Name.ToString(), identifier.ToString()));
}
