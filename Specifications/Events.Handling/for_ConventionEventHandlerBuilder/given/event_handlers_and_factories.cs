// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.DependencyInversion;
using Machine.Specifications;

namespace Dolittle.Events.Handling.for_ConventionEventHandlerBuilder.given
{
    public class event_handlers_and_factories
    {
        public static EventHandler event_handler;
        public static FactoryFor<EventHandler> event_handler_factory;
        public static EventHandlerWithCorrectPublicMethodSignatureButWrongName event_handler_method_with_correct_public_signature_but_wrong_name;
        public static FactoryFor<EventHandlerWithCorrectPublicMethodSignatureButWrongName> event_handler_method_with_correct_public_signature_but_wrong_name_factory;
        public static EventHandlerWithCorrectPrivateMethodSignatureButWrongName event_handler_method_with_correct_private_signature_but_wrong_name;
        public static FactoryFor<EventHandlerWithCorrectPrivateMethodSignatureButWrongName> event_handler_method_with_correct_private_signature_but_wrong_name_factory;
        public static EventHandlerWithWrongFirstParameter event_handler_with_wrong_first_parameter;
        public static FactoryFor<EventHandlerWithWrongFirstParameter> event_handler_with_wrong_first_parameter_factory;
        public static EventHandlerWithWrongSecondParameter event_handler_with_wrong_second_parameter;
        public static FactoryFor<EventHandlerWithWrongSecondParameter> event_handler_with_wrong_second_parameter_factory;
        public static EventHandlerWithExtraParameters event_handler_with_extra_parameters;
        public static FactoryFor<EventHandlerWithExtraParameters> event_handler_with_extra_parameters_factory;
        public static EventHandlerThatDoesNotReturnTask event_handler_that_does_not_return_task;
        public static FactoryFor<EventHandlerThatDoesNotReturnTask> event_handler_that_does_not_return_task_factory;
        public static ExternalEventHandlerWithPrivateEvent external_event_handler_with_private_event;
        public static FactoryFor<ExternalEventHandlerWithPrivateEvent> external_event_handler_with_private_event_factory;

        Establish context = () =>
        {
            event_handler = new EventHandler();
            event_handler_factory = () => event_handler;
            event_handler_method_with_correct_public_signature_but_wrong_name = new EventHandlerWithCorrectPublicMethodSignatureButWrongName();
            event_handler_method_with_correct_public_signature_but_wrong_name_factory = () => event_handler_method_with_correct_public_signature_but_wrong_name;
            event_handler_method_with_correct_private_signature_but_wrong_name = new EventHandlerWithCorrectPrivateMethodSignatureButWrongName();
            event_handler_method_with_correct_private_signature_but_wrong_name_factory = () => event_handler_method_with_correct_private_signature_but_wrong_name;
            event_handler_with_wrong_first_parameter = new EventHandlerWithWrongFirstParameter();
            event_handler_with_wrong_first_parameter_factory = () => event_handler_with_wrong_first_parameter;
            event_handler_with_wrong_second_parameter = new EventHandlerWithWrongSecondParameter();
            event_handler_with_wrong_second_parameter_factory = () => event_handler_with_wrong_second_parameter;
            event_handler_with_extra_parameters = new EventHandlerWithExtraParameters();
            event_handler_with_extra_parameters_factory = () => event_handler_with_extra_parameters;
            event_handler_that_does_not_return_task = new EventHandlerThatDoesNotReturnTask();
            event_handler_that_does_not_return_task_factory = () => event_handler_that_does_not_return_task;
            external_event_handler_with_private_event = new ExternalEventHandlerWithPrivateEvent();
            external_event_handler_with_private_event_factory = () => external_event_handler_with_private_event;
        };
    }
}