// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dolittle.SDK.Analyzers;

/// <summary>
/// Analyzer for <see cref="Dolittle.SDK.Aggregates.AggregateRoot"/>.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class AggregateAnalyzer : DiagnosticAnalyzer
{
    record Types(INamedTypeSymbol AggregateRoot, INamedTypeSymbol AggregateAttribute, INamedTypeSymbol EventAttribute)
    {
        public INamedTypeSymbol AggregateRoot { get; } = AggregateRoot;
        public INamedTypeSymbol AggregateAttribute { get; } = AggregateAttribute;
        public INamedTypeSymbol EventAttribute { get; } = EventAttribute;
    }


    const string AggregateRootBaseClass = "Dolittle.SDK.Aggregates.AggregateRoot";
    const string AggregateRootAttribute = "Dolittle.SDK.Aggregates.AggregateRootAttribute";
    const string EventTypeAttribute = "Dolittle.SDK.Events.EventTypeAttribute";

    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(
            DescriptorRules.Aggregate.MissingAttribute,
            DescriptorRules.Aggregate.MissingMutation,
            DescriptorRules.Aggregate.MutationShouldBePrivate,
            DescriptorRules.Aggregate.MutationHasIncorrectNumberOfParameters,
            DescriptorRules.Events.MissingAttribute
        );

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterCompilationStartAction(compilationContext =>
        {
            var types = GetRelevantTypes(compilationContext.Compilation);
            if (types is null) return;


            // Register an action that accesses the immutable state and reports diagnostics.
            compilationContext.RegisterSymbolAction(
                symbolContext => { AnalyzeAggregates(symbolContext, types); }, SymbolKind.NamedType);
        });
    }


    static void AnalyzeAggregates(SymbolAnalysisContext context, Types types)
    {
        // Check if the symbol has the aggregate root base class
        var aggregateType = (INamedTypeSymbol)context.Symbol;
        if (aggregateType.BaseType?.Equals(types.AggregateRoot) != true) return;
        if (aggregateType.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax() is not ClassDeclarationSyntax aggregateSyntax) return;

        CheckAggregateRootAttributePresent(context, aggregateType, types.AggregateAttribute);


        var handledEvents = CheckOnMethods(context, aggregateType, types);
        CheckApplyInvocations(context, aggregateSyntax, handledEvents);

        // if (namedType.BaseType?.Equals(aggregateType) == true &&
        //     !namedType.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat).Equals(AllowedInternalImplementationTypeName))
        // {
        //     var diagnostic = Diagnostic.Create(Rule, namedType.Locations[0], namedType.Name, DontInheritInterfaceTypeName);
        //     context.ReportDiagnostic(diagnostic);
        // }
    }


    static HashSet<ITypeSymbol> CheckOnMethods(SymbolAnalysisContext context, INamedTypeSymbol aggregateType, Types types)
    {
        var members = aggregateType.GetMembers();
        var onMethods = members.Where(_ => _.Name.Equals("On")).OfType<IMethodSymbol>().ToArray();
        var eventTypesHandled = new HashSet<ITypeSymbol>();

        foreach (var onMethod in onMethods)
        {
            if (onMethod.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax() is not MethodDeclarationSyntax syntax) continue;

            if (!syntax.Modifiers.Any(SyntaxKind.PrivateKeyword))
            {
                context.ReportDiagnostic(Diagnostic.Create(DescriptorRules.Aggregate.MutationShouldBePrivate, syntax.GetLocation(),
                    onMethod.ToDisplayString()));
            }

            var parameters = onMethod.Parameters;
            if (parameters.Length != 1)
            {
                context.ReportDiagnostic(Diagnostic.Create(DescriptorRules.Aggregate.MutationHasIncorrectNumberOfParameters, syntax.GetLocation(),
                    onMethod.ToDisplayString()));
            }

            if (parameters.Length > 0)
            {
                var eventType = parameters[0].Type;
                eventTypesHandled.Add(eventType);

                if (!eventType.HasAttribute(types.EventAttribute))
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        DescriptorRules.Events.MissingAttribute,
                        parameters[0].DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax().GetLocation(),
                        eventType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat))
                    );
                }
            }
        }

        return eventTypesHandled;
    }


    static void CheckAggregateRootAttributePresent(SymbolAnalysisContext context, INamedTypeSymbol aggregateClass, INamedTypeSymbol attributeType)
    {
        var hasAttribute = aggregateClass.GetAttributes().Any(attribute => attribute.AttributeClass?.Equals(attributeType) == true);

        if (!hasAttribute)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                DescriptorRules.Aggregate.MissingAttribute,
                aggregateClass.Locations[0],
                aggregateClass.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)
            ));
        }
    }


    static void CheckApplyInvocations(SymbolAnalysisContext context, ClassDeclarationSyntax aggregateClassSyntax,
        ISet<ITypeSymbol> handledEventTypes)
    {
        var semanticModel = context.Compilation.GetSemanticModel(aggregateClassSyntax.SyntaxTree);
        foreach (var invocation in aggregateClassSyntax.DescendantNodes().OfType<InvocationExpressionSyntax>())
        {
            if (invocation.Expression is not IdentifierNameSyntax { Identifier.Text: "Apply" }) continue;
            if (invocation.ArgumentList.Arguments.Count != 1) continue;
            var argument = invocation.ArgumentList.Arguments[0];
            var typeInfo = semanticModel.GetTypeInfo(argument.Expression);
            if (typeInfo.Type is not { } type) continue;
            if (handledEventTypes.Contains(type)) continue; // On-handler already exists
            
            var props = new Dictionary<string, string?>
            {
                { "eventType", type.ToString() },
            }.ToImmutableDictionary();
            
            context.ReportDiagnostic(Diagnostic.Create(DescriptorRules.Aggregate.MissingMutation, invocation.GetLocation(), props, type.ToString()));
        }
    }

    static Types? GetRelevantTypes(Compilation compilation)
    {
        var aggregateBaseClass = compilation.GetTypeByMetadataName(AggregateRootBaseClass);
        if (aggregateBaseClass == null)
        {
            return default;
        }

        var aggregateRootAttribute = compilation.GetTypeByMetadataName(AggregateRootAttribute);
        if (aggregateRootAttribute == null)
        {
            return default;
        }

        var eventTypeAttribute = compilation.GetTypeByMetadataName(EventTypeAttribute);
        if (eventTypeAttribute == null)
        {
            return default;
        }

        return new Types(aggregateBaseClass, aggregateRootAttribute, eventTypeAttribute);
    }
}
