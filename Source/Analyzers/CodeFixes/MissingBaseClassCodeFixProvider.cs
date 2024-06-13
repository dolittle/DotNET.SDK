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

/// <summary>
/// Codefix provider for adding missing base classes according to the diagnostic
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MissingBaseClassCodeFixProvider))]
public class MissingBaseClassCodeFixProvider : CodeFixProvider
{
    /// <inheritdoc />
    public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(DiagnosticIds.MissingBaseClassRuleId);

    /// <inheritdoc />
    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    /// <inheritdoc />
    public override Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var diagnostic = context.Diagnostics.First();
        var missingClass = diagnostic.Properties["baseClass"];
        if (missingClass is null)
        {
            return Task.CompletedTask;
        }

        var title = $"Add base class: '{missingClass}'";
        context.RegisterCodeFix(
            CodeAction.Create(
                title: title,
                createChangedDocument: c => AddBaseClassAsync(context.Document, diagnostic, missingClass, c),
                equivalenceKey: title),
            diagnostic);

        return Task.CompletedTask;
    }

   
    
    static async Task<Document> AddBaseClassAsync(Document document, Diagnostic diagnostic, string missingClass, CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
        if (root is null || semanticModel is null) return document;
    
        var node = root.FindNode(diagnostic.Location.SourceSpan);
        var classDeclaration = node.FirstAncestorOrSelf<ClassDeclarationSyntax>();
        if (classDeclaration is null) return document;

        var baseClassType = semanticModel.Compilation.GetTypeByMetadataName(missingClass);
        if (baseClassType is null) return document;

        var newClassDeclaration = classDeclaration.AddBaseListTypes(SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(baseClassType.Name))).NormalizeWhitespace();
    
        // Add using directive for the namespace if it's not already there
        var namespaceToAdd = baseClassType.ContainingNamespace?.ToDisplayString();
        if (!string.IsNullOrEmpty(namespaceToAdd) && root is CompilationUnitSyntax compilationUnitSyntax &&
            !NamespaceImported(compilationUnitSyntax, namespaceToAdd!))
        {
            var usingDirective = SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(namespaceToAdd!))
                .NormalizeWhitespace(); // Ensure proper formatting
            compilationUnitSyntax = compilationUnitSyntax.AddUsings(usingDirective);
            root = compilationUnitSyntax;
        }
    
        var newRoot = root.ReplaceNode(classDeclaration, newClassDeclaration);
        return document.WithSyntaxRoot(newRoot);
    }



    static bool NamespaceImported(CompilationUnitSyntax root, string namespaceName)
    {
        return root.Usings.Any(usingDirective => usingDirective.Name?.ToString() == namespaceName);
    }
}
