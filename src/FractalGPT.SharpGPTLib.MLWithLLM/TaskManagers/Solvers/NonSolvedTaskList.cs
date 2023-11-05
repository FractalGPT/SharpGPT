using System;
using System.Collections.Generic;

namespace FractalGPT.SharpGPTLib.MLWithLLM.TaskManagers.Solvers
{
    /// <summary>
    /// Class for storing unsolved tasks.
    /// </summary>
    [Serializable]
    public class NonSolvedTaskList
    {
        private List<PriorityTask> _tasks = new List<PriorityTask>();

        /// <summary>
        /// Adds a task to the list of unsolved tasks.
        /// </summary>
        /// <param name="task">The unsolved task.</param>
        public void Add(PriorityTask task)
        {
            _tasks.Add(task);
        }

        /// <summary>
        /// Retrieves the list of unsolved tasks.
        /// </summary>
        /// <returns>The list of unsolved tasks.</returns>
        public List<PriorityTask> GetTasks()
        {
            return _tasks;
        }
    }
}