// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dolittle.SDK.Analyzers;

static class RoslynExtensions
{
    public static MemberAccessExpressionSyntax[] GetAccessesOfCurrentTime(this SyntaxNode scope,
        SyntaxNodeAnalysisContext context)
    {
        var currentTimeInvocations = scope.DescendantNodes()
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

        return currentTimeInvocations;
    }
}
