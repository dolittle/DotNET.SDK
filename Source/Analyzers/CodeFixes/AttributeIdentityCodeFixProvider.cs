// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
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

delegate string CreateIdentity();

/// <summary>
/// Generates a valid Guid identity for a given Dolittle identity attribute
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AttributeIdentityCodeFixProvider)), Shared]
public class AttributeIdentityCodeFixProvider : CodeFixProvider
{
    const string NoArgumentCorrespondsToRequiredParameter = "CS7036";
    
    /// <inheritdoc />
    public override ImmutableArray<string> FixableDiagnosticIds { get; } = [
        NoArgumentCorrespondsToRequiredParameter,
        DiagnosticIds.AttributeInvalidIdentityRuleId,
        DiagnosticIds.RedactionEventIncorrectPrefix];
    
    /// <inheritdoc />
    public override Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var document = context.Document;
        var diagnostic = context.Diagnostics[0];
        if(diagnostic.Id.Equals(NoArgumentCorrespondsToRequiredParameter, StringComparison.Ordinal))
        {
            RegisterCs7036IfApplicable(context, diagnostic, document);
            return Task.CompletedTask;
        }
        
        if (!diagnostic.Properties.TryGetValue("identityParameter", out var identityParameterName))
        {
            return Task.CompletedTask;
        }
        switch (diagnostic.Id)
        {
            case DiagnosticIds.AttributeInvalidIdentityRuleId:
                context.RegisterCodeFix(
                    CodeAction.Create(
                        "Generate identity", ct => UpdateIdentity(context, document, identityParameterName!,IdentityGenerator.Generate , ct),
                        nameof(AttributeIdentityCodeFixProvider) + ".AddIdentity"),
                    diagnostic);
                break;
            case DiagnosticIds.RedactionEventIncorrectPrefix:
                context.RegisterCodeFix(
                    CodeAction.Create(
                        "Generate redaction identity", ct => UpdateIdentity(context, document, identityParameterName!,IdentityGenerator.GenerateRedactionId , ct),
                        nameof(AttributeIdentityCodeFixProvider) + ".AddRedactionIdentity"),
                    diagnostic);
                break;
        }

        return Task.CompletedTask;
    }

    void RegisterCs7036IfApplicable(CodeFixContext context, Diagnostic diagnostic, Document document)
    {
        try
        {
            var (identityParameterName, type) = Extract7036Arguments(diagnostic);

            switch (type, identityParameterName)
            {
                case ("EventTypeAttribute", "eventTypeId"):
                case ("EventHandlerAttribute", "eventHandlerId"):
                case ("ProjectionAttribute", "projectionId"):
                case ("AggregateRootAttribute", "id"):
                    break;
                default:
                    return;
            }

            context.RegisterCodeFix(
                CodeAction.Create(
                    "Add identity",
                    ct => AddIdentity(context, document, identityParameterName, IdentityGenerator.Generate, ct),
                    nameof(AttributeIdentityCodeFixProvider) + ".AddIdentity"),
                diagnostic);

        }
        catch
        {
            // ignored
        }
    }

    (string parameterName, string typeName) Extract7036Arguments(Diagnostic diagnostic)
    {
        var message = diagnostic.GetMessage();
        var parts = message.Split('\'');
        return (parts[1], parts[3].Split('.').FirstOrDefault());
    }

    static async Task<Document> UpdateIdentity(CodeFixContext context, Document document, string identityParameterName,
        CreateIdentity createIdentity,
        CancellationToken cancellationToken)
    {
        var root = await context.Document.GetSyntaxRootAsync(cancellationToken);
        if (root is null) return document;
        if (!TryGetTargetNode(context, root, out AttributeSyntax attribute)) return document; // Target not found
        var updatedRoot = root.ReplaceNode(attribute, GenerateIdentityAttribute(attribute, identityParameterName, createIdentity));
        return document.WithSyntaxRoot(updatedRoot);
    }
    
    static async Task<Document> AddIdentity(CodeFixContext context, Document document, string identityParameterName,
        CreateIdentity createIdentity,
        CancellationToken cancellationToken)
    {
        var root = await context.Document.GetSyntaxRootAsync(cancellationToken);
        if (root is null) return document;
        if (!TryGetTargetNode(context, root, out AttributeSyntax attribute)) return document; // Target not found
        var updatedRoot = root.ReplaceNode(attribute, GenerateIdentityAttribute(attribute, identityParameterName, createIdentity));
        return document.WithSyntaxRoot(updatedRoot);
    }

    static AttributeSyntax GenerateIdentityAttribute(AttributeSyntax existing, string identityParameterName, CreateIdentity generate)
    {
        var newIdentity = SyntaxFactory.ParseExpression("\"" + generate.Invoke() + "\"");
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
}
