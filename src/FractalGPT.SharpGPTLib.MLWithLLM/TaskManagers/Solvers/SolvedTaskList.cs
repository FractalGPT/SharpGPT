using System;
using System.Collections.Generic;

namespace FractalGPT.SharpGPTLib.MLWithLLM.TaskManagers.Solvers
{
    /// <summary>
    /// Class for storing solved tasks.
    /// </summary>
    [Serializable]
    public class SolvedTaskList
    {
        private List<PriorityTask> _tasks = new List<PriorityTask>();

        /// <summary>
        /// Adds a task to the list of solved tasks.
        /// </summary>
        /// <param name="task">The solved task.</param>
        public void Add(PriorityTask task)
        {
            _tasks.Add(task);
        }

        /// <summary>
        /// Retrieves the list of solved tasks.
        /// </summary>
        /// <returns>The list of solved tasks.</returns>
        public List<PriorityTask> GetTasks()
        {
            return _tasks;
        }
    }
}