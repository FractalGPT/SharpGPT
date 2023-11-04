using System;

namespace FractalGPT.SharpGPTLib.MLWithLLM.TaskManagers
{
    /// <summary>
    /// Represents a task with a unique identifier, a description, and a priority level.
    /// Priority is represented as a double, where higher values signify higher priority.
    /// </summary>
    [Serializable] // This attribute indicates that instances of PriorityTask can be serialized.
    public class PriorityTask : IComparable<PriorityTask>
    {
        // Unique identifier for the task.
        public Guid Id { get; private set; }

        // A brief description of the task.
        public string TaskDescription { get; private set; }

        // The priority level of the task. Higher values represent higher priority.
        public double Priority { get; private set; }

        /// <summary>
        /// Initializes a new instance of the PriorityTask class with a specified description and priority.
        /// A unique identifier is automatically assigned.
        /// </summary>
        /// <param name="description">A brief description of the task.</param>
        /// <param name="priority">The priority level of the task.</param>
        public PriorityTask(string description, double priority)
        {
            Id = Guid.NewGuid(); // Assign a unique identifier.
            TaskDescription = description; // Set the task description.
            Priority = priority; // Set the task priority.
        }

        /// <summary>
        /// Compares the current instance with another PriorityTask object and returns an integer that
        /// indicates whether the current instance precedes, follows, or occurs in the same position in
        /// the sort order as the other object.
        /// </summary>
        /// <param name="other">An object to compare with this instance.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has
        /// these meanings:
        /// - Less than zero: This instance precedes other in the sort order.
        /// - Zero: This instance occurs in the same position in the sort order as other.
        /// - Greater than zero: This instance follows other in the sort order.
        /// </returns>
        public int CompareTo(PriorityTask other)
        {
            if (other == null) return 1; // If other is null, this instance is greater.

            // Higher priority tasks should come first, so we reverse the comparison.
            return other.Priority.CompareTo(this.Priority);
        }
    }
}
