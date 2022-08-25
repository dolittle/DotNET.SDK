// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.ApplicationModel.ClientSetup;
using Dolittle.SDK.Concepts;

namespace Dolittle.SDK.ApplicationModel;

/// <summary>
/// Defines a builder for a processor built from the <typeparamref name="TIdentifier"/> .
/// </summary>
/// <typeparam name="TProcessor">The <see cref="Type"/> of the processor to build.</typeparam>
/// <typeparam name="TIdentifier">The <see cref="Type"/> of the model identifier.</typeparam>
/// <typeparam name="TId">The <see cref="Type"/> of the model identifier unique id.</typeparam>
public interface IProcessorBuilder<TProcessor, TIdentifier, TId> : IEquatable<IProcessorBuilder<TProcessor, TIdentifier, TId>>
    where TProcessor : class
    where TIdentifier : IIdentifier<TId>
    where TId : ConceptAs<Guid>
{
    /// <summary>
    /// Try to build a <typeparamref name="TProcessor"/> from the <typeparamref name="TIdentifier"/> .
    /// </summary>
    /// <param name="identifier">The model identifier.</param>
    /// <param name="applicationModel">The whole application model.</param>
    /// <param name="buildResults">The client build results.</param>
    /// <param name="processor">The built processor.</param>
    /// <returns>True if successful, false if not.</returns>
    bool TryBuild(TIdentifier identifier, IApplicationModel applicationModel, IClientBuildResults buildResults, out TProcessor? processor);
}
