// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Loggers;
using Dolittle.SDK.Builders;
using Dolittle.SDK.Tenancy;

namespace Dolittle.SDK.Benchmarks;

/// <summary>
/// Represents a base for a <see cref="BenchmarkDotNet.Jobs.Job"/> that initializes a Runtime container for each benchmark.
/// </summary>
public abstract class JobBase
{
    Harness.Harness _harness;
    
    /// <summary>
    /// Starts a new <see cref="Harness.Harness"/> and waits for it to boot.
    /// </summary>
    [GlobalSetup]
    public void GlobalSetup()
    {
        var logger = ConsoleLogger.Default;
        var tenants = GetTenantsToSetup();
        ConfiguredTenants = tenants;
        _harness = Harness.Harness.Start(
                logger,
                SetupDolittleClient,
                ConfigureDolittleClient,
                tenants)
            .GetAwaiter().GetResult();
        Setup(_harness.Client);
    }

    /// <summary>
    /// Disposes of the started <see cref="Harness.Harness"/> and waits for completion.
    /// </summary>
    [GlobalCleanup]
    public void GlobalCleanup()
    {
        Cleanup();
        _harness.DisposeAsync().AsTask().GetAwaiter().GetResult();
    }

    /// <summary>
    /// Method to override to setup the <see cref="IDolittleClient"/> for this benchmark.
    /// </summary>
    /// <param name="setupBuilder">The setup builder to use to setup the Dolittle Client.</param>
    protected virtual void SetupDolittleClient(ISetupBuilder setupBuilder)
    {
    }

    /// <summary>
    /// Method to override to configure the <see cref="IDolittleClient"/> for this benchmark.
    /// </summary>
    /// <param name="configurationBuilder">The configuration builder to use to configure the Dolittle Client.</param>
    protected virtual void ConfigureDolittleClient(IConfigurationBuilder configurationBuilder)
    {
    }
    
    /// <summary>
    /// Gets the <see cref="TenantId">tenants</see> configured for the running benchmark.
    /// </summary>
    protected IEnumerable<TenantId> ConfiguredTenants { get; private set; }

    /// <summary>
    /// Method to override to define the <see cref="TenantId">tenants</see> to configure for the Runtime container.
    /// </summary>
    /// <returns>The array of <see cref="TenantId"/> to configure.</returns>
    protected virtual TenantId[] GetTenantsToSetup()
        => new[] { TenantId.Development };

    /// <summary>
    /// The method that sets up the environment for each benchmark to run.
    /// </summary>
    /// <param name="client">The <see cref="IDolittleClient"/> connected to the Runtime container started for this benchmark.</param>
    protected abstract void Setup(IDolittleClient client);

    /// <summary>
    /// The method that cleans up the environment after each benchmark run.
    /// </summary>
    protected abstract void Cleanup();
}
