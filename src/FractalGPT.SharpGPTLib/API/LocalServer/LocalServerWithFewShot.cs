using FractalGPT.SharpGPTLib.Prompts.FewShot;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FractalGPT.SharpGPTLib.API.LocalServer
{
    /// <summary>
    /// Represents a local server configuration that incorporates few-shot learning capabilities 
    /// using a base language model server API and a few-shot manager.
    /// </summary>
    [Serializable]
    public class LocalServerWithFewShot
    {
        /// <summary>
        /// Gets or sets the base language model server API used for text generation.
        /// </summary>
        public BaseLLMServerAPI BaseLLMServer { get; set; }

        /// <summary>
        /// Gets or sets the few-shot manager responsible for managing few-shot learning examples.
        /// </summary>
        public FewShotManager FewShotManagerFromServer { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalServerWithFewShot"/> class.
        /// </summary>
        public LocalServerWithFewShot() 
        {
            FewShotManagerFromServer = new FewShotManager();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalServerWithFewShot"/> class with 
        /// specified base language model server API and few-shot manager.
        /// </summary>
        /// <param name="baseLLMServerAPI">The base language model server API.</param>
        /// <param name="fewShotManagerFromServer">The few-shot manager.</param>
        public LocalServerWithFewShot(BaseLLMServerAPI baseLLMServerAPI, FewShotManager fewShotManagerFromServer)
        {
            BaseLLMServer = baseLLMServerAPI;
            FewShotManagerFromServer = fewShotManagerFromServer;
        }

        /// <summary>
        /// Asynchronously sends a prompt to the language model server for text generation, optionally incorporating few-shot learning examples.
        /// </summary>
        /// <param name="prompt">The text prompt to be sent for generation.</param>
        /// <param name="generationParametrs">Optional parameters to customize the generation process.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation, with a result of the generated text.</returns>
        public async Task<string> SendAsync(string prompt, GenerationParametrs generationParametrs = null)
        {
            string few = FewShotManagerFromServer.ToString();
            string input = $"{few}{FewShotManagerFromServer.StartToken}{prompt}{FewShotManagerFromServer.Sep}";
            string output = await BaseLLMServer.TextGeneration(input, generationParametrs);
            return output;
        }
    }

}
