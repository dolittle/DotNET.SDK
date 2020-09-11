// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reactive.Linq;
using System.Threading;

namespace Dolittle.SDK.Artifacts
{
    public static class ObservableExtensions
    {
        public static Exception CatchError<T>(this IObservable<T> observable)
        {
            Exception exception = null;

            using var waitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);

            observable.Subscribe(
                _ => { },
                e =>
                {
                    exception = e;
                    waitHandle.Set();
                },
                () => waitHandle.Set());

            waitHandle.WaitOne();

            return exception;
        }
    }
}