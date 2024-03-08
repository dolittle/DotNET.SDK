// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events;

namespace Dolittle.SDK.Projections.Types;

[EventType("03a8cbfb-87c9-4237-aed4-154e2e92187d")]
public record VoidProjection(string Foo);

[EventType("72b988a9-9282-4c5a-b078-9fa11606967b")]
public record VoidEventContext(string Foo);

[EventType("ff773317-52f3-4c46-b4a7-dfee267b35e6")]
public record VoidEvent(string Foo);

[Projection("c6f87322-be67-4aaf-a9f4-fdc24ac4f0fb")]
public class VoidSignaturesProjection : ReadModel
{
    public string Content { get; set; } = string.Empty;

    public void On(VoidProjection evt, ProjectionContext ctx) => Content = evt.Foo;
    public void On(VoidEventContext evt, EventContext ctx) => Content = evt.Foo;
    public void On(VoidEvent evt) => Content = evt.Foo;
}
