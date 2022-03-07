// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.SDK.Projections.Store;

/// <summary>
/// Represents the current state of a projection read model.
/// </summary>
/// <typeparam name="TReadModel">The <see cref="System.Type" /> of the projection.</typeparam>
public class CurrentState<TReadModel>
    where TReadModel : class, new()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CurrentState{TReadModel}"/> class.
    /// </summary>
    /// <param name="state">The current <typeparamref name="TReadModel"/> state.</param>
    /// <param name="type">The <see cref="CurrentStateType" />.</param>
    /// <param name="key">The <see cref="Key" />.</param>
    public CurrentState(TReadModel state, CurrentStateType type, Key key)
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
    /// Gets the current state of the projection read model.
    /// </summary>
    public TReadModel State { get; }

    /// <summary>
    /// Gets the <see cref="Key" />.
    /// </summary>
    public Key Key { get; }

    /// <summary>
    /// Implicitly converts a <see cref="CurrentState{TReadModel}" /> to the underlying <typeparamref name="TReadModel"/>.
    /// </summary>
    /// <param name="currentState">The <see cref="CurrentState{TReadModel}" /> to convert.</param>
    public static implicit operator TReadModel(CurrentState<TReadModel> currentState) => currentState.State;
}
