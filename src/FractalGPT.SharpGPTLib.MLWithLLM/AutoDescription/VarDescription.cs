using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace FractalGPT.SharpGPTLib.MLWithLLM.AutoDescription
{

    /// <summary>
    /// Class to describe a variable.
    /// </summary>
    [Serializable]
    [XmlRoot("VarDescription")]
    public class VarDescription
    {
        /// <summary>
        /// The name of the variable.
        /// </summary>
        [XmlElement("NameVar")]
        [JsonPropertyName("nameVar")]
        public string NameVar { get; set; }

        /// <summary>
        /// The linguistic value of the variable.
        /// </summary>
        [XmlElement("LigvValueVar")]
        [JsonPropertyName("ligvValueVar")]
        public string LigvValueVar { get; set; }

        /// <summary>
        /// The description of the variable.
        /// </summary>
        [XmlElement("DescriptionVar")]
        [JsonPropertyName("descriptionVar")]
        public string DescriptionVar { get; set; }

        /// <summary>
        /// Parameterless constructor for the class.
        /// </summary>
        public VarDescription() { }

        /// <summary>
        /// Constructor for the class with parameters.
        /// </summary>
        /// <param name="nameVar">The name of the variable.</param>
        /// <param name="ligvValueVar">The linguistic value of the variable.</param>
        /// <param name="descriptionVar">The description of the variable.</param>
        public VarDescription(string nameVar, string ligvValueVar, string descriptionVar)
        {
            NameVar = nameVar;
            LigvValueVar = ligvValueVar;
            DescriptionVar = descriptionVar;
        }

        /// <summary>
        /// Method to serialize the object to JSON format.
        /// </summary>
        /// <returns>A JSON string representation of the object.</returns>
        public string ToJSON()
        {
            return JsonSerializer.Serialize(this);
        }

        /// <summary>
        /// Method to deserialize the object from JSON format.
        /// </summary>
        /// <param name="json">A JSON string representation of the object.</param>
        /// <returns>An instance of VarDescription object.</returns>
        public static VarDescription FromJSON(string json)
        {
            return JsonSerializer.Deserialize<VarDescription>(json);
        }

        /// <summary>
        /// Overridden ToString method for obtaining a string representation of the object.
        /// </summary>
        /// <returns>A string representation of the object.</returns>
        public override string ToString()
        {
            return $"Name: {NameVar}, Value: {LigvValueVar}, Description: {DescriptionVar}";
        }
    }

}
