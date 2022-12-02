// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Dolittle.SDK.Analyzers.CodeFixes;

static class Extensions
{
    static readonly LineEndingsRewriter _lineEndingsRewriter = new();

    public static T WithLfLineEndings<T>(this T replacedNode) where T : SyntaxNode => (T)_lineEndingsRewriter.Visit(replacedNode);

    public static CompilationUnitSyntax AddMissingUsingDirectives(this CompilationUnitSyntax root, params INamespaceSymbol[] namespaces)
    {
        return root.AddMissingUsingDirectives(namespaces.Select(_ => _.ToDisplayString()).ToArray());
    }
    public static CompilationUnitSyntax AddMissingUsingDirectives(this CompilationUnitSyntax root, params string[] namespaces)
    {
        var usingDirectives = root.DescendantNodes().OfType<UsingDirectiveSyntax>().ToList();
        var nonImportedNamespaces = namespaces.ToImmutableHashSet();

        foreach (var usingDirective in usingDirectives)
        {
            var usingDirectiveName = usingDirective.Name.ToString();
            if (nonImportedNamespaces.Contains(usingDirectiveName))
            {
                nonImportedNamespaces = nonImportedNamespaces.Remove(usingDirectiveName);
            }
        }

        if (!nonImportedNamespaces.Any()) return root;

        var newUsingDirectives = nonImportedNamespaces.Select(namespaceName => SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(namespaceName))).ToArray();
        
        return root.AddUsings(newUsingDirectives).NormalizeWhitespace();
    }
}
