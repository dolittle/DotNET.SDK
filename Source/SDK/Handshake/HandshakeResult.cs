// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Microservices;
using Environment = Dolittle.SDK.Microservices.Environment;

namespace Dolittle.SDK.Handshake;

/// <summary>
/// Represents the result of the handshake with the Dolittle Runtime.
/// </summary>
/// <param name="MicroserviceId">The <see cref="MicroserviceId"/>.</param>
/// <param name="Environment">The current <see cref="Environment"/>.</param>
public record HandshakeResult(MicroserviceId MicroserviceId, Environment Environment, Uri? OTLPEndpoint);
