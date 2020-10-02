// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;

namespace Dolittle.SDK.Services
{
    /// <summary>
    /// Defines a system that coordinates processing.
    /// </summary>
    public interface ICoordinateProcessing
    {
        /// <summary>
        /// Gets a <see cref="Task"/> that represents the completion of all of the processing.
        /// </summary>
        Task Completion { get; }

        /// <summary>
        /// Start processing by subscribing to the given <see cref="IObservable{T}"/>.
        /// </summary>
        /// <param name="processor">The processor to start.</param>
        /// <typeparam name="T">The type of items the processor returns.</typeparam>
        void StartProcessor<T>(IObservable<T> processor);
    }
}
