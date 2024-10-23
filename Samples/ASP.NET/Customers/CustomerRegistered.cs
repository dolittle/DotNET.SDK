// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#nullable enable
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Redaction;

namespace Customers;

[EventType("24e3a119-57d5-45d7-b7ef-a736fe6331e7")]
public class CustomerRegistered
{
    public string Name { get; init; }
    
    // These will be replaced with the given value
    [RedactablePersonalData<string>("<email redacted>")]
    public string Email { get; init; }
    
    [RedactablePersonalData<string>("<phone redacted>")]
    public string PhoneNumber { get; init; }
    
    // If the type is not specified, the property will be removed.
    [RedactablePersonalData]
    public Address? CustomerAddress { get; init; }
    
}

public class Address
{
    public string Street { get; init; }
    public string City { get; init; }
    public string ZipCode { get; init; }
}
