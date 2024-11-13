// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dolittle.SDK.Analyzers;

public abstract class DefaultAnalysisSuppressorTest<TSuppressor>
    where TSuppressor : DiagnosticSuppressor, new()
{
    /// <summary>
    /// Verify that the analyzer does not produce any diagnostics for this source
    /// </summary>
    /// <param name="source">The source to test against</param>
    /// <returns></returns>
    protected Task VerifyAnalyzerFindsNothingAsync(string source) => VerifyAnalyzerAsync(source, DiagnosticResult.EmptyDiagnosticResults);

    protected Task VerifyAnalyzerAsync(string source, params DiagnosticResult[] expected)
    {
        var test = new CSharpAnalyzerTest<TSuppressor, DefaultVerifier>
        {
            TestState =
            {
                Sources = { source },
                ReferenceAssemblies = ReferenceAssemblies.Net.Net90,
            },
            CompilerDiagnostics = CompilerDiagnostics.Suggestions
        };
        test.TestState.ExpectedDiagnostics.AddRange(expected);
        foreach (var assembly in AssembliesUnderTest.Assemblies)
        {
            test.TestState.AdditionalReferences.Add(MetadataReference.CreateFromFile(assembly.Location));
        }

        return test.RunAsync();
    }
}
