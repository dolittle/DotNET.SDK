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
/// Analyzer for <see cref="Dolittle.SDK.Projections"/>.
/// </summary>
#pragma warning restore CS1574, CS1584, CS1581, CS1580
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ProjectionsAnalyzer : DiagnosticAnalyzer
{
    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(
            // DescriptorRules.DuplicateIdentity,
            DescriptorRules.Events.MissingAttribute,
            DescriptorRules.Projection.MissingAttribute,
            DescriptorRules.Projection.MissingBaseClass,
            DescriptorRules.Projection.InvalidOnMethodParameters,
            DescriptorRules.Projection.InvalidOnMethodReturnType,
            DescriptorRules.Projection.InvalidOnMethodVisibility,
            DescriptorRules.Projection.EventTypeAlreadyHandled
        );

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeProjections, ImmutableArray.Create(SyntaxKind.ClassDeclaration));
    }


    static void AnalyzeProjections(SyntaxNodeAnalysisContext context)
    {
        // Check if the symbol has the projection root base class
        var projectionSyntax = (ClassDeclarationSyntax)context.Node;
        // Check if the symbol has the projection root base class
        var projectionSymbol = context.SemanticModel.GetDeclaredSymbol(projectionSyntax);
        if (projectionSymbol?.IsProjection() != true) return;

        CheckProjectionAttributePresent(context, projectionSymbol);
        CheckOnMethods(context, projectionSymbol);
    }


    static void CheckOnMethods(SyntaxNodeAnalysisContext context, INamedTypeSymbol projectionType)
    {
        var members = projectionType.GetMembers();
        var onMethods = members.Where(_ => _.Name.Equals("On")).OfType<IMethodSymbol>().ToArray();
        var eventTypesHandled = new HashSet<ITypeSymbol>(SymbolEqualityComparer.Default);

        foreach (var onMethod in onMethods)
        {
            if (onMethod.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax() is not MethodDeclarationSyntax syntax) continue;

            if (!syntax.Modifiers.Any(SyntaxKind.PublicKeyword))
            {
                context.ReportDiagnostic(Diagnostic.Create(DescriptorRules.Projection.InvalidOnMethodVisibility, syntax.GetLocation(),
                    onMethod.ToDisplayString()));
            }

            var parameters = onMethod.Parameters;
            if (parameters.Length is not 1 and not 2)
            {
                context.ReportDiagnostic(Diagnostic.Create(DescriptorRules.Projection.InvalidOnMethodParameters, syntax.GetLocation(),
                    onMethod.ToDisplayString()));
            }

            if (parameters.Length > 0)
            {
                var eventType = parameters[0].Type;
                if(!eventTypesHandled.Add(eventType))
                {
                    context.ReportDiagnostic(Diagnostic.Create(DescriptorRules.Projection.EventTypeAlreadyHandled, syntax.GetLocation(),
                        eventType.ToDisplayString()));
                }

                if (!eventType.HasEventTypeAttribute())
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        DescriptorRules.Events.MissingAttribute,
                        parameters[0].DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax().GetLocation(),
                        eventType.ToTargetClassAndAttributeProps(DolittleTypes.EventTypeAttribute),
                        eventType.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat))
                    );
                }
            }

            if (parameters.Length > 1)
            {
                var secondParameterTypeSymbol = parameters[1].Type;
                var contextType = secondParameterTypeSymbol.ToDisplayString();
                if (!contextType.Equals(DolittleTypes.ProjectionContextType, StringComparison.Ordinal)
                    && !contextType.Equals(DolittleTypes.EventContext, StringComparison.Ordinal)
                    )
                {
                    var loc = parameters[1].DeclaringSyntaxReferences.First().GetSyntax().GetLocation();
                    context.ReportDiagnostic(Diagnostic.Create(DescriptorRules.Projection.InvalidOnMethodParameters, loc,
                        onMethod.ToDisplayString()));
                }
                
            }
            
            CheckOnReturnType(context, projectionType, onMethod, syntax);
        }
    }

    static void CheckOnReturnType(SyntaxNodeAnalysisContext context, INamedTypeSymbol projectionType, IMethodSymbol onMethod,
        MethodDeclarationSyntax syntax)
    {
        // Check for valid return type. Valid types are void, ProjectionResultType and ProjectionResult<>
        var returnType = onMethod.ReturnType;
        if(returnType.SpecialType == SpecialType.System_Void)
        {
            return; // void is valid
        }
        
        if (returnType is not INamedTypeSymbol namedReturnType)
        {
            context.ReportDiagnostic(Diagnostic.Create(DescriptorRules.Projection.InvalidOnMethodReturnType, syntax.GetLocation(),
                onMethod.ToDisplayString()));
            return;
        }

        if (namedReturnType.IsGenericType)
        {
            var genericType = namedReturnType.TypeArguments[0];
            if (!genericType.Equals(projectionType))
            {
                context.ReportDiagnostic(Diagnostic.Create(DescriptorRules.Projection.InvalidOnMethodReturnType, syntax.GetLocation(),
                    onMethod.ToDisplayString()));
            }
        }
        else
        {
            if (namedReturnType.ToDisplayString() != DolittleTypes.ProjectionResultType)
            {
                context.ReportDiagnostic(Diagnostic.Create(DescriptorRules.Projection.InvalidOnMethodReturnType, syntax.GetLocation(),
                    onMethod.ToDisplayString()));
            }
        }
    }

    static void CheckProjectionAttributePresent(SyntaxNodeAnalysisContext context, INamedTypeSymbol projectionClass)
    {
        var hasAttribute = projectionClass.GetAttributes()
            .Any(attribute => attribute.AttributeClass?.ToDisplayString().Equals(DolittleTypes.ProjectionAttribute, StringComparison.Ordinal) == true);

        if (!hasAttribute)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                DescriptorRules.Projection.MissingAttribute,
                projectionClass.Locations[0],
                projectionClass.ToTargetClassAndAttributeProps(DolittleTypes.ProjectionAttribute),
                projectionClass.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)
            ));
        }
    }
}
