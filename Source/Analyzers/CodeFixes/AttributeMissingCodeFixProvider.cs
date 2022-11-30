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

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AttributeMissingCodeFixProvider)), Shared]
public class AttributeMissingCodeFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds { get; } =
        ImmutableArray.Create(DiagnosticIds.AggregateMissingAttributeRuleId, DiagnosticIds.EventMissingAttributeRuleId);

    public override Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var document = context.Document;
        var diagnostic = context.Diagnostics[0];

        switch (diagnostic.Id)
        {
            case DiagnosticIds.AggregateMissingAttributeRuleId:
                context.RegisterCodeFix(
                    CodeAction.Create(
                        "Generate attribute",
                        ct => GenerateIdentityAttribute(context, document,"AggregateRoot", ct),
                        nameof(AttributeMissingCodeFixProvider) + ".AddAttribute"),
                    diagnostic);
                break;
        }

        return Task.CompletedTask;
    }


    static async Task<Document> GenerateIdentityAttribute(CodeFixContext context, Document document, string attributeClass, CancellationToken cancellationToken)
    {
        var root = await context.Document.GetSyntaxRootAsync(cancellationToken);
        if (root is null) return document;
        if (!TryGetTargetNode(context, root, out ClassDeclarationSyntax classSyntax)) return document; // Target not found

        var updatedRoot = root.ReplaceNode(classSyntax, GenerateIdentityAttribute(classSyntax, attributeClass));
        return document.WithSyntaxRoot(updatedRoot);
    }

    static ClassDeclarationSyntax GenerateIdentityAttribute(ClassDeclarationSyntax existing, string attributeClass)
    {
        var newIdentity = SyntaxFactory.ParseExpression("\"" + IdentityGenerator.Generate() + "\"");

        var attribute = SyntaxFactory.Attribute(SyntaxFactory.IdentifierName(attributeClass), SyntaxFactory.AttributeArgumentList(
            SyntaxFactory.SingletonSeparatedList(
                SyntaxFactory.AttributeArgument(newIdentity))));
        var attributeListSyntax = SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList(attribute));

        return existing.WithAttributeLists(
            existing.AttributeLists.Any()
                ? existing.AttributeLists.Add(attributeListSyntax)
                : SyntaxFactory.SingletonList(attributeListSyntax));
    }

    protected static bool TryGetTargetNode<TNode>(
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
