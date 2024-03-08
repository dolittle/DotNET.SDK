// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events;

namespace Dolittle.SDK.Projections.Types;

[EventType("83225425-c4b8-4ef7-9638-d23530831752")]
public class SomeEvent
{
    public string Thing { get; set; }
}
