using System.Collections.Generic;
using System.Linq;

namespace FractalGPT.SharpGPTLib.MLWithLLM.AutoDescription
{
    /// <summary>
    /// Class to manage a collection of VarDescription objects.
    /// </summary>
    public class VarDescriptionManager
    {
        private readonly Dictionary<string, VarDescription> _varDescriptions = new Dictionary<string, VarDescription>();

        /// <summary>
        /// Initializes a new instance of the VarDescriptionManager class.
        /// </summary>
        public VarDescriptionManager() { }

        /// <summary>
        /// Initializes a new instance of the VarDescriptionManager class with an existing collection of VarDescription objects.
        /// </summary>
        /// <param name="varDescriptions">An IEnumerable collection of VarDescription objects to initialize the manager with.</param>
        public VarDescriptionManager(IEnumerable<VarDescription> varDescriptions)
        {
            foreach (var varDescription in varDescriptions)
            {
                _varDescriptions.Add(varDescription.NameVar, varDescription);
            }
        }

        /// <summary>
        /// Gets the collection of VarDescription objects as an IEnumerable.
        /// </summary>
        public IEnumerable<VarDescription> VarDescriptions => _varDescriptions.Values;

        /// <summary>
        /// Adds a VarDescription object to the collection.
        /// </summary>
        /// <param name="varDescription">The VarDescription object to add.</param>
        /// <returns>True if the variable was added; otherwise, false.</returns>
        public bool AddVarDescription(VarDescription varDescription)
        {
            if (varDescription == null || _varDescriptions.ContainsKey(varDescription.NameVar))
            {
                return false;
            }

            _varDescriptions.Add(varDescription.NameVar, varDescription);
            return true;
        }

        /// <summary>
        /// Removes a VarDescription object from the collection by its name.
        /// </summary>
        /// <param name="name">The name of the variable to remove.</param>
        /// <returns>True if the variable was removed; otherwise, false.</returns>
        public bool RemoveVarDescriptionByName(string name)
        {
            return _varDescriptions.Remove(name);
        }

        /// <summary>
        /// Checks if a variable with a specified name exists in the collection.
        /// </summary>
        /// <param name="name">The name of the variable to check.</param>
        /// <returns>True if the variable exists; otherwise, false.</returns>
        public bool ContainsVarDescription(string name)
        {
            return _varDescriptions.ContainsKey(name);
        }

        /// <summary>
        /// Gets a list of all variable names in the collection.
        /// </summary>
        /// <returns>A list of variable names.</returns>
        public List<string> GetVarNames()
        {
            return _varDescriptions.Keys.ToList();
        }

        /// <summary>
        /// Retrieves a VarDescription object by its name.
        /// </summary>
        /// <param name="name">The name of the variable to retrieve.</param>
        /// <returns>The VarDescription object if found; otherwise, null.</returns>
        public VarDescription GetVarDescriptionByName(string name)
        {
            _varDescriptions.TryGetValue(name, out VarDescription varDescription);
            return varDescription;
        }

        /// <summary>
        /// Updates an existing VarDescription object in the collection.
        /// </summary>
        /// <param name="varDescription">The updated VarDescription object.</param>
        /// <returns>True if the variable was updated; otherwise, false.</returns>
        public bool UpdateVarDescription(VarDescription varDescription)
        {
            if (varDescription == null || !_varDescriptions.ContainsKey(varDescription.NameVar))
            {
                return false;
            }

            _varDescriptions[varDescription.NameVar] = varDescription;
            return true;
        }
    }

}
