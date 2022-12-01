// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Dolittle.SDK.Analyzers;

public class LineEndingsRewriter : CSharpSyntaxRewriter
{
    public override SyntaxTrivia VisitTrivia(SyntaxTrivia trivia)
    {
        return !trivia.IsKind(SyntaxKind.EndOfLineTrivia) ? trivia : SyntaxFactory.ElasticLineFeed;
    }
}
