using System;
using System.Text.Json.Serialization;

namespace FractalGPT.SharpGPTLib.API.LocalServer.LocalServerAnswer;

[Serializable]
public class TextGenerationJSON
{
    [JsonPropertyName("answer")]
    public string Answer { get; set; }
}
