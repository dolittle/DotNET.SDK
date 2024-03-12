// // Copyright (c) Dolittle. All rights reserved.
// // Licensed under the MIT license. See LICENSE file in the project root for full license information.
//
// using System.Threading.Tasks;
//
// namespace Dolittle.SDK.Analyzers.CodeFixes;
// public class ProjectionAttributeMissingCodeFixProviderTests : CodeFixProviderTests<ProjectionsAnalyzer, AttributeMissingCodeFixProvider>
// {
//     //Line ending Roslyn bugs break the test: https://github.com/dotnet/roslyn/issues/62976
//     
//     [Fact]
//     public async Task FixesMissingProjectionAttributes()
//     {
//         var test = @"
// using Dolittle.SDK.Projections;
//
// class SomeProjection: ReadModel
// {
//
// }";
//         var expected = @"
// using Dolittle.SDK.Projections;
//
// [Projection(""61359cf4-3ae7-4a26-8a81-6816d3877f81"")]
// class SomeProjection: ReadModel
// {
//
// }";
//         IdentityGenerator.Override = "61359cf4-3ae7-4a26-8a81-6816d3877f81";
//
//         var diagnosticResult = Diagnostic(DescriptorRules.Projection.MissingAttribute)
//             .WithSpan(4, 7, 4, 21)
//             .WithArguments("SomeProjection");
//         await VerifyCodeFixAsync(test, expected, diagnosticResult);
//     }
//     
// }
