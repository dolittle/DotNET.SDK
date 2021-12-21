// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.SDK;

/// <summary>
/// The callback for setting up an <see cref="IDolittleClient"/> by configuring the <see cref="DolittleClientBuilder"/>.
/// </summary>
/// <param name="clientBuilder">The <see cref="DolittleClientBuilder"/> to configure.</param>
public delegate void SetupDolittleClient(DolittleClientBuilder clientBuilder);
