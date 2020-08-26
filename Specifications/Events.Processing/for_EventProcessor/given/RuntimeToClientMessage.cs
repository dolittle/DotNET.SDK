// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace Dolittle.Events.Processing.for_EventProcessor.given
{
    public class RuntimeToClientMessage : IMessage
    {
        public MessageDescriptor Descriptor => null;

        public int CalculateSize() => 0;

        public void MergeFrom(CodedInputStream input)
        {
        }

        public void WriteTo(CodedOutputStream output)
        {
        }
    }
}