// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Dolittle.SDK.Analyzers;

static class AnalysisExtensions
{


    public static bool IsDolittleType(this ISymbol symbol)
    {
        var symbolNamespace = symbol.ContainingNamespace.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        return symbolNamespace.StartsWith("global::Dolittle.SDK", StringComparison.Ordinal)
               || symbolNamespace.StartsWith("Dolittle.SDK", StringComparison.Ordinal);
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
