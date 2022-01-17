// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.SDK;

/// <summary>
/// The callback for configuring an <see cref="IDolittleClient"/> by configuring the <see cref="DolittleClientConfiguration"/>.
/// </summary>
/// <param name="clientConfiguration">The <see cref="DolittleClientConfiguration"/> to configure.</param>
public delegate void ConfigureDolittleClient(DolittleClientConfiguration clientConfiguration);
