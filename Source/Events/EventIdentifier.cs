// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using Dolittle.SDK.Concepts;
using Dolittle.SDK.Microservices;
using Dolittle.SDK.Tenancy;

namespace Dolittle.SDK.Events
{
    /// <summary>
    /// Represents an unique identifier for an event that has been committed to the <see cref="IEventStore"/>.
    /// </summary>
    /// <remarks>
    /// The identifier is constructed from the <see cref="MicroserviceId"/> and <see cref="TenantId"/> that committed the event, and the <see cref="EventLogSequenceNumber"/> it got in the event log.
    /// </remarks>
    public class EventIdentifier : ConceptAs<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventIdentifier"/> class.
        /// </summary>
        /// <param name="microserviceId">The <see cref="MicroserviceId"/> that committed the event.</param>
        /// <param name="tenantId">The <see cref="TenantId"/> that committed the event.</param>
        /// <param name="sequenceNumber">The <see cref="EventLogSequenceNumber"/> of the event that was committed.</param>
        public EventIdentifier(MicroserviceId microserviceId, TenantId tenantId, EventLogSequenceNumber sequenceNumber)
            => Value = Compose(microserviceId, tenantId, sequenceNumber);

        /// <summary>
        /// Initializes a new instance of the <see cref="EventIdentifier"/> class.
        /// </summary>
        /// <param name="eventIdentifier">The event identifier <see cref="string"/>.</param>
        public EventIdentifier(string eventIdentifier)
        {
            ThrowIfEventIdentifierIsInvalid(eventIdentifier);
            Value = eventIdentifier;
        }

        /// <summary>
        /// Composes an event identifier <see cref="string"/> from its components.
        /// </summary>
        /// <param name="microservice">The <see cref="MicroserviceId"/> that committed the event.</param>
        /// <param name="tenant">The <see cref="TenantId"/> that committed the event.</param>
        /// <param name="sequenceNumber">The <see cref="EventLogSequenceNumber"/> of the event that was committed.</param>
        /// <returns>An event identifier <see cref="string"/>.</returns>
        /// <remarks>
        /// The event identifier string is the Base64 encoding of the concatenated bytes of the microservice ++ tenant ++ event log sequence number (40 bytes total).
        /// When converting the values to bytes, they are in the order that they are transmitted over gRPC as defined by the contracts - i.e. little endian.
        /// </remarks>
        public static string Compose(MicroserviceId microservice, TenantId tenant, EventLogSequenceNumber sequenceNumber)
        {
            var microserviceBytes = microservice.Value.ToByteArray();
            var tenantBytes = tenant.Value.ToByteArray();
            var sequenceNumberBytes = BitConverter.GetBytes(sequenceNumber.Value);

            if (!BitConverter.IsLittleEndian)
                Array.Reverse(sequenceNumberBytes);

            using var stream = new MemoryStream(40);

            stream.Write(microserviceBytes);
            stream.Write(tenantBytes);
            stream.Write(sequenceNumberBytes);

            return Convert.ToBase64String(stream.ToArray());
        }

        /// <summary>
        /// Tries to decompose an event identifier <see cref="string"/> to its components.
        /// </summary>
        /// <param name="eventIdentifier">The event identifier <see cref="string"/>.</param>
        /// <param name="microservice">When this method returns, the <see cref="MicroserviceId"/> that committed the event.</param>
        /// <param name="tenant">When this method returns, the <see cref="TenantId"/> that committed the event.</param>
        /// <param name="sequenceNumber">When this method returns, the <see cref="EventLogSequenceNumber"/> of the event that was committed.</param>
        /// <returns><c>true</c> if the deomposition was successful; otherwise, <c>false</c>.</returns>
        public static bool TryDecompose(string eventIdentifier, out MicroserviceId microservice, out TenantId tenant, out EventLogSequenceNumber sequenceNumber)
        {
            var decoded = new Span<byte>(new byte[40]);
            if (Convert.TryFromBase64String(eventIdentifier, decoded, out var read) && read == 40)
            {
                var microserviceBytes = decoded.Slice(0, 16);
                var tenantBytes = decoded.Slice(16, 16);
                var sequenceNumberBytes = decoded.Slice(32, 8);

                if (!BitConverter.IsLittleEndian)
                    sequenceNumberBytes.Reverse();

                microservice = new Guid(microserviceBytes);
                tenant = new Guid(tenantBytes);
                sequenceNumber = BitConverter.ToUInt64(sequenceNumberBytes);

                return true;
            }

            microservice = null;
            tenant = null;
            sequenceNumber = 0;
            return false;
        }

        /// <summary>
        /// Decomposes the <see cref="EventIdentifier"/> to its components.
        /// </summary>
        /// <param name="microservice">When this method returns, the <see cref="MicroserviceId"/> that committed the event.</param>
        /// <param name="tenant">When this method returns, the <see cref="TenantId"/> that committed the event.</param>
        /// <param name="sequenceNumber">When this method returns, the <see cref="EventLogSequenceNumber"/> of the event that was committed.</param>
        public void Decompose(out MicroserviceId microservice, out TenantId tenant, out EventLogSequenceNumber sequenceNumber)
            => TryDecompose(Value, out microservice, out tenant, out sequenceNumber);

        void ThrowIfEventIdentifierIsInvalid(string eventIdentifier)
        {
            if (!TryDecompose(eventIdentifier, out _, out _, out _))
            {
                throw new EventIdentifierStringIsInvalid(eventIdentifier);
            }
        }
    }
}