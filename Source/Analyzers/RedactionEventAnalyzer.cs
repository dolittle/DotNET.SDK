// // Copyright (c) Dolittle. All rights reserved.
// // Licensed under the MIT license. See LICENSE file in the project root for full license information.
//
// using System.Collections.Immutable;
// using System.Linq;
// using Microsoft.CodeAnalysis;
// using Microsoft.CodeAnalysis.CSharp;
// using Microsoft.CodeAnalysis.CSharp.Syntax;
// using Microsoft.CodeAnalysis.Diagnostics;
//
// namespace Dolittle.SDK.Analyzers;
//
// /// <summary>
// /// Analyzer for <see cref="DescriptorRules.Events.Redaction.RedactablePersonalDataAttribute"/>.
// /// </summary>
// [DiagnosticAnalyzer(LanguageNames.CSharp)]
// public class RedactionEventAnalyzer : DiagnosticAnalyzer
// {
//     static readonly DiagnosticDescriptor _incorrectPrefixRule = DescriptorRules.IncorrectRedactedEventTypePrefix;
//
//
//     /// <inheritdoc />
//     public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
//         [_incorrectPrefixRule];
//
//     /// <inheritdoc />
//     public override void Initialize(AnalysisContext context)
//     {
//         context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze |
//                                                GeneratedCodeAnalysisFlags.ReportDiagnostics);
//         context.EnableConcurrentExecution();
//         
//         context.RegisterSyntaxNodeAction(AnalyzeProperty, SyntaxKind.PropertyDeclaration);
//         context.RegisterSyntaxNodeAction(AnalyzeRecordDeclaration, SyntaxKind.RecordDeclaration);
//     }
//
//     private static void AnalyzeProperty(SyntaxNodeAnalysisContext context)
//     {
//         var propertyDeclaration = (PropertyDeclarationSyntax)context.Node;
//         var propertySymbol = context.SemanticModel.GetDeclaredSymbol(propertyDeclaration);
//
//         if (propertySymbol == null) return;
//
//         var personalDataAttribute = propertySymbol.GetAttributes()
//             .FirstOrDefault(attr => attr.AttributeClass?.Name == "RedactablePersonalDataAttribute");
//
//         if (personalDataAttribute == null) return;
//
//         if (personalDataAttribute.AttributeClass is { IsGenericType: true } namedTypeSymbol)
//         {
//             // If the RedactablePersonalDataAttribute is generic, the type parameter must match the property type
//             var typeArgument = namedTypeSymbol.TypeArguments.First();
//             if (!typeArgument.Equals(propertySymbol.Type, SymbolEqualityComparer.Default))
//             {
//                 context.ReportDiagnostic(Diagnostic.Create(_incorrectPrefixRule, personalDataAttribute.ApplicationSyntaxReference?.GetSyntax().GetLocation() ?? propertyDeclaration.GetLocation(),
//                     typeArgument.Name, propertySymbol.Type.Name));
//             }
//
//         }
//         else
//         {
//             // If it the non
//             if (propertySymbol.Type.NullableAnnotation != NullableAnnotation.Annotated)
//             {
//                 context.ReportDiagnostic(Diagnostic.Create(_nonNullableRule, propertyDeclaration.GetLocation(),
//                     propertySymbol.Name));
//             }
//         }
//     }
//
//     private static void AnalyzeRecordDeclaration(SyntaxNodeAnalysisContext context)
//     {
//         var recordDeclaration = (RecordDeclarationSyntax)context.Node;
//
//         // Check if it's a record with a primary constructor
//         if (recordDeclaration.ParameterList == null) return;
//
//         foreach (var parameter in recordDeclaration.ParameterList.Parameters)
//         {
//             var isPersonalDataAnnotated = parameter.AttributeLists
//                 .SelectMany(list => list.Attributes)
//                 .Any(attr => attr.Name.ToString().StartsWith("RedactablePersonalData"));
//             if (!isPersonalDataAnnotated)
//             {
//                 continue;
//             }
//
//             var parameterSymbol = context.SemanticModel.GetDeclaredSymbol(parameter);
//             if (!IsNullable(parameterSymbol))
//             {
//                 context.ReportDiagnostic(Diagnostic.Create(_nonNullableRule, parameter.GetLocation(),
//                     parameterSymbol.Name));
//             }
//         }
//     }
//
//     static bool IsNullable(IParameterSymbol? parameterSymbol)
//     {
//         return parameterSymbol?.Type.NullableAnnotation == NullableAnnotation.Annotated;
//     }
// }
