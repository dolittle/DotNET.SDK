// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;

namespace Dolittle.SDK.EventHorizon;

/// <summary>
/// Defines an interface for working with the Event Horizon.
/// </summary>
public interface IEventHorizons
{
    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> of type <see cref="SubscribeResponse"/> of all the responses returned from the Runtime.
    /// </summary>
    IObservable<SubscribeResponse> Responses { get; }

    /// <summary>
    /// Registers an Event Horizon subscription with the Runtime.
    /// </summary>
    /// <param name="subscription">The <see cref="Subscription"/> to register.</param>
    /// <returns>The <see cref="SubscribeResponse"/> from the Runtime indicating whether or not the subscription was successful.</returns>
    Task<SubscribeResponse> Subscribe(Subscription subscription);
}