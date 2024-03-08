// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events;

namespace Dolittle.SDK.Projections.Types;

[EventType("055319f1-6af8-48a0-b190-323e21ba6cde")]
public class DeleteEvent
{
    public string Reason { get; init; }
}
