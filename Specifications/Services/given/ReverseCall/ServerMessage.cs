// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Services.Contracts;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace Dolittle.SDK.Services.given.ReverseCall
{
    public class ServerMessage : IMessage
    {
        public MessageDescriptor Descriptor => FileDescriptor.DescriptorProtoFileDescriptor.MessageTypes[0];

        public int CalculateSize() => 0;

        public void MergeFrom(CodedInputStream input) { }

        public void WriteTo(CodedOutputStream output) { }

        public ConnectResponse Response { get; set; }

        public Request Request { get; set; }

        public Ping Ping { get; set; }
    }
}