using System.Text.Json.Serialization;

namespace FractalGPT.SharpGPTLib.API.LLMAPI;

/// <summary>
/// Represents the data sent to an LLM (Large Language Model), including messages and request parameters.
/// </summary>
[Serializable]
public class SendDataLLM
{
    private readonly int bufferSize;
    private int currentIndex;

    /// <summary>
    /// Gets the name of the LLM model.
    /// </summary>
    [JsonPropertyName("model")]
    public string ModelName { get; }

    /// <summary>
    /// Gets the temperature value for text generation (degree of randomness).
    /// </summary>
    [JsonPropertyName("temperature")]
    public double Temperature { get; }

    [JsonPropertyName("repetition_penalty")]
    public double RepetitionPenalty { get; set; }

    [JsonPropertyName("top_p")]
    public double TopP { get; set; }

    [JsonPropertyName("top_k")]
    public int TopK { get; set; }

    [JsonPropertyName("min_tokens")]
    public int MinTokens { get; set; }
    
    [JsonPropertyName("max_tokens")]
    public int MaxTokens { get; set; }

    /// <summary>
    /// Gets or sets the system prompt used at the beginning of every message exchange.
    /// This property is not serialized because it is included as part of the initial messages.
    /// </summary>
    [JsonIgnore]
    public string Prompt { get; set; }

    /// <summary>
    /// Gets the list of messages to be sent to the LLM.
    /// Messages are stored in chronological order with a fixed size limit.
    /// </summary>
    [JsonPropertyName("messages")]
    public List<LLMMessage> Messages { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SendDataLLM"/> class with the specified model name, 
    /// system prompt, buffer size, and temperature.
    /// </summary>
    /// <param name="modelName">The name of the LLM model to use.</param>
    /// <param name="systemPrompt">The system text that initializes the conversation context.</param>
    /// <param name="bufferSize">The maximum number of messages to keep in the conversation.</param>
    /// <param name="temperature">Controls the randomness or creativity of the LLM's output.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="modelName"/> or <paramref name="systemPrompt"/> is null or empty.
    /// </exception>
    public SendDataLLM(string modelName,
        string systemPrompt,
        int bufferSize = 5,
        double temperature = 0.1,
        int topK = 5,
        double topP = 0.8,
        double repetitionPenalty = 1.00,
        int maxTokens = 2048,
        int minTokens = 10)
    {
        if (string.IsNullOrWhiteSpace(modelName))
            throw new ArgumentNullException(nameof(modelName), "Model name cannot be null or empty.");

        if (string.IsNullOrWhiteSpace(systemPrompt))
            throw new ArgumentNullException(nameof(systemPrompt), "System prompt cannot be null or empty.");

        ModelName = modelName;
        Temperature = temperature;
        TopK = topK;
        TopP = topP;
        RepetitionPenalty = repetitionPenalty;
        Prompt = systemPrompt;
        MaxTokens = maxTokens;
        MinTokens = minTokens;

        this.bufferSize = bufferSize;
        Messages = new List<LLMMessage>(bufferSize)
        {
            // Initialize the message list with the initial system message.
            new LLMMessage("system", systemPrompt)
        };
        currentIndex = 1;
    }

    /// <summary>
    /// Загрузка сообщений
    /// </summary>
    /// <param name="messages"></param>
    public void SetMessages(IEnumerable<LLMMessage> messages)
    {
        Messages.Clear();
        Messages.AddRange(messages);
    }

    /// <summary>
    /// Adds a user message to the conversation.
    /// </summary>
    /// <param name="text">The text of the user message.</param>
    public void AddUserMessage(string text)
    {
        AddMessage("user", text);
    }

    /// <summary>
    /// Adds an assistant message to the conversation.
    /// </summary>
    /// <param name="text">The text of the assistant message.</param>
    public void AddAssistantMessage(string text)
    {
        AddMessage("assistant", text);
    }

    /// <summary>
    /// Clears the current list of messages, resetting it to the initial system message.
    /// </summary>
    public void Clear()
    {
        Messages.Clear();
        Messages.Add(new LLMMessage("system", Prompt));
        currentIndex = 1;
    }

    /// <summary>
    /// Adds a new message to the list, maintaining the size limit. 
    /// If the limit is reached, the oldest message is removed.
    /// </summary>
    /// <param name="role">The role of the message sender (e.g., 'system', 'user', or 'assistant').</param>
    /// <param name="text">The text of the message.</param>
    private void AddMessage(string role, string text)
    {
        // If the buffer size is reached, remove the oldest message.
        if (currentIndex >= bufferSize)
        {
            Messages.RemoveAt(0);
        }
        else
        {
            currentIndex++;
        }

        Messages.Add(new LLMMessage(role, text));
    }
}
