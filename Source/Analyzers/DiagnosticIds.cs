// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.SDK.Analyzers;

public static class DiagnosticIds
{
    /// <summary>
    /// Attribute missing the required ID.
    /// </summary>
    public const string AttributeInvalidIdentityRuleId = "SDK0001";

    /// <summary>
    /// Attribute missing the required ID.
    /// </summary>
    public const string EventMissingAttributeRuleId = "SDK0002";
    
    /// <summary>
    /// Identity is shared between multiple targets.
    /// </summary>
    public const string IdentityIsNotUniqueRuleId = "SDK0003";

    /// <summary>
    /// Aggregate missing the required Attribute.
    /// </summary>
    public const string AggregateMissingAttributeRuleId = "AGG0001";

    /// <summary>
    /// Aggregate missing a corresponding on-method.
    /// </summary>
    public const string AggregateMissingMutationRuleId = "AGG0002";

    /// <summary>
    /// Aggregate On-method has an invalid signature.
    /// </summary>
    public const string AggregateMutationHasIncorrectNumberOfParametersRuleId = "AGG0003";

    /// <summary>
    /// Aggregate On-method has an incorrect number of parameters
    /// </summary>
    public const string AggregateMutationShouldBePrivateRuleId = "AGG0004";
    
    /// <summary>
    /// Apply can not be used in an On-method. 
    /// </summary>
    public const string AggregateMutationsCannotProduceEvents = "AGG0005";
}
