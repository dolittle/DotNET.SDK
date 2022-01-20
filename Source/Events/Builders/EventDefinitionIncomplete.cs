// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Events.Builders;

/// <summary>
/// Exception that gets thrown when trying to build an event that is not completely defined.
/// </summary>
public class EventDefinitionIncomplete : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EventDefinitionIncomplete"/> class.
    /// </summary>
    /// <param name="missingInformation">The information missing to complete the subscription definition.</param>
    /// <param name="correctingAction">Required action to complete the subscription definition.</param>
    public EventDefinitionIncomplete(string missingInformation, string correctingAction)
        : base($"Event definition is missing {missingInformation}. {correctingAction} before calling Build()")
    {
    }
}