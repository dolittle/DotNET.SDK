// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.EventHorizon;

/// <summary>
/// Retry policy method for event subscriptions.
/// </summary>
/// <param name="subscription">Subscription that we want to have a retry policy for.</param>
/// <param name="logger">Logger.</param>
/// <param name="methodToPerform">Method to perform.</param>
public delegate Task EventSubscriptionRetryPolicy(Subscription subscription, ILogger logger, Func<Task<bool>> methodToPerform);
