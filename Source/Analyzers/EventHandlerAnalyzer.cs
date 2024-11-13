// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dolittle.SDK.Analyzers;

/// <summary>
/// This analyzer checks for common mistakes in event handlers.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class EventHandlerAnalyzer : DiagnosticAnalyzer
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
    [
        DescriptorRules.InvalidTimestamp,
        DescriptorRules.InvalidStartStopTimestamp,
        DescriptorRules.InvalidAccessibility,
        DescriptorRules.Events.MissingAttribute,
        DescriptorRules.Events.MissingEventContext,
    ];

    const int StartFromTimestampOffset = 6;
    const int StopAtTimestampOffset = 7;

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(AnalyzeEventHandler, ImmutableArray.Create(SyntaxKind.ClassDeclaration));
    }

    void AnalyzeEventHandler(SyntaxNodeAnalysisContext context)
    {
        var classSyntax = (ClassDeclarationSyntax)context.Node;
        var classSymbol = context.SemanticModel.GetDeclaredSymbol(classSyntax);
        if (classSymbol is null || !classSymbol.HasAttribute(DolittleConstants.Types.EventHandlerAttribute))
        {
            return;
        }

        AnalyzeEventHandlerAttribute(context, classSymbol);
        AnalyzeHandleMethods(context, classSymbol);
    }

    void AnalyzeHandleMethods(SyntaxNodeAnalysisContext context, INamedTypeSymbol classSymbol)
    {
        var handleMethods = classSymbol.GetMembers()
            .OfType<IMethodSymbol>()
            .Where(method => method.Name == "Handle");

        foreach (var handleMethod in handleMethods)
        {
            AnalyzeHandleMethod(context, handleMethod);
        }
    }

    void AnalyzeHandleMethod(SyntaxNodeAnalysisContext context, IMethodSymbol handleMethod)
    {
        // Check that it is public
        if (!handleMethod.DeclaredAccessibility.HasFlag(Accessibility.Public))
        {
            var diagnostic = Diagnostic.Create(DescriptorRules.InvalidAccessibility, handleMethod.Locations[0],
                handleMethod.Name, Accessibility.Public);
            context.ReportDiagnostic(diagnostic);
        }

        // Get the first parameter and get the type
        var parameter = handleMethod.Parameters.FirstOrDefault();
        if (parameter is null)
        {
            return;
        }

        // Check if the type has the EventType attribute
        if (!parameter.Type.HasEventTypeAttribute())
        {
            var diagnostic = Diagnostic.Create(DescriptorRules.Events.MissingAttribute, parameter.Locations[0],
                parameter.Type.ToTargetClassAndAttributeProps(DolittleConstants.Types.EventTypeAttribute),
                parameter.Type.Name);
            context.ReportDiagnostic(diagnostic);
        }

        // Check that the method takes an EventContext as the second parameter
        var secondParameter = handleMethod.Parameters.Skip(1).FirstOrDefault();
        if (secondParameter is null || secondParameter.Type.ToString() != DolittleConstants.Types.EventContext)
        {
            var diagnostic = Diagnostic.Create(DescriptorRules.Events.MissingEventContext, handleMethod.Locations[0],
                handleMethod.Name);
            context.ReportDiagnostic(diagnostic);
        }
    }

    static void AnalyzeEventHandlerAttribute(SyntaxNodeAnalysisContext context, INamedTypeSymbol classSymbol)
    {
        var eventHandlerAttribute = classSymbol.GetAttributes()
            .Single(attribute =>
                attribute.AttributeClass?.ToDisplayString() == DolittleConstants.Types.EventHandlerAttribute);
        if (eventHandlerAttribute.ApplicationSyntaxReference?.GetSyntax(context.CancellationToken) is not
            AttributeSyntax attributeSyntaxNode)
        {
            return;
        }

        AnalyzeEventHandlerStartFromStopAt(context, eventHandlerAttribute, attributeSyntaxNode);
    }


    static void AnalyzeEventHandlerStartFromStopAt(SyntaxNodeAnalysisContext context,
        AttributeData eventHandlerAttribute, AttributeSyntax syntaxNode)
    {
        var startFrom = GetAttributeArgument(eventHandlerAttribute, syntaxNode, StartFromTimestampOffset,
            "startFromTimestamp");
        if (startFrom?.value is not null &&
            !DateTimeOffset.TryParse(startFrom.Value.value.ToString(), out var parsedStartFrom))
        {
            var diagnostic = Diagnostic.Create(DescriptorRules.InvalidTimestamp, startFrom.Value.location,
                "startFromTimestamp");
            context.ReportDiagnostic(diagnostic);
        }

        var stopAt = GetAttributeArgument(eventHandlerAttribute, syntaxNode, StopAtTimestampOffset, "stopAtTimestamp");
        if (stopAt?.value is not null && !DateTimeOffset.TryParse(stopAt.Value.value.ToString(), out var parsedStopAt))
        {
            var diagnostic =
                Diagnostic.Create(DescriptorRules.InvalidTimestamp, stopAt.Value.location, "stopAtTimestamp");
            context.ReportDiagnostic(diagnostic);
        }

        if (parsedStartFrom > DateTimeOffset.MinValue && parsedStopAt > DateTimeOffset.MinValue &&
            parsedStartFrom >= parsedStopAt)
        {
            var diagnostic = Diagnostic.Create(DescriptorRules.InvalidStartStopTimestamp, syntaxNode.GetLocation(),
                "startFromTimestamp", "stopAtTimestamp");
            context.ReportDiagnostic(diagnostic);
        }
    }

    public static (object? value, Location location)? GetAttributeArgument(AttributeData eventHandlerAttribute,
        AttributeSyntax syntaxNode, int offset,
        string argumentName)
    {
        if (syntaxNode.ArgumentList is null)
        {
            return null;
        }

        // Handle positional arguments
        if (offset > eventHandlerAttribute.ConstructorArguments.Length)
        {
            return null;
        }

        var argument = eventHandlerAttribute.ConstructorArguments[offset];

        foreach (var argumentSyntax in syntaxNode.ArgumentList.Arguments)
        {
            if (argumentSyntax.NameColon?.Name?.Identifier.Text.Equals(argumentName,
                    StringComparison.OrdinalIgnoreCase) == true)
            {
                return (argument.Value, argumentSyntax.GetLocation());
            }
        }

        if (syntaxNode.ArgumentList.Arguments.Count > offset)
        {
            var positionalSyntax = syntaxNode.ArgumentList.Arguments[offset];
            return (argument.Value, positionalSyntax.GetLocation());
        }

        return (argument.Value, syntaxNode.GetLocation());
    }
}
