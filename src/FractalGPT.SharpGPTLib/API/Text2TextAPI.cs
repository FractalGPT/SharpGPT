using System.Threading.Tasks;

namespace FractalGPT.SharpGPTLib.API
{
    /// <summary>
    /// The task of transforming text to text
    /// </summary>
    public interface Text2TextAPI
    {
        /// <summary>
        /// Main asynchronous handler
        /// </summary>
        /// <param name="text">Input text</param>
        /// <returns></returns>
        Task<string> SendAsyncReturnText(string text);

        /// <summary>
        /// Main handler
        /// </summary>
        /// <param name="text">Input text</param>
        /// <returns></returns>
        string SendReturnText(string text);
    }
}
