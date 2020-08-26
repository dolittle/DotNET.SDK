// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Logging;
using Dolittle.TimeSeries.Connectors;
using Dolittle.TimeSeries.DataPoints;
using Single = Dolittle.TimeSeries.DataTypes.Single;

namespace PullConnector
{
    public class MyConnector : IAmAPullConnector
    {
        readonly ILogger _logger;
        readonly Random _random;

        public MyConnector(ILogger logger)
        {
            _logger = logger;
            _random = new Random();
        }

        public Source Name => "MyPullConnector";

        public Task<IEnumerable<TagDataPoint>> Pull()
        {
            var tags = new [] {"first", "second", "third"};

            _logger.Information(
                "Pulling tags '{tags}'",
                string.Join(", ", tags.Select(_ => _)));
            return Task.FromResult(tags.Select(_ => new TagDataPoint(_, (Single)_random.NextDouble())));
        }
    }
}