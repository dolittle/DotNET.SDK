// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.SDK.Analyzers;

static class DolittleTypes
{
    public const string AggregateRootBaseClass = "Dolittle.SDK.Aggregates.AggregateRoot";
    public const string EventTypeAttribute = "Dolittle.SDK.Events.EventTypeAttribute";
    public const string AggregateRootAttribute = "Dolittle.SDK.Aggregates.AggregateRootAttribute";
    public const string EventHandlerAttribute = "Dolittle.SDK.Events.Handling.EventHandlerAttribute";

    public const string ICommitEventsInterface = "Dolittle.SDK.Events.Store.ICommitEvents";

    public const string EventContext = "Dolittle.SDK.Events.EventContext";
    
    public const string ProjectionBaseClass = "Dolittle.SDK.Projections.ProjectionBase";
    public const string ProjectionAttribute = "Dolittle.SDK.Projections.ProjectionAttribute";
    public const string ProjectionResultType = "Dolittle.SDK.Projections.ProjectionResultType";
    public const string ProjectionContextType = "Dolittle.SDK.Projections.ProjectionContext";
}
