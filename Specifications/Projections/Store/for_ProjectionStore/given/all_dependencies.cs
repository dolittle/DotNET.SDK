// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Projections.Contracts;
using Dolittle.SDK.Common;
using Dolittle.SDK.Common.Model;
using Dolittle.SDK.Events;
using Dolittle.SDK.Projections.Store.Converters;
using Dolittle.SDK.Protobuf;
using Dolittle.SDK.Security;
using Dolittle.SDK.Services;
using Grpc.Core;
using Machine.Specifications;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using ExecutionContext = Dolittle.SDK.Execution.ExecutionContext;
using GetOneRequest = Dolittle.Runtime.Projections.Contracts.GetOneRequest;
using GetOneResponse = Dolittle.Runtime.Projections.Contracts.GetOneResponse;
using JsonSerializer = System.Text.Json.JsonSerializer;
using Status = Grpc.Core.Status;
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
            CultureInfo.InvariantCulture, null);
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
            if (type.TryGetIdentifier<ProjectionModelId>(out var identifier))
            {
                read_model_types.Add(identifier, type);
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

    protected static void get_all_returns(ServerStreamingEnumerable<GetAllResponse> enumerable)
        => method_caller
            .Setup(_ => _.Call(
            Moq.It.IsAny<ProjectionsGetAllInBatches>(),
            Moq.It.IsAny<GetAllRequest>(),
            Moq.It.IsAny<CancellationToken>()))
            .Returns(enumerable);
    

    protected static GetOneRequest request_like(Key key, ProjectionModelId id)
        => Moq.It.Is<GetOneRequest>(_ =>
            _.Key == key.Value
            && _.ProjectionId.Equals(id.Id.ToProtobuf())
            && _.ScopeId.Equals(id.Scope.ToProtobuf())
            && _.CallContext.ExecutionContext.ToExecutionContext().Equals(an_execution_context));
    
    protected static GetOneRequest request_like(Key key, ProjectionId id, ScopeId scope)
        => Moq.It.Is<GetOneRequest>(_ =>
            _.Key == key.Value
            && _.ProjectionId.Equals(id.ToProtobuf())
            && _.ScopeId.Equals(scope.ToProtobuf())
            && _.CallContext.ExecutionContext.ToExecutionContext().Equals(an_execution_context));

    protected static ServerStreamingEnumerable<GetAllResponse> create_enumerable(params ProjectionCurrentState[][] batches)
        => create_enumerable(batches.Select(batch =>
        {
            var response = new GetAllResponse();
            response.States.AddRange(batch);
            return response;
        }));

    protected static ServerStreamingEnumerable<GetAllResponse> create_enumerable(params GetAllResponse[] responses)
        => create_enumerable(responses.AsEnumerable());
    
    protected static ServerStreamingEnumerable<GetAllResponse> create_enumerable(IEnumerable<GetAllResponse> responses)
        => new(new AsyncServerStreamingCall<GetAllResponse>(
            new FakeAsyncStreamReader<GetAllResponse>(responses),
            Task.FromResult(Metadata.Empty),
            () => Status.DefaultSuccess,
            () => Metadata.Empty,
            () => {}));
    
    protected static ProjectionCurrentState create_protobuf_projection_current_state<TReadModel>(TReadModel currentState, Key key, CurrentStateType type)
        where TReadModel : class, new()
        => new()
        {
            State = JsonConvert.SerializeObject(currentState, Formatting.None),
            Key = key,
            Type = get_current_state_type(type)
        };

    protected static ProjectionCurrentState create_protobuf_projection_current_state<TReadModel>(CurrentState<TReadModel> currentState, CurrentStateType type)
        where TReadModel : class, new()
        => create_protobuf_projection_current_state(currentState.State, currentState.Key, type);

    static ProjectionCurrentStateType get_current_state_type(CurrentStateType type)
        => type switch
        {
            CurrentStateType.Persisted => ProjectionCurrentStateType.Persisted,
            CurrentStateType.CreatedFromInitialState => ProjectionCurrentStateType.CreatedFromInitialState,
            _ => (ProjectionCurrentStateType) type
        };
}
