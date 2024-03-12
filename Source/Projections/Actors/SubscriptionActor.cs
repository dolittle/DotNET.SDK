// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Dolittle.SDK.Async;
using Proto;
using Proto.Cluster;

namespace Dolittle.SDK.Projections.Actors;

public class SubscriptionActor<T> : IActor
{
    readonly ChannelWriter<T?> _writer;
    readonly ClusterIdentity _target;
    readonly CancellationToken _stopSubscription;

    public SubscriptionActor(ChannelWriter<T?> writer, ClusterIdentity target, CancellationToken stopSubscription)
    {
        _writer = writer;
        _target = target;
        _stopSubscription = stopSubscription;
    }

    public async Task ReceiveAsync(IContext context)
    {
        try
        {
            switch (context.Message)
            {
                case Started:
                    var response = await context.Cluster().RequestAsync<Try<T?>>(_target, new SubscriptionRequest(context.Self), _stopSubscription);
                    await Handle(context, response);
                    context.ReenterAfterCancellation(_stopSubscription, () =>
                    {
                        _ = context.Cluster().RequestAsync<Unsubscribed>(_target, new Unsubscribe(context.Self), context.CancellationToken);
                        Stop(context);
                    });
                    break;

                case Try<T?> msg:
                    await Handle(context, msg);
                    break;
                case Unsubscribed msg:
                    Stop(context, msg.Exception);
                    break;
                default:
                    return;
            }
        }
        catch (Exception e)
        {
            Stop(context, e);
        }
    }

    async Task Handle(IContext context, Try<T?> response)
    {
        if (response.Success)
        {
            await _writer.WriteAsync(response.Result, _stopSubscription);
        }
        else
        {
            Stop(context, response.Exception);
        }
    }

    void Stop(IContext context, Exception? e = null)
    {
        _writer.TryComplete(e);
        context.Stop(context.Self);
    }
}
