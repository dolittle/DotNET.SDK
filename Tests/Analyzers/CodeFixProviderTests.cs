// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.CSharp.Testing.XUnit;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Roslynator.Testing.CSharp.Xunit;

namespace Dolittle.SDK.Analyzers.CodeFixes;

public abstract class CodeFixProviderTests<TAnalyzer, TCodeFix> : AnalyzerTest<TAnalyzer>
    where TCodeFix : CodeFixProvider, new()
    where TAnalyzer : DiagnosticAnalyzer, new()
{
    protected Task VerifyCodeFixAsync(string source, string expectedResult, DiagnosticResult diagnosticResult)
    {
        return new CSharpCodeFixTest<TAnalyzer, TCodeFix, XUnitVerifier>
        {
            TestCode = source,
            TestState =
            {
                AdditionalReferences =
                {
                    MetadataReference.CreateFromFile(typeof(Aggregates.AggregateRootType).Assembly.Location)
                }
            },
            ExpectedDiagnostics =
            {
                diagnosticResult
            },
            FixedCode = expectedResult,
        }.RunAsync(CancellationToken.None);
        // var verifier = new CSharpCodeFixVerifier<TAnalyzer, TCodeFix, XUnitVerifier>();
        // verifier.
        //
        // return CodeFixVerifier<TAnalyzer, TCodeFix>.VerifyCodeFixAsync(source, diagnosticResult, expectedResult);

        // var expected = new ExpectedTestState(expectedResult);
        // var data = new CompilerDiagnosticFixTestData(DiagnosticId, source);
        //
        //
        //
        //
        // return VerifyFixAsync(
        //     data,
        //     expected,
        //     Options.WithMetadataReferences(
        //         Options.MetadataReferences.AddRange(
        //             MetadataReference.CreateFromFile(typeof(Aggregates.AggregateRootType).Assembly.Location),
        //             MetadataReference.CreateFromFile(typeof(AnnotationIdentityCodeFixProvider).Assembly.Location)))
        //         .,
        //     );
    }
}
