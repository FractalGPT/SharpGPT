namespace FractalGPT.SharpGPTLib.API.LLMAPI;

/// <summary>
/// Represents configuration settings for text generation, encapsulating model parameters and behavior.
/// </summary>
public class GenerateSettings
{

    private double _temperature;
    private double _repetitionPenalty;
    private double _topP;
    private int _topK;
    private int _maxTokens;
    private int _minTokens;
    private int? _numLogprobs;

    /// <summary>
    /// Gets the temperature controlling the randomness of the output. Higher values make output more creative, lower values make it more focused.
    /// Valid range: 0.0 to 2.0.
    /// </summary>
    public double Temperature
    {
        get => _temperature;
        set
        {
            _temperature = ValidateRange(value, 0.0, 2.0, nameof(Temperature));
        }
    }


    /// <summary>
    /// Top logists for each step
    /// Valid range: 1 to 20.
    /// </summary>
    public int? TopLogprobs
    {
        get => _numLogprobs;
        set
        {
            _numLogprobs = ValidateRange(value.Value, 1, 20, nameof(TopLogprobs));
        }
    }

    /// <summary>
    /// Whether to print logarithms of token probabilities
    /// </summary>
    public bool LogProbs { get; set; } = false;

    /// <summary>
    /// Gets or sets the penalty for repeated tokens to discourage repetitive output.
    /// Valid range: 0.0 to 2.0.
    /// </summary>
    public double RepetitionPenalty
    {
        get => _repetitionPenalty;
        set => _repetitionPenalty = ValidateRange(value, 0.0, 2.0, nameof(RepetitionPenalty));
    }


    /// <summary>
    /// Gets or sets the value for nucleus sampling, where only the smallest set of tokens whose cumulative probability exceeds TopP is considered.
    /// Valid range: 0.0 to 1.0.
    /// </summary>
    public double TopP
    {
        get => _topP;
        set => _topP = ValidateRange(value, 0.0, 1.0, nameof(TopP));
    }


    /// <summary>
    /// Gets or sets the number of top tokens to consider during sampling.
    /// Must be a positive integer.
    /// </summary>
    public int TopK
    {
        get => _topK;
        set => _topK = ValidatePositive(value, nameof(TopK));
    }

    /// <summary>
    /// Gets or sets the minimum number of tokens to generate.
    /// Must be non-negative.
    /// </summary>
    public int MinTokens
    {
        get => _minTokens;
        set => _minTokens = ValidateNonNegative(value, nameof(MinTokens));
    }

    /// <summary>
    /// Gets or sets a value indicating whether to stream the output as it is generated.
    /// </summary>
    public bool Stream => !string.IsNullOrEmpty(StreamId);

    public readonly string StreamId;

    public readonly string StreamMethod;

    /// <summary>
    /// Gets or sets the maximum number of tokens to generate.
    /// Must be a positive integer.
    /// </summary>
    public int MaxTokens
    {
        get => _maxTokens;
        set => _maxTokens = ValidatePositive(value, nameof(MaxTokens));
    }

    /// <summary>
    /// Gets or sets additional settings for reasoning behavior during generation. Can be null.
    /// </summary>
    public ReasoningSettings ReasoningSettings { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GenerateSettings"/> class with required and default values.
    /// </summary>
    /// <param name="modelName">The name of the model to use. Cannot be null or empty.</param>
    /// <param name="temperature">The temperature for generation randomness. Default is 1.0.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="modelName"/> is null or empty.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="temperature"/> is out of range.</exception>
    public GenerateSettings(double temperature = 0.1,
        double repetitionPenalty = 1.04,
        double topP = 0.8,
        int topK = 5,
        int minTokens = 8,
        int maxTokens = 2248,
        string streamId = null,
        string streamMethod = "StreamMessage")
    {
        Temperature = temperature;
        _repetitionPenalty = repetitionPenalty;
        _topP = topP;
        _topK = topK;
        _minTokens = minTokens;
        _maxTokens = maxTokens;
        StreamId = streamId;
        StreamMethod = streamMethod;
        ReasoningSettings = null;
    }

    /// <summary>
    /// Validates that a double value is within the specified range.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="min">The minimum allowed value.</param>
    /// <param name="max">The maximum allowed value.</param>
    /// <param name="paramName">The name of the parameter for error reporting.</param>
    /// <returns>The validated value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the value is out of range.</exception>
    private static double ValidateRange(double value, double min, double max, string paramName)
    {
        if (value < min || value > max)
            throw new ArgumentOutOfRangeException(paramName, $"Value must be between {min} and {max}.");
        return value;
    }



    /// <summary>
    /// Validates that a double value is within the specified range.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="min">The minimum allowed value.</param>
    /// <param name="max">The maximum allowed value.</param>
    /// <param name="paramName">The name of the parameter for error reporting.</param>
    /// <returns>The validated value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the value is out of range.</exception>
    private static int ValidateRange(int value, int min, int max, string paramName)
    {
        if (value < min || value > max)
            throw new ArgumentOutOfRangeException(paramName, $"Value must be between {min} and {max}.");
        return value;
    }

    /// <summary>
    /// Validates that an integer value is positive.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="paramName">The name of the parameter for error reporting.</param>
    /// <returns>The validated value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the value is not positive.</exception>
    private static int ValidatePositive(int value, string paramName)
    {
        if (value <= 0)
            throw new ArgumentOutOfRangeException(paramName, "Value must be positive.");
        return value;
    }

    /// <summary>
    /// Validates that an integer value is non-negative.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="paramName">The name of the parameter for error reporting.</param>
    /// <returns>The validated value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the value is negative.</exception>
    private static int ValidateNonNegative(int value, string paramName)
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(paramName, "Value cannot be negative.");
        return value;
    }
}