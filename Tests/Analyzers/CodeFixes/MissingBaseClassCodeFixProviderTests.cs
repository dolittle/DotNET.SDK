// // Copyright (c) Dolittle. All rights reserved.
// // Licensed under the MIT license. See LICENSE file in the project root for full license information.
//
// using System.Threading.Tasks;
//
// namespace Dolittle.SDK.Analyzers.CodeFixes;
//
// public class MissingBaseClassCodeFixProviderTests : CodeFixProviderTests<AttributeIdentityAnalyzer, MissingBaseClassCodeFixProvider>
// {
//     [Fact]
//     public async Task ShouldFixAggregateBaseClass()
//     {
//         var test = @"
// [Dolittle.SDK.Aggregates.AggregateRoot(""c6f87322-be67-4aaf-a9f4-fdc24ac4f0fb"")]
// class SomeAggregateRoot
// {
//     public string Name { get; set; }
// }";
//
//         var expected = @"using Dolittle.SDK.Aggregates;
// [Dolittle.SDK.Aggregates.AggregateRoot(""c6f87322-be67-4aaf-a9f4-fdc24ac4f0fb"")]
// class SomeAggregateRoot : AggregateRoot
// {
//     public string Name { get; set; }
// }";
//
//         var diagnosticResult = Diagnostic(DescriptorRules.MissingBaseClass)
//             .WithSpan(2, 1, 6, 2)
//             .WithArguments("SomeAggregateRoot", "Dolittle.SDK.Aggregates.AggregateRoot");
//
//
//         await VerifyCodeFixAsync(test, expected, diagnosticResult);
//     }
//
//     [Fact]
//     public async Task ShouldFixProjectionBaseClass()
//     {
//         var test = @"
// [Dolittle.SDK.Projections.Projection(""c6f87322-be67-4aaf-a9f4-fdc24ac4f0fb"")]
// class SomeProjection
// {
//     public string Name { get; set; }
// }";
//
//         var expected = @"using Dolittle.SDK.Projections;
// [Dolittle.SDK.Projections.Projection(""c6f87322-be67-4aaf-a9f4-fdc24ac4f0fb"")]
// class SomeProjection : ReadModel
// {
//     public string Name { get; set; }
// }";
//
//         var diagnosticResult = Diagnostic(DescriptorRules.MissingBaseClass)
//             .WithSpan(2, 1, 6, 2)
//             .WithArguments("SomeProjection", "Dolittle.SDK.Projections.ReadModel");
//
//         await VerifyCodeFixAsync(test, expected, diagnosticResult);
//     }
// }
