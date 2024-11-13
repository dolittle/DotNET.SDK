// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dolittle.SDK.Analyzers;

public abstract class CodeFixProviderTests<TAnalyzer, TCodeFix> : AnalyzerTest<TAnalyzer>
    where TCodeFix : CodeFixProvider, new()
    where TAnalyzer : DiagnosticAnalyzer, new()
{
    protected Task VerifyCodeFixAsync(string source, string expectedResult, DiagnosticResult diagnosticResult)
    {
        source = ToLfLineEndings(source);
        expectedResult = ToLfLineEndings(expectedResult);
        
        var test = new CSharpCodeFixTest<TAnalyzer, TCodeFix, DefaultVerifier>
        {
            TestCode = source,
            TestState =
            {
                ReferenceAssemblies = ReferenceAssemblies.Net.Net60
            },
            ExpectedDiagnostics =
            {
                diagnosticResult
            },
            FixedCode = expectedResult,
        };
        foreach (var assembly in AssembliesUnderTest.Assemblies)
        {
            test.TestState.AdditionalReferences.Add(MetadataReference.CreateFromFile(assembly.Location));
        }
        
        return test.RunAsync(CancellationToken.None);
    }

    string ToLfLineEndings(string source)
    {
        return source.Replace("\r\n", "\n");
    }
}
