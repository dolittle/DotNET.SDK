// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;

namespace Dolittle.SDK.Async
{
    /// <summary>
    /// Extension methods for <see cref="Task" />.
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        /// Tries to perform a task that returns a <typeparamref name="T">result</typeparamref>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type" /> of the result of the <see cref="Task{TResult}" />.</typeparam>
        /// <param name="performTask">The callback that performs the task.</param>
        /// <returns>A <see cref="Task{TResult}" /> that, when resolved returns <see cref="Try{TResult}" />.</returns>
        public static async Task<Try<T>> TryTask<T>(this Func<Task<T>> performTask)
        {
            try
            {
                return await performTask().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        /// <summary>
        /// Tries to perform a task .
        /// </summary>
        /// <param name="performTask">The callback that performs the task.</param>
        /// <returns>A <see cref="Task{TResult}" /> that, when resolved returns <see cref="Try{TResult}" />.</returns>
        public static async Task<Try> TryTask(this Func<Task> performTask)
        {
            try
            {
                await performTask().ConfigureAwait(false);
                return new Try();
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        /// <summary>
        /// Try get the <see cref="Exception" /> from a <see cref="Task" />.
        /// </summary>
        /// <param name="task">The <see cref="Task" />.</param>
        /// <param name="exception">The <see cref="Exception" />.</param>
        /// <returns>A value indicating whether the <see cref="Task" /> had an <see cref="Exception" />.</returns>
        public static bool TryGetException(this Task task, out Exception exception)
        {
            exception = task.Exception;
            return exception != null;
        }

        /// <summary>
        /// Try get the innermost <see cref="Exception" /> from a <see cref="Task" />.
        /// </summary>
        /// <param name="task">The <see cref="Task" />.</param>
        /// <param name="exception">The <see cref="Exception" />.</param>
        /// <returns>A value indicating whether the <see cref="Task" /> had an <see cref="Exception" />.</returns>
        public static bool TryGetInnerMostException(this Task task, out Exception exception)
        {
            var hasException = task.TryGetException(out exception);
            while (exception?.InnerException != null) exception = exception.InnerException;
            return hasException;
        }
    }
}