// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Rules
{
    /// <summary>
    /// Defines the way to get a rule implementation.
    /// </summary>
    /// <typeparam name="TDelegate">Type of delegate the rule implementation is for.</typeparam>
    public interface IRuleImplementationFor<TDelegate>
    {
        /// <summary>
        /// Gets the rule.
        /// </summary>
        TDelegate Rule { get; }
    }
}