using System;
using System.Collections.Generic;

namespace FractalGPT.SharpGPTLib.MLWithLLM.TaskManagers
{
    [Serializable]
    public class PriorityQueue<TElement, TPriority> where TPriority : IComparable<TPriority>
    {
        private List<(TElement Element, TPriority Priority)> _elements = new List<(TElement, TPriority)>();

        /// <summary>
        /// Adds an element with the provided priority to the queue.
        /// </summary>
        /// <param name="element">The element to add to the queue.</param>
        /// <param name="priority">The priority of the element.</param>
        public void Enqueue(TElement element, TPriority priority)
        {
            _elements.Add((element, priority));
            _elements.Sort((x, y) => y.Priority.CompareTo(x.Priority));
        }

        /// <summary>
        /// Removes and returns the element with the highest priority.
        /// </summary>
        /// <returns>The element with the highest priority.</returns>
        public TElement Dequeue()
        {
            if (_elements.Count == 0)
            {
                throw new InvalidOperationException("The priority queue is empty.");
            }
            var item = _elements[0];
            _elements.RemoveAt(0);
            return item.Element;
        }

        /// <summary>
        /// Returns the element with the highest priority without removing it.
        /// </summary>
        /// <returns>The element with the highest priority.</returns>
        public TElement Peek()
        {
            if (_elements.Count == 0)
            {
                throw new InvalidOperationException("The priority queue is empty.");
            }
            return _elements[0].Element;
        }

        /// <summary>
        /// Gets the number of elements in the queue.
        /// </summary>
        public int Count => _elements.Count;

        /// <summary>
        /// Clears all the elements in the queue.
        /// </summary>
        public void Clear() => _elements.Clear();

        /// <summary>
        /// Tries to return the element with the highest priority without removing it.
        /// </summary>
        /// <param name="element">The element with the highest priority.</param>
        /// <param name="priority">The priority of the element.</param>
        /// <returns>True if the operation was successful; otherwise, false.</returns>
        public bool TryPeek(out TElement element, out TPriority priority)
        {
            if (_elements.Count > 0)
            {
                var item = _elements[0];
                element = item.Element;
                priority = item.Priority;
                return true;
            }
            element = default;
            priority = default;
            return false;
        }

        /// <summary>
        /// Tries to remove and return the element with the highest priority.
        /// </summary>
        /// <param name="element">The element with the highest priority.</param>
        /// <param name="priority">The priority of the element.</param>
        /// <returns>True if the operation was successful; otherwise, false.</returns>
        public bool TryDequeue(out TElement element, out TPriority priority)
        {
            if (_elements.Count > 0)
            {
                var item = _elements[0];
                _elements.RemoveAt(0);
                element = item.Element;
                priority = item.Priority;
                return true;
            }
            element = default;
            priority = default;
            return false;
        }
    }
}
