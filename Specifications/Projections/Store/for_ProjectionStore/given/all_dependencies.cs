// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Projections.Contracts;
using Dolittle.SDK.Common;
using Dolittle.SDK.Projections.Store.Converters;
using Dolittle.SDK.Protobuf;
using Dolittle.SDK.Security;
using Dolittle.SDK.Services;
using Machine.Specifications;
using Microsoft.Extensions.Logging;
using Moq;
using ExecutionContext = Dolittle.SDK.Execution.ExecutionContext;
using GetOneRequest = Dolittle.Runtime.Projections.Contracts.GetOneRequest;
using GetOneResponse = Dolittle.Runtime.Projections.Contracts.GetOneResponse;
using Version = Dolittle.SDK.Microservices.Version;

namespace Dolittle.SDK.Projections.Store.for_ProjectionStore.given;

public class all_dependencies
{
    protected static Mock<IPerformMethodCalls> method_caller;
    protected static ExecutionContext an_execution_context;
    protected static ProjectionReadModelTypes read_model_types;
    protected static ProjectionStore projection_store;
    Establish context = () =>
    {
        method_caller = new Mock<IPerformMethodCalls>();
        an_execution_context = new ExecutionContext(
            "65AB2D32-BA77-466E-A88A-355DEA846858",
            "AFDE8AB2-CC93-4422-9D82-0C2A343AE8D2",
            Version.NotSet,
            "Some ENV",
            "A36492C2-786A-4210-941E-BFAF1A682607",
            Claims.Empty,
            CultureInfo.InvariantCulture);
        read_model_types = new ProjectionReadModelTypes();
        projection_store = new ProjectionStore(
            method_caller.Object,
            new ProjectionStoreRequestCreator(new CallContextResolver()),
            an_execution_context,
            read_model_types,
            new ProjectionsToSDKConverter(), Mock.Of<ILogger>());
    };

    protected static void with_projection_types(params Type[] projection_types)
    {
        foreach (var type in projection_types)
        {
            if (type.TryGetDecorator<ProjectionAttribute>(out var decorator))
            {
                read_model_types.Add(new ScopedProjectionId(decorator.Identifier, decorator.Scope), type);
            }
        }
    }

    protected static void get_one_returns(GetOneResponse response)
        => method_caller
            .Setup(_ => _.Call(
            Moq.It.IsAny<ProjectionsGetOne>(),
            Moq.It.IsAny<GetOneRequest>(),
            Moq.It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(response));
    
    protected static void get_one_returns<TProjection>(Key key, TProjection state, ProjectionCurrentStateType type)
        => method_caller
            .Setup(_ => _.Call(
            Moq.It.IsAny<ProjectionsGetOne>(),
            Moq.It.IsAny<GetOneRequest>(),
            Moq.It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(new GetOneResponse()
            {
                State = new ProjectionCurrentState()
                {
                    Key = key,
                    State = JsonSerializer.Serialize(state),
                    Type = type
                }
            }));

    protected static void get_all_returns(GetAllResponse response)
        => method_caller
            .Setup(_ => _.Call(
            Moq.It.IsAny<ProjectionsGetAll>(),
            Moq.It.IsAny<GetAllRequest>(),
            Moq.It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(response));

    protected static GetOneRequest request_like(Key key, ScopedProjectionId id)
        => Moq.It.Is<GetOneRequest>(_ =>
            _.Key == key.Value
            && _.ProjectionId.Equals(id.Identifier.ToProtobuf())
            && _.ScopeId.Equals(id.ScopeId.ToProtobuf())
            && _.CallContext.ExecutionContext.ToExecutionContext().Equals(an_execution_context));
}
