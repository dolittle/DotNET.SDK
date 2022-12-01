// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Embeddings;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Handling;
using Dolittle.SDK.Projections;
using Dolittle.SDK.Tenancy;
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
                AdditionalReferences =
                {
                    MetadataReference.CreateFromFile(typeof(AggregateRootAttribute).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(EventTypeAttribute).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(ProjectionAttribute).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(EventHandlerAttribute).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(EmbeddingAttribute).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(Tenant).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(IDolittleClient).Assembly.Location),
                },
                ReferenceAssemblies = ReferenceAssemblies.Net.Net60,
            }
        };
        test.TestState.ExpectedDiagnostics.AddRange(expected);

        return test.RunAsync();
    }

    protected DiagnosticResult Diagnostic()
    {
        return AnalyzerVerifier<TAnalyzer>.Diagnostic();
    }

    protected DiagnosticResult Diagnostic(Microsoft.CodeAnalysis.DiagnosticDescriptor descriptor)
    {
        return AnalyzerVerifier<TAnalyzer>.Diagnostic(descriptor);
    }
}
