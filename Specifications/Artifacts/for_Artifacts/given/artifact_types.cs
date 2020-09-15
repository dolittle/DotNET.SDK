// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Artifacts.for_Artifacts.given
{
    public class artifact_types : Artifacts<artifact_type>
    {
        public artifact_types()
            : base(Moq.Mock.Of<ILogger<Artifacts<artifact_type>>>())
        {
        }
    }
}
