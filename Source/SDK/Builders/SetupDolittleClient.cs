// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Builders;

namespace Dolittle.SDK;

/// <summary>
/// The callback for setting up an <see cref="IDolittleClient"/> by configuring the <see cref="DolittleClientBuilder"/>.
/// </summary>
/// <param name="setupBuilder">The <see cref="ISetupBuilder"/> to configure for setting up the <see cref="IDolittleClient"/>.</param>
public delegate void SetupDolittleClient(ISetupBuilder setupBuilder);
