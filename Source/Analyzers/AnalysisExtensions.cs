// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dolittle.SDK.Analyzers;

static class AnalysisExtensions
{
    public static bool IsDolittleType(this ISymbol symbol)
    {
        var symbolNamespace = symbol.ContainingNamespace.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        return symbolNamespace.StartsWith("global::Dolittle.SDK", StringComparison.Ordinal)
               || symbolNamespace.StartsWith("Dolittle.SDK", StringComparison.Ordinal);
    }

    public static bool FieldsArePrivateByDefault(this SymbolAnalysisContext context) =>
        context.Options.AnalyzerConfigOptionsProvider.GlobalOptions.TryGetValue("dotnet_naming_symbols.private_fields.required_modifiers",
            out var requiredPrivate) && string.IsNullOrEmpty(requiredPrivate);

    /// <summary>
    /// Checks if base class of the type is Dolittle.SDK.Aggregates.AggregateRoot
    /// </summary>
    /// <param name="typeSymbol">The checked class</param>
    /// <returns></returns>
    public static bool IsAggregateRoot(this INamedTypeSymbol typeSymbol)
    {
        var baseType = typeSymbol.BaseType;
        while (baseType != null)
        {
            if (baseType.ToString() == DolittleTypes.AggregateRootBaseClass)
            {
                return true;
            }

            baseType = baseType.BaseType;
        }

        return false;
    }
    
    /// <summary>
    /// Checks if base class of the type is Dolittle.SDK.Projections.ProjectionBase
    /// </summary>
    /// <param name="typeSymbol">The checked class</param>
    /// <returns></returns>
    public static bool IsProjection(this INamedTypeSymbol typeSymbol)
    {
        var baseType = typeSymbol.BaseType;
        while (baseType != null)
        {
            if (baseType.ToString() == DolittleTypes.ProjectionBaseClass)
            {
                return true;
            }

            baseType = baseType.BaseType;
        }

        return false;
    }

    public static bool HasEventTypeAttribute(this ITypeSymbol type) => type.HasAttribute(DolittleTypes.EventTypeAttribute);
    public static bool HasAggregateRootAttribute(this ITypeSymbol type) => type.HasAttribute(DolittleTypes.AggregateRootAttribute);
    public static bool HasEventHandlerAttribute(this ITypeSymbol type) => type.HasAttribute(DolittleTypes.EventHandlerAttribute);

    public static bool HasAttribute(this ITypeSymbol type, string attributeName)
    {
        if (type is null) throw new ArgumentNullException(nameof(type));
        if (attributeName is null) throw new ArgumentNullException(nameof(attributeName));

        return type.GetAttributes().Any(_ =>
        {
            var attributeClassName = _.AttributeClass?.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);
            return attributeClassName == attributeName;
        });
    }

    public static bool ContainingTypeHasInterface(this IMethodSymbol methodSymbol, string qualifiedInterface) =>
        methodSymbol.ContainingType.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat).Equals(qualifiedInterface) ||
        methodSymbol.ContainingType.AllInterfaces.Any(_ => _.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat) == qualifiedInterface);

    public static ImmutableDictionary<string, string?> ToMinimalTypeNameProps(this ITypeSymbol type) =>
        new Dictionary<string, string?>
        {
            { "typeName", type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat) },
        }.ToImmutableDictionary();

    public static ImmutableDictionary<string, string?> ToTargetClassAndAttributeProps(this ITypeSymbol type, string attributeClass) =>
        new Dictionary<string, string?>
        {
            { "targetClass", type.ToString() },
            { "attributeClass", attributeClass },
        }.ToImmutableDictionary();

    public static ImmutableDictionary<string, string?> ToTargetClassProps(this ITypeSymbol type) =>
        new Dictionary<string, string?>
        {
            { "targetClass", type.ToString() },
        }.ToImmutableDictionary();

    public static bool TryGetArgumentValue(this AttributeSyntax attribute, IParameterSymbol parameterSymbol, out ExpressionSyntax expressionSyntax) =>
        attribute.TryGetArgumentValue(parameterSymbol.Name, parameterSymbol.Ordinal, out expressionSyntax);

    public static bool TryGetArgumentValue(this AttributeSyntax attribute, string parameterName, int parameterOrdinal, out ExpressionSyntax expressionSyntax)
    {
        if (attribute.ArgumentList is null)
        {
            expressionSyntax = default!;
            return false;
        }

        var ordinal = parameterOrdinal;
        if (attribute.ArgumentList.Arguments.Count > ordinal)
        {
            var positionalArgument = attribute.ArgumentList.Arguments[ordinal];
            if (positionalArgument.NameColon is null || positionalArgument.MatchesName(parameterName))
            {
                // parameter is positional or name matches
                expressionSyntax = positionalArgument.Expression;
                return true;
            }
        }

        foreach (var argument in attribute.ArgumentList.Arguments)
        {
            if (argument.MatchesName(parameterName))
            {
                expressionSyntax = argument.Expression;
                return true;
            }
        }

        expressionSyntax = default!;
        return false;
    }

    static bool MatchesName(this AttributeArgumentSyntax attributeArgument, string parameterName)
    {
        return attributeArgument.NameColon?.Name.Identifier.Text == parameterName;
    }
    
}
