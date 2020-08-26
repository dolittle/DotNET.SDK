// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Dolittle.Collections;
using Dolittle.Events;
using Dolittle.Reflection;
using Dolittle.Rules;
using Dolittle.Transactions;

namespace Dolittle.Domain
{
    /// <summary>
    /// Represents a base class to be used for Aggregate Roots in your domain.
    /// </summary>
    public abstract class AggregateRoot : ITransaction
    {
        readonly List<RuleSetEvaluation> _ruleSetEvaluations;
        readonly List<BrokenRule> _brokenRules;
        readonly NullFreeList<IEvent> _uncommittedEvents;

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRoot"/> class.
        /// </summary>
        /// <param name="eventSourceId">The <see cref="Events.EventSourceId"/> that the Aggregate Root will apply events on.</param>
        protected AggregateRoot(EventSourceId eventSourceId)
        {
            EventSourceId = eventSourceId;
            Version = AggregateRootVersion.Initial;
            _ruleSetEvaluations = new List<RuleSetEvaluation>();
            _brokenRules = new List<BrokenRule>();
            _uncommittedEvents = new NullFreeList<IEvent>();
        }

        /// <summary>
        /// Gets the <see cref="Events.EventSourceId"/> that the Aggregate Root will apply events to.
        /// </summary>
        public EventSourceId EventSourceId { get; }

        /// <summary>
        /// Gets the current <see cref="AggregateRootVersion"/> reflecting the number of events it has applied to the Event Source.
        /// </summary>
        public AggregateRootVersion Version { get; private set; }

        /// <summary>
        /// Gets sequence of <see cref="IEvent"/>s applied by the Event Source that have not been committed to the Event Store.
        /// </summary>
        public UncommittedAggregateEvents UncommittedEvents
        {
            get
            {
                var events = new UncommittedAggregateEvents(EventSourceId, GetType(), Version - (uint)_uncommittedEvents.Count);
                foreach (var @event in _uncommittedEvents) events.Append(@event);
                return events;
            }
        }

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}">collection</see> of <see cref="BrokenRule">broken rules</see>.
        /// </summary>
        public IEnumerable<BrokenRule> BrokenRules => _brokenRules;

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}">collection</see> of <see cref="RuleSetEvaluation">evaluations</see>.
        /// </summary>
        public IEnumerable<RuleSetEvaluation> RuleSetEvaluations => _ruleSetEvaluations;

        /// <summary>
        /// Builds an evaluation of rules that needs to be passed.
        /// </summary>
        /// <param name="rules"><see cref="IRule">Rules</see> to evaluate.</param>
        /// <returns><see cref="RuleSetEvaluation"/> for handling the evaluation.</returns>
        public RuleSetEvaluation Evaluate(params IRule[] rules)
        {
            var evaluation = new RuleSetEvaluation(new RuleSet(rules));
            evaluation.Evaluate(this);
            _ruleSetEvaluations.Add(evaluation);
            _brokenRules.AddRange(evaluation.BrokenRules);
            return evaluation;
        }

        /// <summary>
        /// Builds an evaluation of rules that needs to be passed.
        /// </summary>
        /// <param name="rules">Rules based on method expressions to evaluate.</param>
        /// <returns><see cref="RuleSetEvaluation"/> for handling the evaluation.</returns>
        public RuleSetEvaluation Evaluate(params Expression<Func<RuleEvaluationResult>>[] rules)
        {
            var actualRules = new List<MethodRule>();
            foreach (var rule in rules)
            {
                var name = rule.GetMethodInfo().Name;
                var method = rule.Compile();
                actualRules.Add(new MethodRule(name, method));
            }

            return Evaluate(actualRules.ToArray());
        }

        /// <summary>
        /// Apply a new event to the EventSource.  This will be applied and added to the <see cref="UncommittedEvents">uncommitted events</see>.
        /// </summary>
        /// <param name="event">The event that is to be applied.</param>
        public void Apply(IEvent @event)
        {
            ThrowIfEventIsNull(@event);
            _uncommittedEvents.Add(@event);
            Version++;
            InvokeOnMethod(@event);
        }

        /// <summary>
        /// Re-apply events from the Event Store.
        /// </summary>
        /// <param name="events">Sequence that contains the events to re-apply.</param>
        public virtual void ReApply(CommittedAggregateEvents events)
        {
            ThrowIfEventWasAppliedToOtherEventSource(events);
            ThrowIfEventWasAppliedByOtherAggregateRoot(events);

            foreach (var @event in events)
            {
                ThrowIfAggreggateRootVersionIsOutOfOrder(@event);
                InvokeOnMethod(@event.Event);
                Version++;
            }
        }

        /// <summary>
        /// Fast forward to the specified version of the <see cref="EventSourceId">EventSource</see>.
        /// </summary>
        /// <param name="version">Version to fast forward to.</param>
        public void FastForward(AggregateRootVersion version)
        {
            ThrowIfStateful();
            Version = version;
        }

        /// <inheritdoc/>
        public virtual void Commit()
        {
            _uncommittedEvents.Clear();
        }

        /// <inheritdoc/>
        public virtual void Rollback()
        {
            Version -= (uint)_uncommittedEvents.Count;
            _uncommittedEvents.Clear();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Commit();
        }

        void InvokeOnMethod(IEvent @event)
        {
            var handleMethod = this.GetOnMethod(@event);
            handleMethod?.Invoke(this, new[] { @event });
        }

        void ThrowIfEventIsNull(IEvent @event)
        {
            if (@event == null) throw new EventCanNotBeNull();
        }

        void ThrowIfEventWasAppliedToOtherEventSource(CommittedAggregateEvents events)
        {
            if (events.EventSource != EventSourceId) throw new EventWasAppliedToOtherEventSource(events.EventSource, EventSourceId);
        }

        void ThrowIfEventWasAppliedByOtherAggregateRoot(CommittedAggregateEvents events)
        {
            if (events.AggregateRoot != GetType()) throw new EventWasAppliedByOtherAggregateRoot(events.AggregateRoot, GetType());
        }

        void ThrowIfAggreggateRootVersionIsOutOfOrder(CommittedAggregateEvent @event)
        {
            if (@event.AggregateRootVersion != Version) throw new AggregateRootVersionIsOutOfOrder(@event.AggregateRootVersion, Version);
        }

        void ThrowIfStateful()
        {
            if (!this.IsStateless()) throw new FastForwardNotAllowedForStatefulAggregateRoot(GetType());
        }
    }
}
