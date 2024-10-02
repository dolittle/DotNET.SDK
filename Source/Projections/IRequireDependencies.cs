// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Projections;

/// <summary>
/// Marks a read model that requires external dependencies
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IRequireDependencies<T> where T : ReadModel
{
    /// <summary>
    /// Initialize the read model with the required dependencies
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <returns></returns>
    public void Resolve(IServiceProvider serviceProvider);
}
