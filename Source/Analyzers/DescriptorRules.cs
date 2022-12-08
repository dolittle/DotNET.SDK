﻿// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;

namespace Dolittle.SDK.Analyzers;

static class DescriptorRules
{
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
    }
}
