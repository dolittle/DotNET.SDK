// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;
using System.Threading;
using Dolittle.Booting;
using Dolittle.Events.Filters.EventHorizon;
using Dolittle.Events.Filters.Internal;
using Dolittle.Logging;
using Dolittle.Reflection;
using Dolittle.Types;

namespace Dolittle.Events.Filters
{
    /// <summary>
    /// Represents a <see cref="ICanPerformBootProcedure"/> that registers event filters with the Runtime.
    /// </summary>
    public class RegistrationBootProcedure : ICanPerformBootProcedure
    {
        readonly IRegisterFilters _manager;
        readonly IInstancesOf<ICanProvideEventFilters> _filterProviders;
        readonly IInstancesOf<ICanProvideEventFiltersWithPartition> _partitionedFilterProviders;
        readonly IInstancesOf<ICanProvidePublicEventFilters> _publicFilterProviders;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistrationBootProcedure"/> class.
        /// </summary>
        /// <param name="manager">The <see cref="IRegisterFilters"/> that will be used to register the filters.</param>
        /// <param name="filterProviders">Providers of <see cref="ICanProvideEventFilters"/>.</param>
        /// <param name="partitionedFilterProviders">Providers of <see cref="ICanProvideEventFiltersWithPartition"/>.</param>
        /// <param name="publicFilterProviders">Providers of <see cref="ICanProvidePublicEventFilters"/>.</param>
        /// <param name="logger"><see cref="ILogger"/> to use for logging.</param>
        public RegistrationBootProcedure(
            IRegisterFilters manager,
            IInstancesOf<ICanProvideEventFilters> filterProviders,
            IInstancesOf<ICanProvideEventFiltersWithPartition> partitionedFilterProviders,
            IInstancesOf<ICanProvidePublicEventFilters> publicFilterProviders,
            ILogger logger)
        {
            _manager = manager;
            _filterProviders = filterProviders;
            _partitionedFilterProviders = partitionedFilterProviders;
            _publicFilterProviders = publicFilterProviders;
            _logger = logger;
        }

        /// <inheritdoc/>
        public bool CanPerform() => Microservice.Configuration.BootProcedure.HasPerformed && Artifacts.Configuration.BootProcedure.HasPerformed;

        /// <inheritdoc/>
        public void Perform()
        {
            _logger.Debug("Discovering event filters in boot procedure");
            foreach (var provider in _filterProviders) RegisterFiltersFromProvider(provider);
            foreach (var provider in _partitionedFilterProviders) RegisterFiltersFromProvider(provider);
            foreach (var provider in _publicFilterProviders) RegisterFiltersFromProvider(provider);
        }

        void RegisterFiltersFromProvider<TFilterType, TEventType, TFilterResult>(ICanProvideFilters<TFilterType, TEventType, TFilterResult> provider)
            where TFilterType : ICanFilter<TEventType, TFilterResult>
            where TEventType : IEvent
        {
            var type = provider.GetType();
            _logger.Trace("Registering filters from {FilterProvider}", type);
            try
            {
                foreach (var filter in provider.Provide()) RegisterFilter(filter);
            }
            catch (Exception ex)
            {
                _logger.Warning(ex, "Errror while providing event filters from {FilterProvider}", type);
            }
        }

        void RegisterFilter<TEventType, TFilterResult>(ICanFilter<TEventType, TFilterResult> filter)
            where TEventType : IEvent
        {
            var type = filter.GetType();
            _logger.Trace("Registering filter {Filter}", type);

            if (!type.HasAttribute<FilterAttribute>())
            {
                _logger.Warning("Event filter {Filter} is missing the required [Filter(...)] attribute. It will not be registered.", type);
                return;
            }

            var filterId = type.GetCustomAttribute<FilterAttribute>().Id;
            var scopeId = type.HasAttribute<ScopeAttribute>() ? type.GetCustomAttribute<ScopeAttribute>().Id : ScopeId.Default;

            switch (filter)
            {
                case ICanFilterEvents eventFilter:
                _manager.Register(filterId, scopeId, eventFilter, CancellationToken.None);
                return;

                case ICanFilterEventsWithPartition partitionedFilter:
                _manager.Register(filterId, scopeId, partitionedFilter, CancellationToken.None);
                return;

                case ICanFilterPublicEvents publicFilter:
                if (scopeId != ScopeId.Default)
                {
                    _logger.Warning("Public filter {Filter} has the [Scope(...)] attribute set with another scope than default. It will not be registered.", type);
                    return;
                }

                _manager.Register(filterId, publicFilter, CancellationToken.None);
                return;

                default:
                _logger.Warning("The filter {Filter} has an unknown type.", type);
                return;
            }
        }
    }
}