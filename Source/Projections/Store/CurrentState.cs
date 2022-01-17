// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.SDK.Projections.Store;

/// <summary>
/// Represents the current projection state.
/// </summary>
/// <typeparam name="TProjection">The <see cref="System.Type" /> of the projection.</typeparam>
public class CurrentState<TProjection>
    where TProjection : class, new()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CurrentState{TProjection}"/> class.
    /// </summary>
    /// <param name="state">The current <typeparamref name="TProjection"/> state.</param>
    /// <param name="type">The <see cref="CurrentStateType" />.</param>
    /// <param name="key">The <see cref="Key" />.</param>
    public CurrentState(TProjection state, CurrentStateType type, Key key)
    {
        State = state;
        WasCreatedFromInitialState = type switch
        {
            CurrentStateType.CreatedFromInitialState => true,
            _ => false
        };
        Key = key;
    }

    /// <summary>
    /// Gets a value indicating whether the state was created from initial state.
    /// </summary>
    public bool WasCreatedFromInitialState { get; }

    /// <summary>
    /// Gets the state.
    /// </summary>
    public TProjection State { get; }

    /// <summary>
    /// Gets the <see cref="Key" />.
    /// </summary>
    public Key Key { get; }

    /// <summary>
    /// Implicitly converts a <see cref="CurrentState{TProjection}" /> to the underlying <typeparamref name="TProjection"/>.
    /// </summary>
    /// <param name="currentState">The <see cref="CurrentState{TProjection}" /> to convert.</param>
    public static implicit operator TProjection(CurrentState<TProjection> currentState) => currentState.State;
}