// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Concepts;

namespace Dolittle.SDK.Events.Processing.Internal
{
    /// <summary>
    /// Defines a system that handles the registration of event processor.
    /// </summary>
    /// <typeparam name="TIdentifier">The <see cref="Type" /> of the <see cref="ConceptAs{T}" /> <see cref="Guid" />.>.</typeparam>
    /// <typeparam name="TRegisterArguments">The <see cref="Type" /> of the registration arguments.</typeparam>
    /// <typeparam name="TRegisterResponse">The <see cref="Type" /> of the registration response.</typeparam>
    public interface IEventProcessorRegistration<TIdentifier, TRegisterArguments, TRegisterResponse> : IObservable<TRegisterResponse>
        where TIdentifier : ConceptAs<Guid>
        where TRegisterResponse : class
    {
        /// <summary>
        /// Gets the <typeparamref name="TIdentifier"/> identifier.
        /// </summary>
        TIdentifier EventProcessorId { get; }
    }
}
