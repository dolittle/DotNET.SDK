// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.Booting;
using Dolittle.Commands.Coordination;
using Dolittle.Execution;
using Dolittle.Tenancy;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EventSourcing
{
    static class Program
    {
        static async Task Main()
        {
            var hostBuilder = new HostBuilder();
            hostBuilder.ConfigureLogging(_ => _.AddConsole());
            hostBuilder.UseEnvironment("Development");
            var host = hostBuilder.Build();
            var loggerFactory = host.Services.GetService(typeof(ILoggerFactory)) as ILoggerFactory;

            var result = Bootloader.Configure(_ =>
            {
                _.UseLoggerFactory(loggerFactory);
                _.Development();
            }).Start();

            var logger = result.Container.Get<Dolittle.Logging.ILogger>();
            logger.Information("Booted");

            // await HeadConnectionLifecycle.Connected.ConfigureAwait(false);
            var commandContextManager = result.Container.Get<ICommandContextManager>();
            var executionContextManager = result.Container.Get<IExecutionContextManager>();
            var commandCoordinator = result.Container.Get<ICommandCoordinator>();

            executionContextManager.CurrentFor(TenantId.Development);

            logger.Information("Handle command");

            var commandResult = commandCoordinator.Handle(new MyCommand());

            logger.Information("Success : {Result}", commandResult.Success);

            await host.RunAsync().ConfigureAwait(false);
        }
    }
}
