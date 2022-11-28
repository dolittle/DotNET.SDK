// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.CSharp.Testing.XUnit;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing.Verifiers;


namespace Dolittle.SDK.Analyzers;

public abstract class AnalyzerTest<TAnalyzer> where TAnalyzer : DiagnosticAnalyzer, new()
{
    protected Task VerifyAnalyzerAsync(string source, params DiagnosticResult[] expected)
    {
        var test = new CSharpAnalyzerTest<TAnalyzer, XUnitVerifier>
        {
            TestState =
            {
                Sources = { source },
                AdditionalReferences =
                {
                    MetadataReference.CreateFromFile(typeof(Aggregates.AggregateRootType).Assembly.Location)
                }
            }
        };
        test.TestState.ExpectedDiagnostics.AddRange(expected);

        return test.RunAsync();
    }

    protected DiagnosticResult Diagnostic()
    {
        return AnalyzerVerifier<TAnalyzer>.Diagnostic();
    }

    protected DiagnosticResult Diagnostic(DiagnosticDescriptor descriptor)
    {
        return AnalyzerVerifier<TAnalyzer>.Diagnostic(descriptor);
    }
}
