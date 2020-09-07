// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;
using Dolittle.SDK.Security;
using Dolittle.SDK.Tenancy;

namespace Dolittle.SDK.Execution
{
    /// <summary>
    /// Defines the manager for <see cref="ExecutionContext"/>.
    /// </summary>
    public interface IExecutionContextManager
    {
        /// <summary>
        /// Gets the current <see cref="ExecutionContext"/>.
        /// </summary>
        ExecutionContext Current { get; }

        /// <summary>
        /// Set current execution context for a <see cref="TenantId"/>.
        /// </summary>
        /// <param name="tenant"><see cref="TenantId"/> to set for.</param>
        /// <param name="filePath">FilePath of the caller.</param>
        /// <param name="lineNumber">Linenumber in the file of the caller.</param>
        /// <param name="member">Membername of the caller.</param>
        /// <returns>Current <see cref="IExecutionContextManager"/>.</returns>
        IExecutionContextManager ForTenant(TenantId tenant, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string member = "");

        /// <summary>
        /// Set current execution context for a <see cref="CorrelationId"/>.
        /// </summary>
        /// <param name="correlationId"><see cref="CorrelationId"/> to associate.</param>
        /// <param name="filePath">FilePath of the caller.</param>
        /// <param name="lineNumber">Linenumber in the file of the caller.</param>
        /// <param name="member">Membername of the caller.</param>
        /// <returns>The <see cref="IExecutionContextManager"/> for continuation.</returns>
        IExecutionContextManager ForCorrelation(CorrelationId correlationId, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string member = "");

        /// <summary>
        /// Set current execution context for a <see cref="TenantId"/> with <see cref="CorrelationId"/> and <see cref="Claims"/>.
        /// </summary>
        /// <param name="claims"><see cref="Claims"/> to associate.</param>
        /// <param name="filePath">FilePath of the caller.</param>
        /// <param name="lineNumber">Linenumber in the file of the caller.</param>
        /// <param name="member">Membername of the caller.</param>
        /// <returns>Current <see cref="ExecutionContext"/>.</returns>
        IExecutionContextManager ForClaims(Claims claims, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string member = "");
    }
}