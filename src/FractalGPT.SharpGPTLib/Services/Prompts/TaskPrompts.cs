namespace FractalGPT.SharpGPTLib.Services.Prompts
{
    [Serializable]
    public static class TaskPrompts
    {

        public static string InputPrompt(string text, string task, string lang = "en")
        {
            string summarizationPromptBase = PromptManager.TasksForChatModelPrompts[$"{task}_{lang}"];
            return summarizationPromptBase.Replace("{text}", text);
        }

    }
}
