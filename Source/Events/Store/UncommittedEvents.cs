// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.Generic;

namespace Dolittle.SDK.Events.Store
{
    /// <summary>
    /// Represents a sequence of <see cref="UncommittedEvent"/>s that have not been committed to the Event Store.
    /// </summary>
    public class UncommittedEvents : IList<UncommittedEvent>
    {
        readonly List<UncommittedEvent> _events = new List<UncommittedEvent>();

        /// <summary>
        /// Gets a value indicating whether or not there are any events in the committed sequence.
        /// </summary>
        public bool HasEvents => Count > 0;

        /// <inheritdoc/>
        public int Count => _events.Count;

        /// <inheritdoc/>
        public bool IsReadOnly => false;

        /// <inheritdoc/>
        public UncommittedEvent this[int index]
        {
            get => _events[index];
            set => Insert(index, value);
        }

        /// <inheritdoc/>
        public IEnumerator<UncommittedEvent> GetEnumerator() => _events.GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => _events.GetEnumerator();

        /// <inheritdoc/>
        public int IndexOf(UncommittedEvent item) => _events.IndexOf(item);

        /// <inheritdoc/>
        public void Insert(int index, UncommittedEvent item)
        {
            ThrowIfEventIsNull(item);
            _events.Insert(index, item);
        }

        /// <inheritdoc/>
        public void RemoveAt(int index) => _events.RemoveAt(index);

        /// <inheritdoc/>
        public void Add(UncommittedEvent item)
        {
            ThrowIfEventIsNull(item);
            _events.Add(item);
        }

        /// <inheritdoc/>
        public void Clear() => _events.Clear();

        /// <inheritdoc/>
        public bool Contains(UncommittedEvent item) => _events.Contains(item);

        /// <inheritdoc/>
        public void CopyTo(UncommittedEvent[] array, int arrayIndex)
        {
            foreach (var item in array)
            {
                ThrowIfEventIsNull(item);
            }

            _events.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc/>
        public bool Remove(UncommittedEvent item) => _events.Remove(item);

        void ThrowIfEventIsNull(UncommittedEvent @event)
        {
            if (@event == null) throw new EventCanNotBeNull();
        }
    }
}
