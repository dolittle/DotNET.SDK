// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace Dolittle.SDK.Analyzers;

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
    }
}
