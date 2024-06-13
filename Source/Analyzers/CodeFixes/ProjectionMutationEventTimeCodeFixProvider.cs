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
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ProjectionMutationEventTimeCodeFixProvider)), Shared]
public class ProjectionMutationEventTimeCodeFixProvider : CodeFixProvider
{
    /// <inheritdoc />
    public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(
        DiagnosticIds.ProjectionMutationUsedCurrentTime
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
                case DiagnosticIds.ProjectionMutationUsedCurrentTime:
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            "Replace with EventContext.Occurred",
                            ct => ReplaceDateTimeNowWithEventContextOccurred(context, diagnostic, document, ct),
                            nameof(ProjectionMutationEventTimeCodeFixProvider) + ".UseEventContext"),
                        diagnostic);
                    break;
            }
        }


        return Task.CompletedTask;
    }

    async Task<Document> ReplaceDateTimeNowWithEventContextOccurred(CodeFixContext context, Diagnostic diagnostic,
        Document document, CancellationToken _)
    {
        if (!diagnostic.Properties.TryGetValue("expression", out var expression))
        {
            return document;
        }

        // Keep the type / kind of the expression
        var replacement = expression switch
        {
            "System.DateTime.Now" => "Occurred.DateTime",
            "System.DateTime.UtcNow" => "Occurred.UtcDateTime",
            "System.DateTimeOffset.Now" => "Occurred",
            "System.DateTimeOffset.UtcNow" => "Occurred",
            _ => null
        };

        if (replacement is null)
        {
            return document;
        }

        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        if (root is null) return document;

        var existingMethod = GetMethodDeclaration(root, diagnostic);
        if (existingMethod is null) return document;
        var dateTimeNode = root.FindNode(diagnostic.Location.SourceSpan);

        var (parameterList, eventContextParameterName) = GetParameterListAndContextName(existingMethod.ParameterList);


        root = root.ReplaceNodes(new[] { existingMethod.ParameterList, dateTimeNode }, (original, _) =>
        {
            if (original == existingMethod.ParameterList)
            {
                return parameterList;
            }

            return SyntaxFactory.ParseExpression(eventContextParameterName + "." + replacement);
        });


        root = EnsureNamespaceImported((CompilationUnitSyntax)root, "Dolittle.SDK.Events");

        return document.WithSyntaxRoot(root.NormalizeWhitespace(eol:"\n"));
    }

    static (ParameterListSyntax, string eventContextParameterName) GetParameterListAndContextName(
        ParameterListSyntax parameterList)
    {
        if (parameterList.Parameters.Count < 2) // Missing EventContext parameter
        {
            // Add the EventContext parameter
            return (WithEventContextParameter(parameterList), "ctx");
        }

        // Get the parameter name
        var parameterName = parameterList.Parameters[1].Identifier.Text;
        return (parameterList, parameterName);
    }

    static ParameterListSyntax WithEventContextParameter(ParameterListSyntax parameterList, string parameterName = "ctx")
    {
        var existingParameters = parameterList.Parameters;
        // Get the first parameter that is not the EventContext parameter
        var eventParameter = existingParameters.FirstOrDefault(parameter => parameter.Type?.ToString() != "EventContext");
        if (eventParameter is null)
        {
            return parameterList;
        }

        var eventContextParameter = SyntaxFactory.Parameter(SyntaxFactory.Identifier(parameterName))
            .WithType(SyntaxFactory.ParseTypeName("EventContext"));

        return parameterList.WithParameters(parameterList.Parameters.Insert(1, eventContextParameter));
    }

    MethodDeclarationSyntax? GetMethodDeclaration(SyntaxNode root, Diagnostic diagnostic)
    {
        var diagnosticSpan = diagnostic.Location.SourceSpan;
        var methodDeclaration = root.FindToken(diagnosticSpan.Start).Parent?.AncestorsAndSelf()
            .OfType<MethodDeclarationSyntax>().First();
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
