// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dolittle.SDK.Analyzers.Suppressors;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class AggregateMutationNotUsedSuppressor : DiagnosticSuppressor
{
    public override ImmutableArray<SuppressionDescriptor> SupportedSuppressions { get; } = ImmutableArray.Create(SuppressionDescriptors.IDE0051Unused);

    public override void ReportSuppressions(SuppressionAnalysisContext context)
    {
        foreach (var diagnostic in context.ReportedDiagnostics)
        {
            if (diagnostic.Id != "IDE0051" || diagnostic.Location.SourceTree?.FilePath == null) continue;
            context.ReportSuppression(Suppression.Create(SuppressionDescriptors.IDE0051Unused, diagnostic));

            // var syntaxTree = diagnostic.Location.SourceTree;
            // var semanticModel = context.GetSemanticModel(syntaxTree);
            // var syntaxNode = syntaxTree.GetRoot().FindNode(diagnostic.Location.SourceSpan);
            // if (syntaxNode is not MethodDeclarationSyntax methodDeclaration) continue;
            // if (!methodDeclaration.Identifier.Text.Equals("On", StringComparison.Ordinal)) continue;
            // if (semanticModel.GetDeclaredSymbol(syntaxNode) is not IMethodSymbol { ContainingType: { } namedTypeSymbol }) continue;
            //
            // if (!namedTypeSymbol.IsAggregateRoot()) continue;
            //
            // context.ReportSuppression(Suppression.Create(SuppressionDescriptors.IDE0051Unused, diagnostic));
        }
    }
}
