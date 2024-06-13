// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

namespace Dolittle.SDK.Analyzers.CodeFixes;

/// <summary>
/// Adds On-method (mutation) for the specific event type
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AggregateMutationCodeFixProvider)), Shared]
public class AggregateMutationCodeFixProvider : CodeFixProvider
{
    /// <inheritdoc />
    public override ImmutableArray<string> FixableDiagnosticIds { get; } =
        ImmutableArray.Create(DiagnosticIds.AggregateMissingMutationRuleId);

    /// <summary>
    /// Gets the fix all provider
    /// </summary>
    /// <returns></returns>
    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    /// <inheritdoc />
    public override Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var document = context.Document;
        var diagnostic = context.Diagnostics[0];
        if (!diagnostic.Properties.TryGetValue("typeName", out var eventType))
        {
            return Task.CompletedTask;
        }

        switch (diagnostic.Id)
        {
            case DiagnosticIds.AggregateMissingMutationRuleId:
                context.RegisterCodeFix(
                    CodeAction.Create(
                        "Generate On-method", ct => GenerateStub(context, document, eventType!, ct),
                        nameof(AggregateMutationCodeFixProvider) + ".AddMutation"),
                    diagnostic);
                break;
        }

        return Task.CompletedTask;
    }

    static async Task<Document> GenerateStub(CodeFixContext context, Document document, string eventType, CancellationToken ct)
    {
        if (await context.Document.GetSyntaxRootAsync(ct) is not CompilationUnitSyntax root) return document;

        var member = SyntaxFactory.ParseMemberDeclaration($"void On({eventType} evt) {{  }}\n\n");
        if (member is not MethodDeclarationSyntax method) return document;


        var classDeclaration = root.DescendantNodes().OfType<ClassDeclarationSyntax>()
            .First(declaration => declaration.Span.Contains(context.Span));

        var replacedNode = root.ReplaceNode(classDeclaration,
            Formatter.Format(WithMutationMethod(classDeclaration, method), document.Project.Solution.Workspace));

        return document.WithSyntaxRoot(replacedNode.WithLfLineEndings());
    }

    static SyntaxNode WithMutationMethod(ClassDeclarationSyntax classDeclaration, MethodDeclarationSyntax method)
    {
        var lastOnMethod = classDeclaration.Members
            .LastOrDefault(m => m is MethodDeclarationSyntax { Identifier.Text: "On" });
        var methodDeclarationSyntax = method.WithLeadingTrivia(SyntaxFactory.LineFeed);
        return lastOnMethod is null
            ? classDeclaration.AddMembers(methodDeclarationSyntax)
            : classDeclaration.InsertNodesAfter(lastOnMethod, new[] { methodDeclarationSyntax });
    }
}
