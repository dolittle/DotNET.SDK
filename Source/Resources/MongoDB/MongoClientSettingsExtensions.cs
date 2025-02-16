// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using MongoDB.Driver.Core.Configuration;
using MongoDB.Driver.Core.Extensions.DiagnosticSources;

namespace Dolittle.SDK.Resources.MongoDB;

/// <summary>
/// Used to configure Mongo Client Settings
/// </summary>
static class MongoClientSettingsExtensions
{
    /// <summary>
    /// Initializes <see cref="DiagnosticsActivityEventSubscriber"/>, which creates traces based on MongoDB events, and adds it as subscriber to <see cref="ClusterBuilder"/>.
    /// </summary>
    /// <param name="builder">The MongoDB ClusterBuilder.</param>
    public static void AddTelemetry(this ClusterBuilder builder)
    {
        HashSet<string> commandsToFilter = new(
            ["ping", "isMaster", "buildInfo", "saslStart", "saslContinue"],
            StringComparer.OrdinalIgnoreCase);
        builder.Subscribe(
            new DiagnosticsActivityEventSubscriber(
                new InstrumentationOptions
                {
                    ShouldStartActivity = commandStartedEvent => !commandsToFilter.Contains(
                        commandStartedEvent.CommandName)
                }));
    }
}
