// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Events
{
    /// <summary>
    /// Exception that gets thrown when an <see cref="IEvent"/> couldn't be deserialized.
    /// </summary>s
    public class CouldNotDeserializeEventFromScope : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CouldNotDeserializeEventFromScope"/> class.
        /// </summary>
        /// <param name="scope">The <see cref="ScopeId"/> of where the event is.</param>
        /// <param name="ex">The catched exception which made deserialization impossible in the first place.</param>
        public CouldNotDeserializeEventFromScope(
            ScopeId scope,
            Exception ex)
            : base($"Couldn't deserialize event in scope '{scope}'. Exception occurred during deserialization:{Environment.NewLine}{ex}")
        {
        }
    }
}