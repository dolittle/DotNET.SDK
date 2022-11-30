// // Copyright (c) Dolittle. All rights reserved.
// // Licensed under the MIT license. See LICENSE file in the project root for full license information.
//
// using System.Threading.Tasks;
//
// namespace Dolittle.SDK.Analyzers.CodeFixes;
//
// public class AggregateAttributeMissingCodeFixProviderTests : CodeFixProviderTests<AggregateAnalyzer, AttributeMissingCodeFixProvider>
// {
//     [Fact]
//     public async Task FixesMissingAggregateAttributes()
//     {
//         var test = @"
// using Dolittle.SDK.Aggregates;
//
// class SomeAggregate: AggregateRoot
// {
//
// }";
//
//         var expected = @"
// using Dolittle.SDK.Aggregates;
//
// [AggregateRoot(""61359cf4-3ae7-4a26-8a81-6816d3877f81"")]
// class SomeAggregate: AggregateRoot
// {
//
// }";
//         IdentityGenerator.Override = "61359cf4-3ae7-4a26-8a81-6816d3877f81";
//
//         var diagnosticResult = Diagnostic(DescriptorRules.Aggregate.MissingAttribute)
//             .WithSpan(4, 7, 4, 20)
//             .WithArguments("SomeAggregate");
//         await VerifyCodeFixAsync(test, expected, diagnosticResult);
//     }
//     
// }
