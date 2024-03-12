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
/// Adds the missing attribute to the target type
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AttributeMissingCodeFixProvider)), Shared]
public class AttributeMissingCodeFixProvider : CodeFixProvider
{
    /// <inheritdoc />
    public override ImmutableArray<string> FixableDiagnosticIds { get; } =
        ImmutableArray.Create(
            DiagnosticIds.AggregateMissingAttributeRuleId,
            DiagnosticIds.EventMissingAttributeRuleId,
            DiagnosticIds.ProjectionMissingAttributeRuleId);

    /// <inheritdoc />
    public override Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var document = context.Document;
        var diagnostic = context.Diagnostics[0];

        if (!diagnostic.Properties.TryGetValue("targetClass", out var className) || className is null) return Task.CompletedTask;
        if (!diagnostic.Properties.TryGetValue("attributeClass", out var attributeName) || attributeName is null) return Task.CompletedTask;

        switch (diagnostic.Id)
        {
            case DiagnosticIds.AggregateMissingAttributeRuleId:
            case DiagnosticIds.EventMissingAttributeRuleId:
            case DiagnosticIds.ProjectionMissingAttributeRuleId:
                context.RegisterCodeFix(
                    CodeAction.Create(
                        "Generate attribute",
                        ct => GenerateIdentityAttribute(context, document, className, attributeName, ct),
                        nameof(AttributeMissingCodeFixProvider) + ".AddAggregateAttribute"),
                    diagnostic);
                break;
        }

        return Task.CompletedTask;
    }


    static async Task<Solution> GenerateIdentityAttribute(CodeFixContext context, Document document, string targetClass, string attributeClass,
        CancellationToken cancellationToken)
    {
        var semanticModel = await context.Document.GetSemanticModelAsync(cancellationToken);
        if (semanticModel is null) return document.Project.Solution;
        var targetType = semanticModel.Compilation.GetTypeByMetadataName(targetClass);
        if (targetType is null) return document.Project.Solution;
        var attributeType = semanticModel.Compilation.GetTypeByMetadataName(attributeClass);
        if (attributeType is null) return document.Project.Solution;


        // Add the attribute of attributeType to targetType, even if it is in another project
        var targetTypeSyntaxReference = targetType.DeclaringSyntaxReferences[0];
        if (await targetTypeSyntaxReference.GetSyntaxAsync(cancellationToken) is not TypeDeclarationSyntax classSyntax)
            return document.Project.Solution;

        var updatedSyntax = WithIdentityAttribute(classSyntax, attributeType);

        var targetDocument = document.Project.Solution.GetDocument(targetTypeSyntaxReference.SyntaxTree);
        if (targetDocument is null) return document.Project.Solution;

        if (await targetDocument.GetSyntaxRootAsync(cancellationToken) is not CompilationUnitSyntax existingRoot) return document.Project.Solution;

        var updatedRoot = existingRoot
            .ReplaceNode(classSyntax, updatedSyntax);

        // Add missing using statements to the updated root
        updatedRoot = updatedRoot.AddMissingUsingDirectives(attributeType.ContainingNamespace);

        var updatedDocument = targetDocument.WithSyntaxRoot(updatedRoot);

        return updatedDocument.Project.Solution;
    }


    static TypeDeclarationSyntax WithIdentityAttribute(TypeDeclarationSyntax existing, INamedTypeSymbol attributeClass)
    {
        var attributeSyntax = CreateIdentifierAttribute(attributeClass);
        return existing.AddAttributeLists(SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList(attributeSyntax)));
    }

    static AttributeSyntax CreateIdentifierAttribute(INamedTypeSymbol attributeClass)
    {
        var attributeList = SyntaxFactory.ParseAttributeArgumentList($"(\"{IdentityGenerator.Generate()}" + "\")");
        var identifier = attributeClass.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
        if (identifier.EndsWith("Attribute"))
        {
            identifier = identifier.Substring(0, identifier.Length - "Attribute".Length);
        }

        // Adds the AttributeArgumentList to the existing type
        var attributeSyntax = SyntaxFactory.Attribute(SyntaxFactory.IdentifierName(identifier),
            attributeList);
        return attributeSyntax;
    }
}
