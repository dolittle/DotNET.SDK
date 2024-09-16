﻿// Copyright (c) Dolittle. All rights reserved.
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
    /// <summary>
    /// Verify that the analyzer does not produce any diagnostics for this source
    /// </summary>
    /// <param name="source">The source to test against</param>
    /// <returns></returns>
    protected Task VerifyAnalyzerFindsNothingAsync(string source) => VerifyAnalyzerAsync(source, DiagnosticResult.EmptyDiagnosticResults);

    protected Task VerifyAnalyzerAsync(string source, params DiagnosticResult[] expected)
    {
        var test = new CSharpAnalyzerTest<TAnalyzer, XUnitVerifier>
        {
            TestState =
            {
                Sources = { source },
                ReferenceAssemblies = ReferenceAssemblies.Net.Net60,
            },
        };
        test.TestState.ExpectedDiagnostics.AddRange(expected);
        foreach (var assembly in AssembliesUnderTest.Assemblies)
        {
            test.TestState.AdditionalReferences.Add(MetadataReference.CreateFromFile(assembly.Location));
        }

        return test.RunAsync();
    }

    protected DiagnosticResult Diagnostic()
    {
        return CSharpAnalyzerVerifier<TAnalyzer, DefaultVerifier>.Diagnostic();
    }

    protected DiagnosticResult Diagnostic(DiagnosticDescriptor descriptor)
    {
        return CSharpAnalyzerVerifier<TAnalyzer, DefaultVerifier>.Diagnostic(descriptor);
    }
}
