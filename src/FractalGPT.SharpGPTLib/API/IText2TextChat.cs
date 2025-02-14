namespace FractalGPT.SharpGPTLib.API;

/// <summary>
/// The task of transforming text to text with chat model
/// </summary>
public interface IText2TextChat
{
    /// <summary>
    /// Main asynchronous handler
    /// </summary>
    /// <param name="text">Input text</param>
    /// <returns></returns>
    Task<string> SendReturnTextAsync(string text);

    /// <summary>
    /// Main handler
    /// </summary>
    /// <param name="text">Input text</param>
    /// <returns></returns>
    string SendReturnText(string text);

    /// <summary>
    /// Setting the prompt
    /// </summary>
    /// <param name="prompt"></param>
    void SetPrompt(string prompt);

    /// <summary>
    /// Clearing the context
    /// </summary>
    void ClearContext();
}
