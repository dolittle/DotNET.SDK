// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq.Expressions;

namespace Dolittle.SDK.Projections.Builder;

/// <summary>
/// Exception that gets thrown when trying to construct a property key selector from an expression that is not a <see cref="MemberExpression"/>.
/// </summary>
public class KeySelectorExpressionWasNotAMemberExpression : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KeySelectorExpressionWasNotAMemberExpression"/> class.
    /// </summary>
    public KeySelectorExpressionWasNotAMemberExpression()
        : base("The key selector expression was not a member expression")
    {
    }
}
