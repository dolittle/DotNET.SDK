// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Dolittle.SDK.Analyzers.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MethodVisibilityCodeFixProvider))]
public class MethodVisibilityCodeFixProvider : CodeFixProvider
{
    public sealed override ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create(DiagnosticIds.InvalidAccessibility,
            DiagnosticIds.AggregateMutationShouldBePrivateRuleId);

    public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public sealed override Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var diagnostic = context.Diagnostics.First();
        if (!diagnostic.Properties.TryGetValue("targetVisibility", out var targetVisibility) || string.IsNullOrWhiteSpace(targetVisibility))
        {
            return Task.CompletedTask;
        }

        switch (targetVisibility)
        {
            case "public":
                RegisterMakePublicCodeFix(context, diagnostic);
                break;
            case "private":
                RegisterMakePrivateCodeFix(context, diagnostic);
                break;
        }

        return Task.CompletedTask;
    }

    void RegisterMakePublicCodeFix(CodeFixContext context, Diagnostic diagnostic)
    {
        context.RegisterCodeFix(
            CodeAction.Create(
                title: "Make method public",
                createChangedDocument: c => MakePublicAsync(context, diagnostic, c),
                equivalenceKey: "Make method public"),
            diagnostic);
    }

    void RegisterMakePrivateCodeFix(CodeFixContext context, Diagnostic diagnostic)
    {
        context.RegisterCodeFix(
            CodeAction.Create(
                title: "Make method private",
                createChangedDocument: c => MakePrivateAsync(context, diagnostic, c),
                equivalenceKey: "Make method private"),
            diagnostic);
    }

    async Task<Document> MakePublicAsync(CodeFixContext context, Diagnostic diagnostic, CancellationToken cancellationToken)
    {
        var syntaxRoot = await context.Document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

        var methodDecl = syntaxRoot!.FindToken(diagnostic.Location.SourceSpan.Start).Parent!.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().First();

        // find existing modifier
        var modifier = methodDecl.Modifiers.FirstOrDefault(m =>
            m.IsKind(SyntaxKind.PrivateKeyword) ||
            m.IsKind(SyntaxKind.InternalKeyword) ||
            m.IsKind(SyntaxKind.ProtectedKeyword));

        var newMethod = methodDecl;
        // If exists remove it
        if (modifier != null)
        {
            newMethod = methodDecl.WithModifiers(methodDecl
                .Modifiers.Remove(modifier)
                .Add(SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
                .WithLeadingTrivia(modifier.LeadingTrivia);
        }
        else
        {
            var newModifiers = methodDecl.Modifiers.Add(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
            newMethod = methodDecl.WithModifiers(newModifiers);
            
        }
        // Add the 'public' modifier

        var newSyntaxRoot = syntaxRoot.ReplaceNode(methodDecl, newMethod);

        return context.Document.WithSyntaxRoot(newSyntaxRoot);
    }

    async Task<Document> MakePrivateAsync(CodeFixContext context, Diagnostic diagnostic, CancellationToken cancellationToken)
    {
        var syntaxRoot = await context.Document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

        var methodDecl = syntaxRoot!.FindToken(diagnostic.Location.SourceSpan.Start).Parent!.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().First();

        // find existing modifier
        var modifier = methodDecl.Modifiers.FirstOrDefault(m =>
            m.IsKind(SyntaxKind.PublicKeyword) ||
            m.IsKind(SyntaxKind.InternalKeyword) ||
            m.IsKind(SyntaxKind.ProtectedKeyword));

        // Remove public modifier, but keep the leading trivia
        var newMethod = methodDecl.WithModifiers(methodDecl.Modifiers.Remove(modifier)).WithLeadingTrivia(modifier.LeadingTrivia);
        var newSyntaxRoot = syntaxRoot.ReplaceNode(methodDecl, newMethod);

        return context.Document.WithSyntaxRoot(newSyntaxRoot);
    }
}
