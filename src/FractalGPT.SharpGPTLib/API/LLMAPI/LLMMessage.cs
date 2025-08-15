using System.Buffers.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FractalGPT.SharpGPTLib.API.LLMAPI;

/// <summary>
/// Represents a chat message from different roles (e.g., "user", "assistant").
/// </summary>
[Serializable]
public class LLMMessage
{
    /// <summary>
    /// Gets the role of the message sender (e.g., "user" or "assistant").
    /// </summary>
    [JsonPropertyName("role")]
    public string Role { get; private set; }

    /// <summary>
    /// Gets or sets the content of the message (can be null).
    /// </summary>
    [JsonPropertyName("content")]
    public object Content { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LLMMessage"/> class for serialization.
    /// </summary>
    private LLMMessage() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="LLMMessage"/> class with the specified role and content.
    /// </summary>
    /// <param name="role">The role of the message sender (e.g., "user", "assistant").</param>
    /// <param name="content">The text content of the message (can be null).</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="role"/> is null or whitespace.</exception>
    public LLMMessage(string role, string content)
    {
        if (string.IsNullOrWhiteSpace(role))
            throw new ArgumentException("Role cannot be null or whitespace.", nameof(role));

        Role = role;
        var ContentM = new MessageContent(content); // Allow null content
        ContentM.AddImage("https://sun9-48.userapi.com/s/v1/if2/xUAZ-qJVU9q0EqraxHUlF7jPt1u80h2_bBIcZMEes-mGEKF6kNIaLlZnokv8qjmvhCCoQpykF58x0JNSs3iYDT7U.jpg?quality=95&as=32x24,48x36,72x54,108x81,160x120,240x180,360x270,480x360,540x405,640x480,720x540,1080x810,1280x959,1440x1079,2560x1919&from=bu&cs=1280x0");
        Content = ContentM;
    }

    public LLMMessage(string role, MessageContent content)
    {
        if (string.IsNullOrWhiteSpace(role))
            throw new ArgumentException("Role cannot be null or whitespace.", nameof(role));

        Role = role;
        Content = content; // Allow null content
    }

    /// <summary>
    /// Creates a message for sending to the LLM API.
    /// </summary>
    /// <param name="role">The role of the sender.</param>
    /// <param name="content">The message content (can be null).</param>
    /// <returns>A new <see cref="LLMMessage"/> instance.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="role"/> is invalid.</exception>
    public static LLMMessage CreateMessage(Roles role, string content)
    {
        var senderRole = role.ToString().ToLower();
        return new LLMMessage(senderRole, content);
    }

    /// <summary>
    /// Creates a deep copy of the <see cref="LLMMessage"/> instance.
    /// </summary>
    /// <returns>A new <see cref="LLMMessage"/> instance with the same properties.</returns>
    public LLMMessage DeepClone()
    {
        if(Content is string) 
            return new LLMMessage(Role, Content as string);
        else return new LLMMessage(Role, Content as MessageContent);
    }
}

/// <summary>
/// Содержание контента (тексты, изображения)
/// </summary>
[Serializable]
public class MessageContent : List<IContentItem>
{
    public MessageContent() { }

    public MessageContent(string content)
    {
        TextContentItem textContent = new TextContentItem();
        textContent.Text = content;
        Add(textContent);
    }

    public void AddImage(string url)
    {
        ImageContent imageContent = new ImageContent(url);
        Add(imageContent);
    }


    public void AddImage(IEnumerable<byte> image)
    {
        ImageContent imageContent = new ImageContent(image);
        Add(imageContent);
    }


}

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(TextContentItem), "text")]
[JsonDerivedType(typeof(ImageContent), "image_url")]
public interface IContentItem
{
    [JsonPropertyName("type")]
    string Type { get; }
}

/// <summary>
/// Текстовая часть контента
/// </summary>
[Serializable]
public class TextContentItem : IContentItem
{
    [JsonIgnore]
    public string Type => "text";

    [JsonPropertyName("text")]
    public string Text { get; set; }
}

[Serializable]
public class ImageContent : IContentItem
{
    [JsonIgnore]
    public string Type => "image_url";

    [JsonPropertyName("image_url")]
    public ImageUrl ImageUrl { get; set; }


    public ImageContent() { }

    public ImageContent(string imageUrl)
    {
        ImageUrl = new ImageUrl { Url = imageUrl };
    }

    public ImageContent(IEnumerable<byte> image)
    {
        string base64 = Convert.ToBase64String(image.ToArray());
        ImageUrl = new ImageUrl { Url = $"data:image/jpeg;base64,{base64}" };
    }
}


public class ImageUrl 
{
    [JsonPropertyName("url")]
    public string Url { get; set; }
}

/// <summary>
/// Roles for chat messages.
/// </summary>
[Serializable]
public enum Roles : byte
{
    Assistant = 1,
    User = 2,
    System = 3
}