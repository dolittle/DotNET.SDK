// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dolittle.SDK.Analyzers;

#pragma warning disable CS1574, CS1584, CS1581, CS1580
/// <summary>
/// Analyzer for <see cref="Dolittle.SDK.Aggregates.AggregateRoot"/>.
/// </summary>
#pragma warning restore CS1574, CS1584, CS1581, CS1580
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class AggregateAnalyzer : DiagnosticAnalyzer
{
    static readonly ImmutableDictionary<string, string?> _targetVisibilityPrivate = ImmutableDictionary
        .Create<string, string?>()
        .Add("targetVisibility", "private");

    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(
            DescriptorRules.ExceptionInMutation,
            DescriptorRules.Aggregate.MissingAttribute,
            DescriptorRules.Aggregate.MissingMutation,
            DescriptorRules.Aggregate.MutationShouldBePrivate,
            DescriptorRules.Aggregate.MutationHasIncorrectNumberOfParameters,
            DescriptorRules.Aggregate.MutationsCannotProduceEvents,
            DescriptorRules.Events.MissingAttribute,
            DescriptorRules.Aggregate.PublicMethodsCannotMutateAggregateState,
            DescriptorRules.Aggregate.MutationsCannotUseCurrentTime
        );

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeAggregates, ImmutableArray.Create(SyntaxKind.ClassDeclaration));
    }


    static void AnalyzeAggregates(SyntaxNodeAnalysisContext context)
    {
        // Check if the symbol has the aggregate root base class
        var aggregateSyntax = (ClassDeclarationSyntax)context.Node;
        // Check if the symbol has the aggregate root base class
        var aggregateSymbol = context.SemanticModel.GetDeclaredSymbol(aggregateSyntax);
        if (aggregateSymbol?.IsAggregateRoot() != true) return;

        CheckAggregateRootAttributePresent(context, aggregateSymbol);


        var handledEvents = CheckOnMethods(context, aggregateSymbol);
        CheckApplyInvocations(context, aggregateSyntax, handledEvents);
        CheckApplyInvocationsInOnMethods(context, aggregateSymbol);
        CheckMutationsInPublicMethods(context, aggregateSymbol);
    }


    static HashSet<ITypeSymbol> CheckOnMethods(SyntaxNodeAnalysisContext context, INamedTypeSymbol aggregateType)
    {
        var members = aggregateType.GetMembers();
        var onMethods = members.Where(_ => _.Name.Equals("On")).OfType<IMethodSymbol>().ToArray();
        var eventTypesHandled = new HashSet<ITypeSymbol>(SymbolEqualityComparer.Default);

        foreach (var onMethod in onMethods)
        {
            if (onMethod.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax() is not MethodDeclarationSyntax syntax)
                continue;

            if (syntax.Modifiers.Any(SyntaxKind.PublicKeyword)
                || syntax.Modifiers.Any(SyntaxKind.InternalKeyword)
                || syntax.Modifiers.Any(SyntaxKind.ProtectedKeyword))
            {
                context.ReportDiagnostic(Diagnostic.Create(DescriptorRules.Aggregate.MutationShouldBePrivate,
                    syntax.GetLocation(),
                    _targetVisibilityPrivate, onMethod.ToDisplayString()));
            }

            var parameters = onMethod.Parameters;
            if (parameters.Length != 1)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    DescriptorRules.Aggregate.MutationHasIncorrectNumberOfParameters, syntax.GetLocation(),
                    onMethod.ToDisplayString()));
            }

            EnsureMutationDoesNotAccessCurrentTime(context, syntax);
            EnsureMutationDoesNotThrowExceptions(context, syntax);


            if (parameters.Length > 0)
            {
                var eventType = parameters[0].Type;
                eventTypesHandled.Add(eventType);

                if (!eventType.HasEventTypeAttribute())
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        DescriptorRules.Events.MissingAttribute,
                        parameters[0].DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax().GetLocation(),
                        eventType.ToTargetClassAndAttributeProps(DolittleConstants.Types.EventTypeAttribute),
                        eventType.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat))
                    );
                }
            }
        }

        return eventTypesHandled;
    }

    static void EnsureMutationDoesNotThrowExceptions(SyntaxNodeAnalysisContext context,
        MethodDeclarationSyntax onMethod)
    {
        var throwStatements = onMethod.DescendantNodes().OfType<ThrowStatementSyntax>().ToArray();
        foreach (var throwStatement in throwStatements)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                DescriptorRules.ExceptionInMutation,
                throwStatement.GetLocation()
            ));
        }

        var throwIfMethods = onMethod.DescendantNodes()
            .OfType<InvocationExpressionSyntax>()
            .Where(invocation =>
                invocation.Expression is MemberAccessExpressionSyntax { Name: IdentifierNameSyntax identifier } &&
                identifier.Identifier.ValueText.StartsWith("ThrowIf", StringComparison.Ordinal))
            .ToArray();
        
        foreach (var throwIfMethod in throwIfMethods)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                DescriptorRules.ExceptionInMutation,
                throwIfMethod.GetLocation()
            ));
        }
    }

    /// <summary>
    /// Checks if the method gets the current time via DateTime or DateTimeOffset
    /// Since this is not allowed for the mutations, we need to report a diagnostic
    /// </summary>
    /// <param name="context"></param>
    /// <param name="onMethod"></param>
    static void EnsureMutationDoesNotAccessCurrentTime(SyntaxNodeAnalysisContext context,
        MethodDeclarationSyntax onMethod)
    {
        var currentTimeInvocations = onMethod.DescendantNodes()
            .OfType<MemberAccessExpressionSyntax>()
            .Where(memberAccess =>
            {
                var now = memberAccess.Name
                    is IdentifierNameSyntax { Identifier.Text: "Now" }
                    or IdentifierNameSyntax { Identifier.Text: "UtcNow" };
                if (!now)
                {
                    return false;
                }

                var typeInfo = context.SemanticModel.GetTypeInfo(memberAccess.Expression);
                // Check if the type is DateTime or DateTimeOffset
                return typeInfo.Type?.ToDisplayString() == "System.DateTime" ||
                       typeInfo.Type?.ToDisplayString() == "System.DateTimeOffset";
            }).ToArray();

        foreach (var currentTimeInvocation in currentTimeInvocations)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                DescriptorRules.Aggregate.MutationsCannotUseCurrentTime,
                currentTimeInvocation.GetLocation(),
                new[] { currentTimeInvocation.ToFullString() }
            ));
        }
    }

    static void CheckAggregateRootAttributePresent(SyntaxNodeAnalysisContext context, INamedTypeSymbol aggregateClass)
    {
        var hasAttribute = aggregateClass.GetAttributes()
            .Any(attribute =>
                attribute.AttributeClass?.ToDisplayString()
                    .Equals(DolittleConstants.Types.AggregateRootAttribute, StringComparison.Ordinal) == true);

        if (!hasAttribute)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                DescriptorRules.Aggregate.MissingAttribute,
                aggregateClass.Locations[0],
                aggregateClass.ToTargetClassAndAttributeProps(DolittleConstants.Types.AggregateRootAttribute),
                aggregateClass.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)
            ));
        }
    }


    static void CheckApplyInvocations(SyntaxNodeAnalysisContext context, ClassDeclarationSyntax aggregateClassSyntax,
        ISet<ITypeSymbol> handledEventTypes)
    {
        var semanticModel = context.SemanticModel;
        foreach (var invocation in aggregateClassSyntax.DescendantNodes().OfType<InvocationExpressionSyntax>())
        {
            if (invocation.Expression is not IdentifierNameSyntax { Identifier.Text: "Apply" }) continue;
            if (invocation.ArgumentList.Arguments.Count != 1) continue;
            var argument = invocation.ArgumentList.Arguments[0];
            var typeInfo = semanticModel.GetTypeInfo(argument.Expression);
            if (typeInfo.Type is not { } type) continue;
            if (!type.HasEventTypeAttribute())
            {
                context.ReportDiagnostic(Diagnostic.Create(DescriptorRules.Events.MissingAttribute,
                    invocation.GetLocation(),
                    type.ToTargetClassAndAttributeProps(DolittleConstants.Types.EventTypeAttribute), type.ToString()));
            }

            if (!handledEventTypes.Contains(type))
            {
                context.ReportDiagnostic(Diagnostic.Create(DescriptorRules.Aggregate.MissingMutation,
                    invocation.GetLocation(), type.ToMinimalTypeNameProps(),
                    type.ToString()));
            }
        }
    }

    static void CheckApplyInvocationsInOnMethods(SyntaxNodeAnalysisContext context, INamedTypeSymbol aggregateType)
    {
        var onMethods = aggregateType
            .GetMembers()
            .Where(member => member.Name.Equals("On"))
            .OfType<IMethodSymbol>()
            .Where(method => !method.DeclaredAccessibility.HasFlag(Accessibility.Public))
            .ToArray();

        foreach (var onMethod in onMethods)
        {
            if (onMethod.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax() is not MethodDeclarationSyntax syntax)
            {
                continue;
            }

            var applyInvocations = syntax
                .DescendantNodes()
                .OfType<InvocationExpressionSyntax>()
                .Where(invocation => invocation.Expression is IdentifierNameSyntax { Identifier.Text: "Apply" })
                .ToArray();

            foreach (var applyInvocation in applyInvocations)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    DescriptorRules.Aggregate.MutationsCannotProduceEvents,
                    applyInvocation.GetLocation(),
                    new[] { onMethod.ToDisplayString() }
                ));
            }

            var memberApplyInvocations = syntax
                .DescendantNodes()
                .OfType<MemberAccessExpressionSyntax>()
                .Where(memberAccess => memberAccess.Name is IdentifierNameSyntax { Identifier.Text: "Apply" })
                .Where(memberAccess => memberAccess.Name is IdentifierNameSyntax { Identifier.Text: "Apply" } &&
                                       memberAccess.Expression is ThisExpressionSyntax or BaseExpressionSyntax)
                .ToArray();

            foreach (var invocation in memberApplyInvocations)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    DescriptorRules.Aggregate.MutationsCannotProduceEvents,
                    invocation.GetLocation(),
                    new[] { onMethod.ToDisplayString() }
                ));
            }
        }
    }

    static void CheckMutationsInPublicMethods(SyntaxNodeAnalysisContext context, INamedTypeSymbol aggregateType)
    {
        var publicMethods = aggregateType
            .GetMembers()
            .Where(member => !member.Name.Equals("On"))
            .OfType<IMethodSymbol>()
            .Where(method => method.DeclaredAccessibility.HasFlag(Accessibility.Public))
            .ToArray();
        if (publicMethods.Length == 0)
        {
            return;
        }

        var walker = new MutationWalker(context, aggregateType);

        foreach (var onMethod in publicMethods)
        {
            if (onMethod.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax() is not MethodDeclarationSyntax syntax)
            {
                continue;
            }

            walker.Visit(syntax);
        }
    }

    class MutationWalker : CSharpSyntaxWalker
    {
        readonly SyntaxNodeAnalysisContext _context;
        readonly INamedTypeSymbol _aggregateType;

        public MutationWalker(SyntaxNodeAnalysisContext context, INamedTypeSymbol aggregateType)
        {
            _context = context;
            _aggregateType = aggregateType;
        }

        public override void VisitAssignmentExpression(AssignmentExpressionSyntax node)
        {
            var leftExpression = node.Left;

            if (leftExpression is IdentifierNameSyntax || leftExpression is MemberAccessExpressionSyntax)
            {
                var symbolInfo = _context.SemanticModel.GetSymbolInfo(leftExpression);
                if (symbolInfo.Symbol is IFieldSymbol || symbolInfo.Symbol is IPropertySymbol)
                {
                    var containingType = symbolInfo.Symbol.ContainingType;
                    if (containingType != null && SymbolEqualityComparer.Default.Equals(_aggregateType, containingType))
                    {
                        var diagnostic =
                            Diagnostic.Create(DescriptorRules.Aggregate.PublicMethodsCannotMutateAggregateState,
                                leftExpression.GetLocation());
                        _context.ReportDiagnostic(diagnostic);
                    }
                }
            }

            base.VisitAssignmentExpression(node);
        }

        // You can also add other types of mutations like increments, decrements, method calls etc.
    }
}
