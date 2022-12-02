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

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AggregateMutationCodeFixProvider)), Shared]
public class AggregateMutationCodeFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(DiagnosticIds.AggregateMissingMutationRuleId);

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

    async Task<Document> GenerateStub(CodeFixContext context, Document document, string eventType, CancellationToken ct)
    {
        if(await context.Document.GetSyntaxRootAsync(ct) is not CompilationUnitSyntax root) return document;
        
        var member = SyntaxFactory.ParseMemberDeclaration($"private void On({eventType} @event) => throw new NotImplementedException();");
        if (member is not MethodDeclarationSyntax method) return document;

        var classDeclaration = root.DescendantNodes().OfType<ClassDeclarationSyntax>().First(declaration => declaration.Span.Contains(context.Span));

        var replacedNode = root.ReplaceNode(classDeclaration,
            Formatter.Format(classDeclaration.AddMembers(method.WithLeadingTrivia(SyntaxFactory.LineFeed)),
                document.Project.Solution.Workspace));

        return document.WithSyntaxRoot(replacedNode.AddMissingUsingDirectives("System").WithLfLineEndings());
    }
}
