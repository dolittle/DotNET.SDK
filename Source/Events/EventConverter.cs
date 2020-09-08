// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.SDK.Artifacts;
using Dolittle.SDK.Protobuf;
using Dolittle.Serialization.Json;
using Contracts = Dolittle.Runtime.Events.Contracts;

namespace Dolittle.SDK.Events
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventConverter"/>.
    /// </summary>
    public class EventConverter : IEventConverter
    {
        readonly IArtifactTypeMap _artifactTypeMap;
        readonly ISerializer _serializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventConverter"/> class.
        /// </summary>
        /// <param name="artifactTypeMap"><see cref="IArtifactTypeMap"/> for mapping types and artifacts.</param>
        /// <param name="serializer"><see cref="ISerializer"/> for serialization.</param>
        public EventConverter(
            IArtifactTypeMap artifactTypeMap,
            ISerializer serializer)
        {
            _artifactTypeMap = artifactTypeMap;
            _serializer = serializer;
        }

        /// <inheritdoc/>
        public Contracts.UncommittedEvent ToProtobuf(UncommittedEvent @event)
            => new Contracts.UncommittedEvent
            {
                Artifact = ToProtobuf(@event.Event.GetType()),
                EventSourceId = @event.EventSource.ToProtobuf(),
                Public = IsPublicEvent(@event.Event),
                Content = _serializer.EventToJson(@event.Event),
            };

        /// <inheritdoc/>
        public IEnumerable<Contracts.UncommittedEvent> ToProtobuf(UncommittedEvents events)
            => events.Select(_ => ToProtobuf(_));

        /// <inheritdoc/>
        public CommittedEvent ToSDK(Contracts.CommittedEvent source)
        {
            var eventType = source.Type;
            try
            {
                var content = _serializer.JsonToEvent(eventType, source.Content);
                return new CommittedEvent(
                    source.EventLogSequenceNumber,
                    source.Occurred.ToDateTimeOffset(),
                    source.EventSourceId.To<EventSourceId>(),
                    source.ExecutionContext.ToExecutionContext(),
                    // artifact stuff, pls fix so it has toSDK()
                    source.Type,
                    content,
                    source.Public,
                    source.External,
                    source.ExternalEventLogSequenceNumber,
                    source.ExternalEventReceived.ToDateTimeOffset()
                    );
            }
            catch (Exception ex)
            {
                throw new CouldNotDeserializeEvent(
                    source.Type.Id.To<ArtifactId>(),
                    eventType,
                    source.Content,
                    source.EventLogSequenceNumber,
                    ex);
            }
        }

        /// <inheritdoc/>
        public CommittedEvents ToSDK(IEnumerable<Contracts.CommittedEvent> source)
            => new CommittedEvents(source.Select(ToSDK).ToList());

        Artifacts.Contracts.Artifact ToProtobuf(Type artifact)
        {
            var mapped = _artifactTypeMap.GetArtifactFor(artifact);
            return new Artifacts.Contracts.Artifact
            {
                Id = mapped.Id.ToProtobuf(),
                Generation = mapped.Generation,
            };
        }

        Type ToSDK(Artifacts.Contracts.Artifact artifact)
            => _artifactTypeMap.GetTypeFor(new Artifact(artifact.Id.To<ArtifactId>(), artifact.Generation));

        bool IsPublicEvent(IEvent @event)
            => typeof(IPublicEvent).IsAssignableFrom(@event.GetType());
    }
}
