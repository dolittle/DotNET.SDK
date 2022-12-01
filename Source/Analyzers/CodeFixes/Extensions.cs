// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Dolittle.SDK.Analyzers.CodeFixes;

static class Extensions
{
    static readonly LineEndingsRewriter _lineEndingsRewriter = new();
    public static ClassDeclarationSyntax AddMemberFormatted(this ClassDeclarationSyntax classDeclaration, MemberDeclarationSyntax member)
    {
        var existingMember = classDeclaration.Members.FirstOrDefault();
        if (existingMember is null)
        {
            
        }
        var members = classDeclaration.Members.Add(member);
        return classDeclaration.AddMembers(member.WithTriviaFrom(existingMember));
    }

    public static T WithLfLineEndings<T>(this T replacedNode) where T : SyntaxNode => (T)_lineEndingsRewriter.Visit(replacedNode);
}
