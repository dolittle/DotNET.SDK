// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// See https://www.mking.net/blog/error-cs0518-isexternalinit-not-defined
#if NETCOREAPP3_0 || NETCOREAPP3_1

using System.ComponentModel;

namespace System.Runtime.CompilerServices;

/// <summary>
/// Reserved to be used by the compiler for tracking metadata.
/// This class should not be used by developers in source code.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
static class IsExternalInit
{
}

#endif
