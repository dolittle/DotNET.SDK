// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Services
{
    /// <summary>
    /// Represents an implementation of <see cref="ICoordinateProcessing" />.
    /// </summary>
    public class ProcessingCoordinator : ICoordinateProcessing, IDisposable
    {
        readonly TaskCompletionSource<Unit> _taskCompletionSource = new TaskCompletionSource<Unit>(TaskCreationOptions.RunContinuationsAsynchronously);
        readonly Subject<IObservable<Unit>> _processors = new Subject<IObservable<Unit>>();
        readonly ILogger _logger;
        readonly CancellationToken _cancellationToken;
        bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessingCoordinator"/> class.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        /// <param name="token">The <see cref="CancellationToken"/> which if cancelled will stop the processing.</param>
        public ProcessingCoordinator(ILogger logger, CancellationToken token)
        {
            _logger = logger;
            _cancellationToken = token;
            _cancellationToken.Register(() => _processors.OnCompleted());
            StartProcessing();
        }

        /// <inheritdoc/>
        public Task Completion => _taskCompletionSource.Task;

        /// <inheritdoc/>
        public void StartProcessor<T>(IObservable<T> processor)
        {
            ThrowIfProcessingCoordinatorIsStopped();
            _processors.OnNext(processor.Select(_ => Unit.Default));
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose resources.
        /// </summary>
        /// <param name="disposeManagedResources">Whether to dispose managed resources.</param>
        protected virtual void Dispose(bool disposeManagedResources)
        {
            if (_disposed) return;

            _taskCompletionSource.TrySetCanceled();
            if (disposeManagedResources)
            {
                _processors.Dispose();
            }

            _disposed = true;
        }

        void ThrowIfProcessingCoordinatorIsStopped()
        {
            if (_cancellationToken.IsCancellationRequested)
            {
                throw new CannotAddProcessorsToAStoppedProcessingCoordinator();
            }
        }

        void StartProcessing()
            => _processors
                .Merge()
                .Subscribe(
                    _ => { },
                    exception =>
                    {
                        _logger.LogError(exception, "An error occured during execution of processors");
                        _taskCompletionSource.SetException(exception);
                    },
                    () => _taskCompletionSource.SetResult(Unit.Default));
    }
}
