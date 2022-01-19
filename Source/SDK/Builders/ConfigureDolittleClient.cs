// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.SDK.Builders;

/// <summary>
/// The callback for configuring an <see cref="DolittleClientConfiguration"/> by configuring the <see cref="DolittleClientConfiguration"/>.
/// </summary>
/// <param name="configurationBuilder">The <see cref="IConfigurationBuilder"/> for building the <see cref="DolittleClientConfiguration"/>.</param>
public delegate void ConfigureDolittleClient(IConfigurationBuilder configurationBuilder);
