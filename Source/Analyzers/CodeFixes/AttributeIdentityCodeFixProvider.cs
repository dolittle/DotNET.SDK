// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Dolittle.SDK.Analyzers.CodeFixes;

/// <summary>
/// Generates a valid Guid identity for a given Dolittle identity attribute
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AttributeIdentityCodeFixProvider)), Shared]
public class AttributeIdentityCodeFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(DiagnosticIds.AttributeInvalidIdentityRuleId);

    public override Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var document = context.Document;
        var diagnostic = context.Diagnostics[0];
        if (!diagnostic.Properties.TryGetValue("identityParameter", out var identityParameterName))
        {
            return Task.CompletedTask;
        }

        switch (diagnostic.Id)
        {
            case DiagnosticIds.AttributeInvalidIdentityRuleId:
                context.RegisterCodeFix(
                    CodeAction.Create(
                        "Generate identity", ct => GenerateIdentity(context, document, identityParameterName!, ct),
                        nameof(AttributeIdentityCodeFixProvider) + ".AddIdentity"),
                    diagnostic);
                break;
        }

        return Task.CompletedTask;
    }


    static async Task<Document> GenerateIdentity(CodeFixContext context, Document document, string identityParameterName,
        CancellationToken cancellationToken)
    {
        var root = await context.Document.GetSyntaxRootAsync(cancellationToken);
        if (root is null) return document;
        if (!TryGetTargetNode(context, root, out AttributeSyntax attribute)) return document; // Target not found
        var updatedRoot = root.ReplaceNode(attribute, GenerateIdentity(attribute, identityParameterName));
        return document.WithSyntaxRoot(updatedRoot);
    }

    static AttributeSyntax GenerateIdentity(AttributeSyntax existing, string identityParameterName)
    {
        var newIdentity = SyntaxFactory.ParseExpression("\"" + IdentityGenerator.Generate() + "\"");
        if (existing.ArgumentList is not null && existing.TryGetArgumentValue(identityParameterName, 0, out var oldIdentity))
        {
            return existing.ReplaceNode(oldIdentity, newIdentity);
        }

        return existing.WithArgumentList(
            SyntaxFactory.AttributeArgumentList(
                SyntaxFactory.SingletonSeparatedList(
                    SyntaxFactory.AttributeArgument(newIdentity))));
    }

    static bool TryGetTargetNode<TNode>(
        CodeFixContext context,
        SyntaxNode root,
        out TNode node) where TNode : SyntaxNode
    {
#pragma warning disable CS8601
        node = root
            .FindNode(context.Span, false, getInnermostNodeForTie: true)?
            .FirstAncestorOrSelf<TNode>(ascendOutOfTrivia: true);
#pragma warning restore CS8601
        return node is not null;
    }


    public override FixAllProvider GetFixAllProvider()
    {
        return WellKnownFixAllProviders.BatchFixer;
    }
}
