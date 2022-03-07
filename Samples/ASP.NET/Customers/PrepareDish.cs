// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel.DataAnnotations;

namespace Customers;

public class EatDish
{
    [Required]
    public string Dish { get; set; } = "";

    [Required]
    public string Customer { get; set; } = "";
}
