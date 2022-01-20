// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.SDK.EventHorizon.for_EventHorizons.given;

public class an_event_horizons_and_a_subscription : an_event_horizons
{
    protected static Subscription subscription;

    Establish context = () => subscription = new Subscription(
        "1c2d9351-573a-4db4-8b2f-86d3f9b788e6",
        "8cd97f6a-a581-465d-a18f-38b05dc60f6e",
        "7c0fbb24-318b-4916-8443-19f5149e0990",
        "fe797449-619b-49b2-ac3a-0842cfe08874",
        "7e6c4b73-e824-4262-bea7-f0b9746bb071",
        "bb75270c-daa4-4e5e-9cf8-e4f2b427236c");
}