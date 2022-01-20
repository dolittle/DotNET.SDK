// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;
using Dolittle.SDK.Artifacts;

namespace Dolittle.SDK.Events.Builders;

/// <summary>
/// Defines a system that associates <see cref="Type"/> to an <see cref="EventType"/>.
/// </summary>
public interface IEventTypesBuilder
{
    /// <summary>
    /// Associate a <see cref="Type" /> with an <see cref="EventType" />.
    /// </summary>
    /// <param name="eventType">The <see cref="EventType" /> that the <see cref="Type" /> is associated to.</param>
    /// <typeparam name="T">The <see cref="Type" /> that gets associated to an <see cref="EventType" />.</typeparam>
    /// <returns>The <see cref="IEventTypesBuilder"/> for building <see cref="IEventTypes" />.</returns>
    public IEventTypesBuilder Associate<T>(EventType eventType)
        where T : class;

    /// <summary>
    /// Associate a <see cref="Type" /> with an <see cref="EventType" />.
    /// </summary>
    /// <param name="type">The <see cref="Type" /> to associate with an <see cref="EventType" />.</param>
    /// <param name="eventType">The <see cref="EventType" /> that the <see cref="Type" /> is associated to.</param>
    /// <returns>The <see cref="IEventTypesBuilder"/> for building <see cref="IEventTypes" />.</returns>
    public IEventTypesBuilder Associate(Type type, EventType eventType);

    /// <summary>
    /// Associate a <see cref="Type" /> with an <see cref="EventType" />.
    /// </summary>
    /// <param name="eventTypeId">The <see cref="EventTypeId" /> that the <see cref="Type" /> is associated to.</param>
    /// <param name="alias"><see cref="EventTypeAlias">Alias</see> of the Event Type.</param>
    /// <typeparam name="T">The <see cref="Type" /> that gets associated to an <see cref="EventType" />.</typeparam>
    /// <returns>The <see cref="IEventTypesBuilder"/> for building <see cref="IEventTypes" />.</returns>
    public IEventTypesBuilder Associate<T>(EventTypeId eventTypeId, EventTypeAlias alias = default)
        where T : class;

    /// <summary>
    /// Associate a <see cref="Type" /> with an <see cref="EventType" />.
    /// </summary>
    /// <param name="type">The <see cref="Type" /> to associate with an <see cref="EventType" />.</param>
    /// <param name="eventTypeId">The <see cref="EventTypeId" /> that the <see cref="Type" /> is associated to.</param>
    /// <param name="alias"><see cref="EventTypeAlias">Alias</see> of the Event Type.</param>
    /// <returns>The <see cref="IEventTypesBuilder"/> for building <see cref="IEventTypes" />.</returns>
    public IEventTypesBuilder Associate(Type type, EventTypeId eventTypeId, EventTypeAlias alias = default);

    /// <summary>
    /// Associate a <see cref="Type" /> with an <see cref="EventType" />.
    /// </summary>
    /// <param name="eventTypeId">The <see cref="EventTypeId" /> that the <see cref="Type" /> is associated to.</param>
    /// <param name="generation">The <see cref="Generation" /> of the <see cref="EventType" />.</param>
    /// <param name="alias"><see cref="EventTypeAlias">Alias</see> of the Event Type.</param>
    /// <typeparam name="T">The <see cref="Type" /> that gets associated to an <see cref="EventType" />.</typeparam>
    /// <returns>The <see cref="IEventTypesBuilder"/> for building <see cref="IEventTypes" />.</returns>
    public IEventTypesBuilder Associate<T>(EventTypeId eventTypeId, Generation generation, EventTypeAlias alias = default)
        where T : class;

    /// <summary>
    /// Associate a <see cref="Type" /> with an <see cref="EventType" />.
    /// </summary>
    /// <param name="type">The <see cref="Type" /> to associate with an <see cref="EventType" />.</param>
    /// <param name="eventTypeId">The <see cref="EventTypeId" /> that the <see cref="Type" /> is associated to.</param>
    /// <param name="generation">The <see cref="Generation" /> of the <see cref="EventType" />.</param>
    /// <param name="alias"><see cref="EventTypeAlias">Alias</see> of the Event Type.</param>
    /// <returns>The <see cref="IEventTypesBuilder"/> for building <see cref="IEventTypes" />.</returns>
    public IEventTypesBuilder Associate(Type type, EventTypeId eventTypeId, Generation generation, EventTypeAlias alias = default);

    /// <summary>
    /// Associate a <see cref="Type" /> with the <see cref="EventType" /> given by an attribute.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type" /> to associate with an <see cref="EventType" />.</typeparam>
    /// <returns>The <see cref="IEventTypesBuilder"/> for building <see cref="IEventTypes" />.</returns>
    /// <remarks>
    /// The type must have a eventType attribute.
    /// </remarks>
    public IEventTypesBuilder Register<T>()
        where T : class;

    /// <summary>
    /// Associate the <see cref="Type" /> with the <see cref="EventType" /> given by an attribute.
    /// </summary>
    /// <param name="type">The <see cref="Type" /> to associate with an <see cref="EventType" />.</param>
    /// <returns>The <see cref="IEventTypesBuilder"/> for building <see cref="IEventTypes" />.</returns>
    /// <remarks>
    /// The type must have a eventType attribute.
    /// </remarks>
    public IEventTypesBuilder Register(Type type);

    /// <summary>
    /// Registers all event type classes from an <see cref="Assembly" />.
    /// </summary>
    /// <param name="assembly">The <see cref="Assembly" /> to register the event type classes from.</param>
    /// <returns>The <see cref="IEventTypesBuilder" /> for continuation.</returns>
    public IEventTypesBuilder RegisterAllFrom(Assembly assembly);
}
