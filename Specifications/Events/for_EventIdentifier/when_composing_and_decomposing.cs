// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.ApplicationModel;
using Dolittle.Tenancy;
using Machine.Specifications;

namespace Dolittle.Events.for_EventIdentifier
{
    public class when_composing_and_decomposing
    {
        static Microservice microservice;
        static TenantId tenant;
        static EventLogSequenceNumber sequence_number;
        static bool did_decompose;
        static Microservice decomposed_microservice;
        static TenantId decomposed_tenant;
        static EventLogSequenceNumber decomposed_sequence_number;

        Establish context = () =>
        {
            microservice = Guid.Parse("67200e3b-0474-46e0-acaf-04392dc808a4");
            tenant = Guid.Parse("ca7bb7de-5f4c-43b2-86bd-1ceca4c7a17a");
            sequence_number = 459939704;
        };

        Because of = () =>
        {
            var eventIdentifier = EventIdentifier.Compose(microservice, tenant, sequence_number);
            did_decompose = EventIdentifier.TryDecompose(eventIdentifier, out decomposed_microservice, out decomposed_tenant, out decomposed_sequence_number);
        };

        It should_succeed = () => did_decompose.ShouldEqual(true);
        It should_decompose_the_correct_microservice = () => decomposed_microservice.ShouldEqual(microservice);
        It should_decompose_the_correct_tenant = () => decomposed_tenant.ShouldEqual(tenant);
        It should_decompose_the_correct_sequence_number = () => decomposed_sequence_number.ShouldEqual(sequence_number);
    }
}