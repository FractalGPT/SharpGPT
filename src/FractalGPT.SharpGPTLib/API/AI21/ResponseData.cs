using System;
using System.Collections.Generic;

namespace FractalGPT.SharpGPTLib.API.AI21
{
    /// <summary>
    /// Represents the response structure containing a list of completions.
    /// </summary>
    [Serializable]
    public class ResponseData
    {
        /// <summary>
        /// Gets or sets the list of completions received in the response.
        /// </summary>
        public List<Completion> Completions { get; set; }
    }

}
