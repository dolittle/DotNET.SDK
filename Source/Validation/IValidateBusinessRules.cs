// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Validation
{
    /// <summary>
    /// Marker interface to identify types that can perform business validation.
    /// </summary>
    /// <remarks>
    /// Types inheriting from this interface will be automatically registered and used for validation of properties
    /// of types (i.e. commands) for which there are no explicitly defined validators.
    /// You most likely want to subclass <see cref="BusinessValidator{T}"/>.
    /// </remarks>
    /// <typeparam name="T">Type being validated.</typeparam>
    public interface IValidateBusinessRules<T>
    {
    }
}
