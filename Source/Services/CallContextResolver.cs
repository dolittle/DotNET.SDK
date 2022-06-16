// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using Dolittle.SDK.Execution;
using Dolittle.SDK.Protobuf;
using Dolittle.Services.Contracts;
using ExecutionContext = Dolittle.SDK.Execution.ExecutionContext;

namespace Dolittle.SDK.Services;

/// <summary>
/// Represents an implementation of <see cref="IResolveCallContext" />.
/// </summary>
public class CallContextResolver : IResolveCallContext
{
    /// <inheritdoc/>
    public CallRequestContext ResolveFrom(ExecutionContext executionContext)
    {
        var context = executionContext.ToProtobuf();
        Activity.Current?.SetTraceContext(context);
        return new()
        {
            HeadId = HeadId.NotSet.Value.ToProtobuf(),
            ExecutionContext = context
        };
    }
}
