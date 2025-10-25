using FractalGPT.SharpGPTLib.Core.Models.Common.Messages;

namespace FractalGPT.SharpGPTLib.Utilities.Extensions;

public static class ContextExtention
{
    /// <summary>
    /// Исправление контекста
    /// </summary>
    /// <param name="context">Контекст сообщений</param>
    public static List<LLMMessage> FixContext(this IEnumerable<LLMMessage> context)
    {
        List<LLMMessage> fixedMessages = [];

        foreach (var message in context)
        {
            if (fixedMessages.Count == 0)
            {
                if (message.Role == "assistant")
                    fixedMessages.Add(LLMMessage.CreateMessage(Roles.User, " "));

                fixedMessages.Add(message);
            }
            else
            {
                if (message.Role == fixedMessages[fixedMessages.Count - 1].Role)
                    fixedMessages.Add(LLMMessage.CreateMessage(message.Role == "assistant" ? Roles.User : Roles.Assistant, ""));

                if (message.Role == "assistant" && fixedMessages[fixedMessages.Count - 1].Role == "system")
                    fixedMessages.Add(LLMMessage.CreateMessage(Roles.User, ""));

                fixedMessages.Add(message);
            }
        }

        return fixedMessages;
    }
}
