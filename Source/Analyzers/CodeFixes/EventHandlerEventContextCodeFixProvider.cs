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

namespace Dolittle.SDK.Analyzers.CodeFixes;

/// <summary>
/// Code fix provider for adding EventContext parameter to event handler methods
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AttributeMissingCodeFixProvider)), Shared]
public class EventHandlerEventContextCodeFixProvider : CodeFixProvider
{
    /// <inheritdoc />
    public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(
        DiagnosticIds.EventHandlerMissingEventContext
    );

    /// inheritdoc
    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;
    
    /// inheritdoc
    public override Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var document = context.Document;

        foreach (var diagnostic in context.Diagnostics)
        {
            switch (diagnostic.Id)
            {
                case DiagnosticIds.EventHandlerMissingEventContext:
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            "Add EventContext parameter",
                            ct => AddEventContextParameter(context, diagnostic, document, ct),
                            nameof(EventHandlerEventContextCodeFixProvider) + ".AddEventContext"),
                        diagnostic);
                    break;
            }
        }


        return Task.CompletedTask;
    }

    async Task<Document> AddEventContextParameter(CodeFixContext context, Diagnostic diagnostic, Document document, CancellationToken _)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        if (root is null) return document;

        // Find the method declaration identified by the diagnostic.
        var methodDeclaration = GetMethodDeclaration(root, diagnostic);
        if (methodDeclaration is null)
        {
            return document;
        }


        var updatedRoot = root.ReplaceNode(methodDeclaration, WithEventContextParameter(methodDeclaration));
        var newRoot = EnsureNamespaceImported((CompilationUnitSyntax)updatedRoot, "Dolittle.SDK.Events");
        return document.WithSyntaxRoot(newRoot);
    }

    /// <summary>
    /// Adds EventContext parameter to the method declaration
    /// </summary>
    /// <param name="methodDeclaration"></param>
    /// <returns></returns>
    static MethodDeclarationSyntax WithEventContextParameter(MethodDeclarationSyntax methodDeclaration)
    {
        var existingParameters = methodDeclaration.ParameterList.Parameters;
        // Get the first parameter that is not the EventContext parameter
        var eventParameter = existingParameters.FirstOrDefault(parameter => parameter.Type?.ToString() != "EventContext");
        if (eventParameter is null)
        {
            return methodDeclaration;
        }


        var eventContextParameter = SyntaxFactory.Parameter(SyntaxFactory.Identifier("ctx")).WithType(SyntaxFactory.ParseTypeName("EventContext"));

        var originalParameterList = methodDeclaration.ParameterList;
        var newParameterList = SyntaxFactory.ParameterList(
                SyntaxFactory.SeparatedList(
                    new[]
                    {
                        eventParameter,
                        eventContextParameter
                    }
                )
            ).WithLeadingTrivia(originalParameterList.GetLeadingTrivia())
            .WithTrailingTrivia(originalParameterList.GetTrailingTrivia());
        return methodDeclaration.WithParameterList(newParameterList);
    }

    MethodDeclarationSyntax? GetMethodDeclaration(SyntaxNode root, Diagnostic diagnostic)
    {
        var diagnosticSpan = diagnostic.Location.SourceSpan;
        var methodDeclaration = root.FindToken(diagnosticSpan.Start).Parent?.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().First();
        return methodDeclaration;
    }

    static CompilationUnitSyntax EnsureNamespaceImported(CompilationUnitSyntax root, string namespaceToInclude)
    {
        var usingDirective = SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(namespaceToInclude));
        var existingUsings = root.Usings;

        if (existingUsings.Any(u => u.Name?.ToFullString() == namespaceToInclude))
        {
            // Namespace is already imported.
            return root;
        }
        var lineEndingTrivia = root.DescendantTrivia().First(it => it.IsKind(SyntaxKind.EndOfLineTrivia));
        usingDirective = usingDirective.WithTrailingTrivia(lineEndingTrivia);

        return root.WithUsings(existingUsings.Add(usingDirective));
    }
}
