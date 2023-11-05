using System;
using System.Text;

namespace FractalGPT.SharpGPTLib.MLWithLLM.TaskManagers.Solvers
{
    /// <summary>
    /// An abstract class representing a base solver for tasks.
    /// </summary>
    [Serializable]
    public abstract class BaseSolver
    {
        /// <summary>
        /// The task manager responsible for queuing and storing tasks.
        /// </summary>
        private TaskManager _taskManager = new TaskManager();

        /// <summary>
        /// Class for storing solved tasks.
        /// </summary>
        private SolvedTaskList _solvedTaskList = new SolvedTaskList();

        /// <summary>
        /// Class for storing unsolved tasks.
        /// </summary>
        private NonSolvedTaskList _nonSolvedTaskList = new NonSolvedTaskList();

        /// <summary>
        /// Adds a task to the system with the specified description and priority.
        /// </summary>
        /// <param name="description">The task description.</param>
        /// <param name="priority">The task priority.</param>
        public void AddTask(string description, double priority = 1)
        {
            _taskManager.AddTask(description, priority);
        }

        /// <summary>
        /// Performs one step of task solving: retrieves a task and processes it.
        /// </summary>
        public void SolverStep()
        {
            var task = _taskManager.GetNextTask();
            var isSolveAndSolveResult = Processor(task);

            if (isSolveAndSolveResult.Item1)
                _solvedTaskList.Add(isSolveAndSolveResult.Item2);
            else
                _nonSolvedTaskList.Add(isSolveAndSolveResult.Item2);
        }

        /// <summary>
        /// An abstract method for processing a task, to be implemented in subclasses.
        /// </summary>
        /// <param name="task">The task to process.</param>
        /// <returns>A tuple containing the processing result and the task itself.</returns>
        public abstract (bool, PriorityTask) Processor(PriorityTask task);

        /// <summary>
        /// Returns the lists of solved and unsolved tasks.
        /// </summary>
        /// <returns>A tuple containing the lists of solved and unsolved tasks.</returns>
        public (SolvedTaskList, NonSolvedTaskList) GetTaskLists()
        {
            return (_solvedTaskList, _nonSolvedTaskList);
        }
    }
}