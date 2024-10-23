// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Redaction;

namespace Customers;

[EventType("849494db-3173-4259-a4f7-e409ae1785dc")]
public class CustomerDetailsForgotten : PersonalDataRedactedForEvent<CustomerRegistered>
{
    
}