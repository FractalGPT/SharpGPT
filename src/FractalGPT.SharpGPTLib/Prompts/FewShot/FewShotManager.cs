using System.Text;

namespace FractalGPT.SharpGPTLib.Prompts.FewShot
{
    /// <summary>
    /// Manages a collection of <see cref="FewShotElement"/> objects, providing functionality to aggregate their content
    /// with customizable separators and tokens.
    /// </summary>
    [Serializable]
    public class FewShotManager
    {
        /// <summary>
        /// The list of <see cref="FewShotElement"/> objects managed by this instance.
        /// </summary>
        public List<FewShotElement> FewShots = new List<FewShotElement>();

        /// <summary>
        /// Gets or sets the end token used to denote the end of a model's output. Default is "</s>".
        /// </summary>
        public string EndToken { get; set; } = "</s>";

        /// <summary>
        /// Gets or sets the start token used to denote the beginning of a model's output. Default is "<s>".
        /// </summary>
        public string StartToken { get; set; } = "<s>";

        /// <summary>
        /// Gets or sets the separator used between the prompt and the model output within a single <see cref="FewShotElement"/>.
        /// Default is a newline character.
        /// </summary>
        public string Sep { get; set; } = "\n";

        /// <summary>
        /// Gets or sets the separator used between different <see cref="FewShotElement"/> objects when aggregating their content.
        /// This property needs to be set explicitly as it does not have a default value.
        /// </summary>
        public string SepShots { get; set; } = "\n";

        /// <summary>
        /// Initializes a new instance of the <see cref="FewShotManager"/> class with an empty list of <see cref="FewShotElement"/> objects.
        /// </summary>
        public FewShotManager() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FewShotManager"/> class, populating it with a collection of <see cref="FewShotElement"/> objects.
        /// </summary>
        /// <param name="fewShotElements">The collection of <see cref="FewShotElement"/> objects to initialize the manager with.</param>
        public FewShotManager(IEnumerable<FewShotElement> fewShotElements)
        {
            FewShots = fewShotElements.ToList();
        }

        /// <summary>
        /// Returns a string that represents the current object, specifically aggregating the content of all managed <see cref="FewShotElement"/> objects
        /// using the configured tokens and separators.
        /// </summary>
        /// <returns>A string representation of the aggregated content of all <see cref="FewShotElement"/> objects managed by this instance.</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var element in FewShots)
            {
                _ = sb.Append(element.GetString(StartToken, EndToken, Sep));
                _ = sb.Append(SepShots);
            }

            return sb.ToString().TrimEnd(SepShots.ToCharArray()); // Optionally trim the trailing separator for cleaner output
        }
    }

}
