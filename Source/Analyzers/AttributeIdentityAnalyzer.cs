// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Linq;
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
    const string BaseclassKey = "baseClass";

    static readonly ImmutableDictionary<string, string?> _missingProjectionBaseClassProperties =
        ImmutableDictionary<string, string?>.Empty
            .Add(BaseclassKey, DolittleTypes.ReadModelClass);

    static readonly ImmutableDictionary<string, string?> _missingAggregateBaseClassProperties =
        ImmutableDictionary<string, string?>.Empty
            .Add(BaseclassKey, DolittleTypes.AggregateRootBaseClass);

    readonly ConcurrentDictionary<(string type, Guid id), AttributeSyntax> _identities = new();

    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(DescriptorRules.InvalidIdentity, DescriptorRules.DuplicateIdentity, DescriptorRules.MissingBaseClass, DescriptorRules.InvalidTimespan);

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
                CheckHasBaseClass(context, DolittleTypes.AggregateRootBaseClass, _missingAggregateBaseClassProperties);
                break;

            case "ProjectionAttribute":
                CheckAttributeIdentity(attribute, symbol, context);
                CheckAttributeParseAbleIfPresent(attribute, symbol, context, "idleUnloadTimeout", IsValidTimespan, DescriptorRules.InvalidTimespan);
                CheckHasBaseClass(context, DolittleTypes.ReadModelClass, _missingProjectionBaseClassProperties);
                break;
        }
    }

    void CheckAttributeParseAbleIfPresent(AttributeSyntax attribute, IMethodSymbol symbol, SyntaxNodeAnalysisContext context, string parameterName,
        Func<string, bool> isParseAble, DiagnosticDescriptor descriptor)
    {
        var parameter = symbol.Parameters.FirstOrDefault(_ => _.Name == parameterName);
        if (parameter is null || !attribute.TryGetArgumentValue(parameter, out var value)) return;
        if (!isParseAble(value.GetText().ToString().Trim('\"')))
        {
            var properties = ImmutableDictionary<string, string?>.Empty.Add("parameterName", parameterName);
            context.ReportDiagnostic(Diagnostic.Create(descriptor, attribute.GetLocation(), properties, attribute.Name.ToString(), parameterName));
        }
    }

    void CheckHasBaseClass(SyntaxNodeAnalysisContext context, string expectedBaseClass, ImmutableDictionary<string, string?> properties)
    {
        if (context.Node.FirstAncestorOrSelf<ClassDeclarationSyntax>() is not { } classDeclaration) return;

        if (classDeclaration.BaseList is null || classDeclaration.BaseList.Types.Count == 0 || !TypeExtends(classDeclaration, expectedBaseClass, context))
        {
            var className = classDeclaration.Identifier.ToString();
            context.ReportDiagnostic(Diagnostic.Create(DescriptorRules.MissingBaseClass, classDeclaration.GetLocation(), properties, className,
                expectedBaseClass));
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
        if (!TryGetStringValue(attribute, identityParameter, context, out var identityText)) return;
        var attributeName = attribute.Name.ToString();

        if (!Guid.TryParse(identityText!.Trim('"'), out var identifier))
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

    static bool TryGetStringValue(AttributeSyntax attribute, IParameterSymbol parameter, SyntaxNodeAnalysisContext context, out string? argumentString)
    {
        if (!attribute.TryGetArgumentValue(parameter, out var expression))
        {
            argumentString = null;
            return true;
        }
        
        // Check if the argument is a string literal or a constant
        if (expression is LiteralExpressionSyntax { Token.Value: string value })
        {
            argumentString = value;
            return true;
        }

        if (expression is MemberAccessExpressionSyntax memberAccess)
        {
            // If the argument is a member access, check if it's a constant
            // Then retrieve the constant value
            var symbol = context.SemanticModel.GetSymbolInfo(memberAccess).Symbol;
            if (symbol is IFieldSymbol { HasConstantValue: true } field)
            {
                argumentString = field.ConstantValue?.ToString();
                return true;
            }
        }

        argumentString = null;
        return false;
    }

    static void ReportDuplicateIdentity(AttributeSyntax attribute, SyntaxNodeAnalysisContext context, Guid identifier) =>
        context.ReportDiagnostic(
            Diagnostic.Create(DescriptorRules.DuplicateIdentity, attribute.GetLocation(), attribute.Name.ToString(), identifier.ToString()));

    static bool IsValidTimespan(string value) => TimeSpan.TryParse(value, out _);
}
