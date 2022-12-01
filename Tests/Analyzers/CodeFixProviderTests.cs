// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Tenancy.Contracts;
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Embeddings;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Handling;
using Dolittle.SDK.Projections;
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
                    MetadataReference.CreateFromFile(typeof(AggregateRootAttribute).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(EventTypeAttribute).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(ProjectionAttribute).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(EventHandlerAttribute).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(EmbeddingAttribute).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(Tenant).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(IDolittleClient).Assembly.Location),
                },
                ReferenceAssemblies = ReferenceAssemblies.Net.Net60
            },
            ExpectedDiagnostics =
            {
                diagnosticResult
            },
            FixedCode = expectedResult,
        }.RunAsync(CancellationToken.None);
    }
}
