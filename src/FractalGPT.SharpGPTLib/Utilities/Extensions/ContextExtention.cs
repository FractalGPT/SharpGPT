using FractalGPT.SharpGPTLib.Core.Models.Common.Messages;

namespace FractalGPT.SharpGPTLib.Utilities.Extensions;

public static class ContextExtention
{
    /// <summary>
    /// Исправление контекста для соответствия требованиям OpenAI API.
    /// API требует, чтобы:
    /// 1. Сообщения чередовались между ролями (user/assistant)
    /// 2. Первое сообщение не было от assistant
    /// 3. После system не шло сразу assistant
    /// Метод автоматически вставляет пустые сообщения для соблюдения этих требований.
    /// </summary>
    /// <param name="context">Контекст сообщений</param>
    /// <returns>Исправленный контекст с корректным чередованием ролей</returns>
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
