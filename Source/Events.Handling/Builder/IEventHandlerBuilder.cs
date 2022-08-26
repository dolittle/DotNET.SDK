// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.ApplicationModel;
using Dolittle.SDK.Events.Handling.Builder.Methods;

namespace Dolittle.SDK.Events.Handling.Builder;

/// <summary>
/// Defines a builder for an event handler.
/// </summary>
public interface IEventHandlerBuilder
{
    /// <summary>
    /// Defines the event handler to be partitioned - this is default for an event handler.
    /// </summary>
    /// <returns>The builder for continuation.</returns>
    IEventHandlerMethodsBuilder Partitioned();

    /// <summary>
    /// Defines the event handler to be unpartitioned. By default it will be partitioned.
    /// </summary>
    /// <returns>The builder for continuation.</returns>
    IEventHandlerMethodsBuilder Unpartitioned();

    /// <summary>
    /// Defines the event handler to operate on a specific <see cref="ScopeId" />.
    /// </summary>
    /// <param name="scopeId">The <see cref="ScopeId" />.</param>
    /// <returns>The builder for continuation.</returns>
    IEventHandlerBuilder InScope(ScopeId scopeId);

    /// <summary>
    /// Defines the event handler to have a specific <see cref="IdentifierAlias" />.
    /// </summary>
    /// <param name="alias">The <see cref="IdentifierAlias" />.</param>
    /// <returns>The builder for continuation.</returns>
    IEventHandlerBuilder WithAlias(IdentifierAlias alias);
}
