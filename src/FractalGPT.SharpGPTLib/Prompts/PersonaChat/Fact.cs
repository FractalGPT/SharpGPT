namespace FractalGPT.SharpGPTLib.Prompts.PersonaChat
{
    /// <summary>
    /// Represents a single fact about a persona in a persona chat context.
    /// This class is used to encapsulate and manage individual facts that define the character or attributes of a chatbot persona.
    /// </summary>
    [Serializable]
    public class Fact
    {
        /// <summary>
        /// Gets or sets the body of the fact.
        /// </summary>
        public string FactBody { get; set; }

        /// <summary>
        /// Initializes a new instance of the Fact class with a specified fact string.
        /// </summary>
        /// <param name="fact">The text of the fact.</param>
        public Fact(string fact)
        {
            FactBody = fact;
        }

        /// <summary>
        /// Converts a collection of fact strings into a list of Fact objects.
        /// </summary>
        /// <param name="factStrings">An enumerable collection of fact strings.</param>
        /// <returns>A list of Fact objects created from the provided fact strings.</returns>
        public static List<Fact> GetFacts(IEnumerable<string> factStrings)
        {
            List<Fact> facts = new List<Fact>();
            foreach (var fact in factStrings)
                facts.Add(new Fact(fact));

            return facts;
        }
    }
}

