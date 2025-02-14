namespace FractalGPT.SharpGPTLib.Prompts.PersonaChat;

/// <summary>
/// Represents a segment of dialogue, maintaining a buffer of the latest messages.
/// This class is designed to manage conversations by keeping track of a fixed number of recent messages.
/// </summary>
[Serializable]
public class PersonaContext
{
    /// <summary>
    /// Gets or sets the tag preceding messages from the user.
    /// </summary>
    public string UserTag { get; set; } = "Ты:";

    /// <summary>
    /// Gets or sets the tag preceding messages from the bot.
    /// </summary>
    public string BotTag { get; set; } = "Я:";

    private readonly int bufferSize;
    private int currentIndex;

    /// <summary>
    /// Gets the list of messages in chronological order, with a limit on the number of messages.
    /// </summary>
    public List<string> Messages { get; private set; }

    /// <summary>
    /// Initializes a new instance of the PersonaContext class with a specified buffer size for the messages.
    /// </summary>
    /// <param name="bufferSize">The maximum number of messages the list can hold.</param>
    public PersonaContext(int bufferSize = 5)
    {
        this.bufferSize = bufferSize;
        Messages = new List<string>(bufferSize);
        currentIndex = 1;
    }

    /// <summary>
    /// Adds a user message to the list of messages.
    /// </summary>
    /// <param name="text">The text of the user's message.</param>
    public void AddUserMessage(string text)
    {
        AddMessage("user", text);
    }

    /// <summary>
    /// Adds a bot message to the list of messages.
    /// </summary>
    /// <param name="text">The text of the bot's (assistant's) message.</param>
    public void AddAssistantMessage(string text)
    {
        AddMessage("bot", text);
    }

    /// <summary>
    /// Adds a new message to the list, maintaining its size within the specified limit.
    /// Older messages are removed to make room for new ones when the maximum size is reached.
    /// </summary>
    /// <param name="role">The role in the conversation (user or assistant).</param>
    /// <param name="text">The text of the message.</param>
    public void AddMessage(string role, string text)
    {
        // Removes the oldest message if the buffer size is reached
        if (currentIndex >= bufferSize)
            Messages.RemoveAt(0);
        else
            currentIndex++;

        string tag = role == "user" ? UserTag : BotTag;
        Messages.Add($"{tag} {text}");
    }

    /// <summary>
    /// Clears the current list of messages, resetting it to its initial state.
    /// </summary>
    public void Clear()
    {
        Messages.Clear();
        currentIndex = 1;
    }
}
