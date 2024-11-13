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
    
    internal static readonly DiagnosticDescriptor InvalidTimespan =
        new(
            DiagnosticIds.InvalidTimeSpanParameter,
            title: "Invalid Timespan format",
            messageFormat: "Value '{0}' should be a valid Timespan",
            DiagnosticCategories.Sdk,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "The value should be a valid Timespan.");
    
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
    
    internal static readonly DiagnosticDescriptor MissingBaseClass =
        new(
            DiagnosticIds.MissingBaseClassRuleId,
            title: "Class is missing required base class",
            messageFormat: "'{0}' is not a base class of {1}",
            DiagnosticCategories.Sdk,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Add the required base class.");

    internal static readonly DiagnosticDescriptor InvalidAccessibility = 
        new(
            DiagnosticIds.InvalidAccessibility,
            title: "Invalid accessibility level",
            messageFormat: "{0} needs to be '{1}'",
            DiagnosticCategories.Sdk,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Change the accessibility level to '{1}'.");


    internal static readonly DiagnosticDescriptor ExceptionInMutation =
        new(
            DiagnosticIds.ExceptionInMutation,
            title: "Exceptions can not be thrown from mutation methods",
            messageFormat: "On-methods can not throw exceptions",
            DiagnosticCategories.Sdk,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "On-methods can not throw exceptions. This will prevent the system from being able to replay events.");
    
    internal static readonly DiagnosticDescriptor NonNullableRedactableField =
        new(
            DiagnosticIds.NonNullableRedactableField,
            title: "Redactable fields must be nullable",
            messageFormat: "Since the value can be deleted, the field '{0}' should be nullable",
            DiagnosticCategories.Sdk,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "The field should be nullable since the value can be deleted.");
    
    internal static readonly DiagnosticDescriptor IncorrectRedactableFieldType =
        new(
            DiagnosticIds.RedactableFieldIncorrectType,
            title: "Generic Type must match the property type",
            messageFormat: "The generic type for RedactablePersonalDataAttribute was {0}, must match the property type {1}",
            DiagnosticCategories.Sdk,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "The generic type must match the property type.");

    internal static readonly DiagnosticDescriptor IncorrectRedactedEventTypePrefix =
        new(
            DiagnosticIds.RedactionEventIncorrectPrefix,
            title: "Redaction events must have the correct prefix",
            messageFormat: "The prefix for redaction events types should be 'de1e7e17-bad5-da7a', was '{0}'",
            DiagnosticCategories.Sdk,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "In order for redaction events to be recognized by the runtime, it must have the correct prefix.");
    
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
                description: "Mark the class with an attribute to assign an identifier to it.");

        internal static readonly DiagnosticDescriptor MissingMutation =
            new(
                DiagnosticIds.AggregateMissingMutationRuleId,
                title: "Aggregate does not have a mutation method for applied event",
                messageFormat: "'{0}' does not have a corresponding On-method",
                DiagnosticCategories.Sdk,
                DiagnosticSeverity.Info,
                isEnabledByDefault: true,
                description: "Consider adding a mutation method for the event type.");

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
                description: "Change the On-method to take a single event as a parameter.");
        
        internal static readonly DiagnosticDescriptor MutationsCannotUseCurrentTime =
            new(
                DiagnosticIds.AggregateMutationsCannotUseCurrentTime,
                title: "On-methods must only use data from the event",
                messageFormat: "'{0}' is invalid, On-methods must only use data from the event",
                DiagnosticCategories.Sdk,
                DiagnosticSeverity.Error,
                isEnabledByDefault: true,
                description: "If you need to capture the time of the event in the aggregate state, you must include it in the event.");

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

    internal static class Projection
    {
        internal static readonly DiagnosticDescriptor MissingAttribute =
            new(
                DiagnosticIds.ProjectionMissingAttributeRuleId,
                title: "Class does not have the correct identifying ProjectionAttribute",
                messageFormat: "'{0}' is missing ProjectionAttribute",
                DiagnosticCategories.Sdk,
                DiagnosticSeverity.Error,
                isEnabledByDefault: true,
                description: "Mark the class with an attribute to assign an identifier to it");

        // ProjectionMissingBaseClassRuleId
        
        internal static readonly DiagnosticDescriptor MissingBaseClass =
            new(
                DiagnosticIds.ProjectionMissingBaseClassRuleId,
                title: "Projection does not inherit from ReadModel",
                messageFormat: "'{0}' does not inherit from ReadModel",
                DiagnosticCategories.Sdk,
                DiagnosticSeverity.Error,
                isEnabledByDefault: true,
                description: "Inherit from ReadModel.");
        
        internal static readonly DiagnosticDescriptor InvalidOnMethodParameters =
            new(
                DiagnosticIds.ProjectionInvalidOnMethodParametersRuleId,
                title: "Invalid On-method",
                messageFormat: "'{0}' has an invalid signature",
                DiagnosticCategories.Sdk,
                DiagnosticSeverity.Error,
                isEnabledByDefault: true,
                description: "Change the On-method to match the required signature. The method should take an event as first parameter and a ProjectionContext as the second parameter.");
        
        internal static readonly DiagnosticDescriptor InvalidOnMethodReturnType =
            new(
                DiagnosticIds.ProjectionInvalidOnMethodReturnTypeRuleId,
                title: "Invalid On-method return type",
                messageFormat: "'{0}' returns an invalid type",
                DiagnosticCategories.Sdk,
                DiagnosticSeverity.Error,
                isEnabledByDefault: true,
                description: "Change the On-method to return void, ProjectionResultType or ProjectionResult<TReadModel>.");
        
        internal static readonly DiagnosticDescriptor InvalidOnMethodVisibility =
            new(
                DiagnosticIds.InvalidAccessibility,
                title: "On-methods need to be public",
                messageFormat: "'{0}' is not public",
                DiagnosticCategories.Sdk,
                DiagnosticSeverity.Error,
                isEnabledByDefault: true,
                description: "Change the On-method to public visibility.");
        
        internal static readonly DiagnosticDescriptor EventTypeAlreadyHandled =
            new(
                DiagnosticIds.ProjectionDuplicateEventHandler,
                title: "Event type already handled",
                messageFormat: "'{0}' is already handled",
                DiagnosticCategories.Sdk,
                DiagnosticSeverity.Error,
                isEnabledByDefault: true,
                description: "The event type is already handled by another On-method.");
        
        internal static readonly DiagnosticDescriptor MutationUsedCurrentTime =
            new(
                DiagnosticIds.ProjectionMutationUsedCurrentTime,
                title: "On-methods must only use data from the event or EventContext",
                messageFormat: "'{0}' is invalid, On-methods must only use data from the event or EventContext",
                DiagnosticCategories.Sdk,
                DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: "If you need to get the committed time of the event, use EventContext.Occurred.");
    }

}
