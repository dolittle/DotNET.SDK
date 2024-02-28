// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;

namespace Dolittle.SDK.Projections.Core;

public class MissingProjectionAttribute(MemberInfo projectionType) : Exception($"Missing [Projection] attribute on {projectionType.Name}")
{
}
