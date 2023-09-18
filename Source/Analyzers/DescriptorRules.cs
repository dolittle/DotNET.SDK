// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;

namespace Dolittle.SDK.Analyzers;

static class DescriptorRules
{
    internal static readonly DiagnosticDescriptor InvalidTimestamp =
        new(
            DiagnosticIds.InvalidTimestampParameter,
            title: "Invalid DateTimeOffset format",
            messageFormat: "Value '{0}' should be a valid DateTimeOffset",
            DiagnosticCategories.Sdk,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "The value should be a valid DateTimeOffset.");
    
    internal static readonly DiagnosticDescriptor InvalidStartStopTimestamp =
        new(
            DiagnosticIds.InvalidStartStopTime,
            title: "Start is not before stop",
            messageFormat: "'{0}' should be before '{1}'",
            DiagnosticCategories.Sdk,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Start timestamp should be before stop timestamp.");
    
    internal static readonly DiagnosticDescriptor InvalidIdentity =
        new(
            DiagnosticIds.AttributeInvalidIdentityRuleId,
            title: "Invalid identity in attribute",
            messageFormat: "'{0}'.{1} '{2}' is not a valid Guid",
            DiagnosticCategories.Sdk,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Add a Guid identity");

    internal static readonly DiagnosticDescriptor DuplicateIdentity =
        new(
            DiagnosticIds.IdentityIsNotUniqueRuleId,
            title: "Identifier is not unique",
            messageFormat: "{0} '{1}' is not unique",
            DiagnosticCategories.Sdk,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Assign a unique identity in the attribute");

    internal static readonly DiagnosticDescriptor InvalidAccessibility = 
        new(
            DiagnosticIds.InvalidAccessibility,
            title: "Invalid accessibility level",
            messageFormat: "{0} needs to be '{1}'",
            DiagnosticCategories.Sdk,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Change the accessibility level to '{1}'.");

    internal static class Events
    {
        internal static readonly DiagnosticDescriptor MissingAttribute =
            new(
                DiagnosticIds.EventMissingAttributeRuleId,
                title: "Event does not have the EventTypeAttribute",
                messageFormat: "'{0}' is missing EventTypeAttribute",
                DiagnosticCategories.Sdk,
                DiagnosticSeverity.Error,
                isEnabledByDefault: true,
                description: "Mark the event with an EventTypeAttribute and assign an identifier to it");
        
        internal static readonly DiagnosticDescriptor MissingEventContext =
            new(
                DiagnosticIds.EventHandlerMissingEventContext,
                title: "Handle method does not take EventContext as the second parameter",
                messageFormat: "{0} is missing EventContext argument",
                DiagnosticCategories.Sdk,
                DiagnosticSeverity.Error,
                isEnabledByDefault: true,
                description: "Add the EventContext as the second parameter to the Handle method");
    }

    internal static class Aggregate
    {
        internal static readonly DiagnosticDescriptor MissingAttribute =
            new(
                DiagnosticIds.AggregateMissingAttributeRuleId,
                title: "Class does not have the correct identifying attribute",
                messageFormat: "'{0}' is missing {1}",
                DiagnosticCategories.Sdk,
                DiagnosticSeverity.Error,
                isEnabledByDefault: true,
                description: "Mark the class with an attribute to assign an identifier to it");

        internal static readonly DiagnosticDescriptor MissingMutation =
            new(
                DiagnosticIds.AggregateMissingMutationRuleId,
                title: "Aggregate does not have a mutation method for applied event",
                messageFormat: "'{0}' does not have a corresponding On-method",
                DiagnosticCategories.Sdk,
                DiagnosticSeverity.Info,
                isEnabledByDefault: true,
                description: "Consider adding a mutation method for the event type");

        internal static readonly DiagnosticDescriptor MutationShouldBePrivate =
            new(
                DiagnosticIds.AggregateMutationShouldBePrivateRuleId,
                title: "On-methods should be private",
                messageFormat: "'{0}' is not private",
                DiagnosticCategories.Sdk,
                DiagnosticSeverity.Error,
                isEnabledByDefault: true,
                description: "Change the On-method visibility to private");

        internal static readonly DiagnosticDescriptor MutationHasIncorrectNumberOfParameters =
            new(
                DiagnosticIds.AggregateMutationHasIncorrectNumberOfParametersRuleId,
                title: "On-methods should take a single event as a parameter",
                messageFormat: "'{0}' is invalid, On-methods should take a single event as a parameter",
                DiagnosticCategories.Sdk,
                DiagnosticSeverity.Error,
                isEnabledByDefault: true,
                description: "Change the On-method to take a single event as a parameter");

        internal static readonly DiagnosticDescriptor MutationsCannotProduceEvents = new(
            DiagnosticIds.AggregateMutationsCannotProduceEvents,
            "Apply method should not be called within On method",
            "Apply method should not be called within On method in aggregates as they are NOT allowed to produce new events",
            DiagnosticCategories.Sdk,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true
        );
        
        internal static readonly DiagnosticDescriptor PublicMethodsCannotMutateAggregateState = new(
            DiagnosticIds.PublicMethodsCannotMutateAggregateState,
            "Aggregates should only be mutated with events",
            "Public methods can not mutate the state of an aggregate. All mutations needs to be done via events.",
            DiagnosticCategories.Sdk,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true
        );
    }
}
