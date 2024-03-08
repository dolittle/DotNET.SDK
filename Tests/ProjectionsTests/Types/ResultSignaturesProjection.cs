// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events;

namespace Dolittle.SDK.Projections.Types;

[EventType("a9965b2f-1f01-487d-ba64-75f95d814d88")]
public record ResultProjection(string Foo);

[EventType("af4b6cef-cef9-47db-b83c-d598ce12b112")]
public record ResultEventContext(string Foo);

[EventType("05dbd26d-134c-4a11-adcb-5808f53e8da5")]
public record ResultEvent(string Foo);

[EventType("cc4f0199-ecc1-434d-acd7-1df0e17fc487")]
public record ResultDelete;

[Projection("c6f87322-be67-4aaf-a9f4-fdc24ac4f0fb")]
public class ResultSignaturesProjection : ReadModel
{
    public string Content { get; set; } = string.Empty;

    public ProjectionResult<ResultSignaturesProjection> On(ResultProjection evt, ProjectionContext ctx)
    {
        Content = evt.Foo;
        return this;
    }

    public ProjectionResult<ResultSignaturesProjection> On(ResultEventContext evt, EventContext ctx)
    {
        Content = evt.Foo;
        return this;
    }

    public ProjectionResult<ResultSignaturesProjection> On(ResultEvent evt)
    {
        Content = evt.Foo;
        return this;
    }

    public ProjectionResult<ResultSignaturesProjection> On(ResultDelete evt) => ProjectionResult<ResultSignaturesProjection>.Delete;
}
