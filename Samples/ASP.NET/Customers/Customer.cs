// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Redaction;
using Microsoft.Extensions.Logging;

namespace Customers;

[AggregateRoot("dcdaecc0-29c9-41f4-96d1-9bddefe8b39a")]
public class Customer : AggregateRoot
{
    readonly ILogger<Customer> _logger;

    string? _name;
    string? _email;
    string? _phoneNumber;
    Address? _customerAddress;

    public Customer(EventSourceId eventSource, ILogger<Customer> logger)
        : base(eventSource)
    {
        _logger = logger;
    }

    string Name => EventSourceId;

    public void Register(string name, string email, string phoneNumber, Address address)
    {
        if (_name is not null)
        {
            throw new CustomerAlreadyRegistered();
        }

        Apply(new CustomerRegistered
        {
            Name = name,
            Email = email,
            PhoneNumber = phoneNumber,
            CustomerAddress = address
        });
    }

    public void GdprForget(string reason, string performedBy)
    {
        if (_name is null)
        {
            throw new CustomerNotRegistered();
        }
        

        Apply(Redactions.Create<CustomerRegistered, CustomerDetailsForgotten>(reason, performedBy));
    }

    public void EatDish(string dish)
    {
        Apply(new DishEaten(dish, Name));
        _logger.LogInformation("Customer {Name} is eating {Dish}", Name, dish);
    }

    public void On(CustomerRegistered evt)
    {
        _name = evt.Name;
        _email = evt.Email;
        _phoneNumber = evt.PhoneNumber;
        _customerAddress = evt.CustomerAddress;
    }
    
    public void On(CustomerDetailsForgotten _)
    {
        _email = null;
        _phoneNumber = null;
        _customerAddress = null;
    }
}

public class CustomerNotRegistered : Exception
{
}

public class CustomerAlreadyRegistered : Exception
{
}