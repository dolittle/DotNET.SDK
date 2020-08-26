---
title: Event Processors
description: This contains details on how event processors work in C#
keywords: Events, Processors
author: einari
weight: 1
aliases: 
    - /runtime/dotnet-sdk/events/event_processors
    - /runtime/dotnet.sdk/events/event_processors
---

Event processors are key to dealing acting on state changes represented as events coming through the system.
Processing an even can mean pretty much anything - it is a reactive model that enables one to react and perform
relevant actions in the system. This often means saving something to a database, but could also mean that you
could be sending out an email or doing other tasks as a consequence.

A key aspect of event processors in Dolittle is that you can have multiple processors for each type of event.
These processors does not need to know about each other at all, in fact - it is fundamental that you think of
them in an autonomous decoupled manner, as there is no guarantee in which order they will be called.
If you have the need for ordering, you should be explicit about it and instead have a processor that will
orchestrate and delegate the work.

In C#, in order for you to be able to process an event, there is a marker interface that you need to mark
your class with; `ICanProcessEvents`- which is found in the `Dolittle.Events` namespace:

```csharp
using Dolittle.Events;

namespace MyNamespace
{
    public class MyEventProcessors : ICanProcessEvents
    {
    }
}
```

Once you have a class marked with this interface, you **MUST** implement a method for the event you want to
process. Since the `ICanProcessEvents` does not require you to implement anything - it means that this is
a convention in Dolittle. At startup, Dolittle will discover any implementations of the `ICanProcessEvents`
interface and look for public methods called Process() on the implementation with one of the three following
signatures:

```csharp
public void Process(MyEvent @event);
public void Process(MyEvent @event, EventSourceId eventSourceId);
public void Process(MyEvent @event, EventMetadata eventMetadata);
```

You pick the one that makes the most sense for your purpose. If your event does not need to know about
the `EventSourceId` or any other details in the `EventMetadata`- you can use the first overload.

{{% notice note %}}
Your event processor can have process multiple event types. You simply just add a `Process()` method for
any events that makes sense to process in your calss.
{{% /notice %}}

## Dependency Inversion

Since the event processor itself will be instantiated using the IoC container, you can take dependencies
to whatever you want to. Since your often dealing with storing data, a repository for read models could
be the thing you need. An example of using it with a `Process()` method:

```csharp
using Dolittle.Events;
using Dolittle.Read;

namespace MyNamespace
{
    public class MyEventProcessors : ICanProcessEvents
    {
        IReadModelRepositoryFor<MyReadModel> _repository;

        public MyEventProcessors(IReadModelRepositoryFor<MyReadModel> repository)
        {
            _repository = repository;
        }

        public void Process(MyEvent @event, EventSourceId eventSourceId)
        {
            _repository.Insert(new MyReadModel {
                Id = eventSourceId,
                SomeProperty = @event.SomeProperty
            });
        }
    }
}
```
