// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace Dolittle.SDK.Benchmarks.Harness;

/// <summary>
/// Represents extension methods for <see cref="IDockerClient"/>.
/// </summary>
public static class DockerClientExtensions
{
    /// <summary>
    /// Gets a container status by ID.
    /// </summary>
    /// <param name="client">The <see cref="IDockerClient"/> to use to start the container.</param>
    /// <param name="id">The ID of the container to get.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>The <see cref="ContainerListResponse"/> status of the container.</returns>
    public static async Task<ContainerListResponse> GetContainerById(this IDockerClient client, string id, CancellationToken cancellationToken = default)
    {
        foreach (var container in await client.Containers.ListContainersAsync(ListContainerById(id), cancellationToken).ConfigureAwait(false))
        {
            if (container.ID == id)
            {
                return container;
            }
        }

        throw new Exception("Could not find container");
    }
    
    /// <summary>
    /// Starts a container by ID and returns the status of the started container.
    /// </summary>
    /// <param name="client">The <see cref="IDockerClient"/> to use to start the container.</param>
    /// <param name="id">The ID of the container to start.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>The <see cref="ContainerListResponse"/> status of the started container.</returns>
    public static async Task<ContainerListResponse> StartContainer(this IDockerClient client, string id, CancellationToken cancellationToken = default)
    {
        foreach (var container in await client.Containers.ListContainersAsync(ListContainerById(id), cancellationToken).ConfigureAwait(false))
        {
            if (container.ID == id && container.State == "running")
            {
                return container;
            }
        }
        
        if (!await client.Containers.StartContainerAsync(id, new ContainerStartParameters(), cancellationToken).ConfigureAwait(false))
        {
            throw new Exception("Could not start container");
        }

        foreach (var container in await client.Containers.ListContainersAsync(ListContainerById(id), cancellationToken).ConfigureAwait(false))
        {
            if (container.ID == id && container.State == "running")
            {
                return container;
            }
        }

        throw new Exception("Could not start container");
    }

    /// <summary>
    /// Creates, starts and returns the status of a new container with the specified create parameters.
    /// </summary>
    /// <param name="client">The <see cref="IDockerClient"/> to use to create and start the container.</param>
    /// <param name="parameters">The <see cref="CreateContainerParameters"/> to use to create the container.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>The <see cref="ContainerListResponse"/> status of the created container.</returns>
    public static async Task<ContainerListResponse> StartNewContainer(this IDockerClient client, CreateContainerParameters parameters, CancellationToken cancellationToken = default)
    {
        var created = await client.Containers.CreateContainerAsync(parameters, cancellationToken).ConfigureAwait(false);
        return await client.StartContainer(created.ID, cancellationToken).ConfigureAwait(false);
    }

    static ContainersListParameters ListContainerById(string id) 
        => new()
        {
            All = true,
            Filters = new Dictionary<string, IDictionary<string, bool>>
            {
                ["id"] = new Dictionary<string, bool>
                {
                    [id] = true,
                },
            },
        };
}
