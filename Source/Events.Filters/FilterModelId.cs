// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.ApplicationModel;

namespace Dolittle.SDK.Events.Filters;

/// <summary>
/// Represents the identifier of a filter in an application model.
/// </summary>
public class FilterModelId : Identifier<FilterId, ScopeId>
{
    /// <summary>
    /// Initializes an instance of the <see cref="FilterModelId"/>m clas
    /// </summary>
    /// <param name="id">The <see cref="FilterId"/>.</param>
    /// <param name="scope">The <see cref="ScopeId"/>.</param>
    public FilterModelId(FilterId id, ScopeId scope) : base("Filter", id, scope)
    {
        Scope = scope;
    }
    
    /// <summary>
    /// Gets the <see cref="ScopeId"/>.
    /// </summary>
    public ScopeId Scope { get; }
}
