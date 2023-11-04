using System;

namespace FractalGPT.SharpGPTLib.MLWithLLM.TaskManagers
{
    /// <summary>
    /// Manages a thread-safe priority queue of tasks.
    /// </summary>
    [Serializable]
    public class TaskManager
    {
        private readonly object _syncLock = new object();
        private PriorityQueue<PriorityTask, double> _taskQueue;

        public TaskManager()
        {
            _taskQueue = new PriorityQueue<PriorityTask, double>();
        }

        /// <summary>
        /// Adds a new task to the queue with the given priority.
        /// </summary>
        /// <param name="taskDescription">Description of the task.</param>
        /// <param name="priority">Priority of the task.</param>
        public void AddTask(string taskDescription, double priority)
        {
            lock (_syncLock)
            {
                PriorityTask newTask = new PriorityTask(taskDescription, priority);
                _taskQueue.Enqueue(newTask, newTask.Priority);
            }
        }

        /// <summary>
        /// Gets the next task in the queue without removing it.
        /// </summary>
        /// <returns>The next task.</returns>
        public PriorityTask PeekNextTask()
        {
            lock (_syncLock)
            {
                if (_taskQueue.TryPeek(out PriorityTask nextTask, out double priority))
                {
                    return nextTask;
                }
            }

            return null;
        }

        /// <summary>
        /// Removes and returns the next task in the queue.
        /// </summary>
        /// <returns>The next task.</returns>
        public PriorityTask GetNextTask()
        {
            lock (_syncLock)
            {
                if (_taskQueue.TryDequeue(out PriorityTask nextTask, out double priority))
                {
                    return nextTask;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the current number of tasks in the queue.
        /// </summary>
        /// <returns>The number of tasks.</returns>
        public int GetTaskCount()
        {
            lock (_syncLock)
            {
                return _taskQueue.Count;
            }
        }

        /// <summary>
        /// Clears all tasks from the queue.
        /// </summary>
        public void ClearTasks()
        {
            lock (_syncLock)
            {
                _taskQueue.Clear();
            }
        }
    }
}
