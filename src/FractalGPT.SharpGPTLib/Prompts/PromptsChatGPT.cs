using System;

namespace FractalGPT.SharpGPTLib.Prompts
{
    /// <summary>
    /// Prompts for LLM
    /// </summary>
    [Serializable]
    public static class PromptsChatGPT
    {

        /// <summary>
        /// Prompt for responses in English by ChatGPT
        /// </summary>
        public static string ChatGPTDefaltPromptEN = PromptManager.SystemPrompts["default_en"];

        /// <summary>
        /// Prompt for responses in Russian by ChatGPT
        /// </summary>
        public static string ChatGPTDefaltPromptRU { get; set; } = PromptManager.SystemPrompts["default_ru"];

        /// <summary>
        /// Prompt for text summarization by ChatGPT
        /// </summary>
        public static string ChatGPTSummarizationPromptEN { get; set; } = PromptManager.SystemPrompts["summarization_en"];

        /// <summary>
        /// Prompt for text-based Q&A
        /// </summary>
        public static string ChatGPTTextQAPromptEN { get; set; } = PromptManager.SystemPrompts["text_qa_en"];

        /// <summary>
        /// Prompt for generating an LLM prompt for a task
        /// </summary>
        public static string ChatGPTPromptGenerationPromptEN { get; set; } = PromptManager.SystemPrompts["prompt_generation_en"];
    }
}
