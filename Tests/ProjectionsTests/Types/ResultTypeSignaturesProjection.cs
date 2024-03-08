// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events;

namespace Dolittle.SDK.Projections.Types;

[EventType("a9965b2f-1f01-487d-ba64-75f95d814d88")]
public record ResultTypeProjection(string Foo);

[EventType("af4b6cef-cef9-47db-b83c-d598ce12b112")]
public record ResultTypeEventContext(string Foo);

[EventType("05dbd26d-134c-4a11-adcb-5808f53e8da5")]
public record ResultTypeEvent(string Foo);

[EventType("cc4f0199-ecc1-434d-acd7-1df0e17fc487")]
public record ResultTypeDelete;

[Projection("c6f87322-be67-4aaf-a9f4-fdc24ac4f0fb")]
public class ResultTypeSignaturesProjection : ReadModel
{
    public string Content { get; set; } = string.Empty;

    public ProjectionResultType On(ResultTypeProjection evt, ProjectionContext ctx)
    {
        Content = evt.Foo;
        return ProjectionResultType.Replace;
    }

    public ProjectionResultType On(ResultTypeEventContext evt, EventContext ctx)
    {
        Content = evt.Foo;
        return ProjectionResultType.Replace;
    }

    public ProjectionResultType On(ResultTypeEvent evt)
    {
        Content = evt.Foo;
        return ProjectionResultType.Replace;
    }

    public ProjectionResultType On(ResultTypeDelete evt) => ProjectionResultType.Delete;
}
