// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events;

namespace Kitchen;

[EventType("76faefee-102e-4c6f-b65d-0b47fedece42")]
public record Restocked(int Amount, string Supplier);