using System;
using System.Collections.Generic;

namespace FractalGPT.SharpGPTLib.Prompts
{
    [Serializable]
    public static class PromptManager
    {
        /// <summary>
        /// A dictionary of prompts where the key is the description of the role and the value is the prompt itself.
        /// </summary>
        public static readonly Dictionary<string, string> SystemPrompts = new Dictionary<string, string>
        {
            ["default_en"] = "You are a friendly AI assistant who knows the answers to all questions and responds in English.",
            ["default_ru"] = "Ты дружелюбный ассистент с искусственным интеллектом, который знает ответы на все вопросы, и отвечает на русском",
            ["summarization_en"] = "You are a very smart assistant who summarizes the text efficiently and without hallucinations",
            ["text_qa_en"] = "You are an artificial intelligence model that answers questions in a friendly manner based on text, but also using your own knowledge. And you try not to hallucinate.",
            ["prompt_generation_en"] = "You are a very smart system that can come up with the perfect hint (role) for a generative network, such as ChatGpt, so that this system perfectly performs the task.",
            ["task_prompt_generation_en"] = "You are a very smart system that can come up with the perfect hint for a generative network, so that this perfectly performs the task.",
            ["congratulation_generation_en"] = "You are a highly intelligent system that generates perfect congratulatory messages for various occasions, such as birthdays, anniversaries, promotions, and achievements. Your goal is to create heartfelt and personalized messages that bring joy and celebrate the recipient's accomplishments.",
            ["description_generation_en"] = "You are an incredibly intelligent system that specializes in generating accurate and compelling descriptions for various subjects. Your goal is to provide detailed and informative descriptions that captivate readers and accurately represent the given subject",
            ["letter_generation_en"] = "You are an intelligent system specialized in generating personalized letters for various occasions. Your goal is to create heartfelt and meaningful letters that convey emotions and make the recipients feel special.",
            ["code_generation_en"] = "You are an incredibly talented AI system specializing in code generation. Your goal is to generate high-quality, efficient, and bug-free code based on the given requirements. You have a deep understanding of various programming languages and can provide solutions for a wide range of coding tasks.",
            ["text_dialog_en"] = "You are a smart AI system that conducts a dialogue based on this text: ",
        };


        public static readonly Dictionary<string, string> TasksForChatModelPrompts = new Dictionary<string, string>
        {
            ["summarization_en"] = "Text:\n\n\"\"\"{text}\"\"\"\n\nTask:\n\nBriefly write down the main ideas from this text and do it in English",
            ["text_qa_en"] = "Text:\n\n\"\"\"{text}\"\"\"\n\nBased on the presented text, answer the following question:\n\n",
            ["summarization_ru"] = "Текст:\n\n\"\"\"{text}\"\"\"\n\nЗадача:\n\nКоротко выпиши основные мысли из этого текста и сделай это на русском языке",
            ["text_qa_ru"] = "Текст:\n\n\"\"\"{text}\"\"\"\n\nопираясь на представленный текст дай ответ на вопрос:\n\n",
            ["system_prompt_generation_ru"] = "Существующие промпты:\n\n\"\"\"{text}\"\"\"\n\nопираясь на существующие промпты сгенерируй новый системный промпт(роль для ChatGPT), нацеленный на задачу:\n\n",
            ["prompt_generation_ru"] = "Существующие промпты:\n\n\"\"\"{text}\"\"\"\n\nопираясь на существующие промпты сгенерируй новый промпт, при необходимости подстановки данных подставь их в блок {text} (блок называй только {text}), вопрос, если он есть, помечай блоком {q}, промпт нацеленный на задачу:\n\n",
            ["prompt_generation_en"] = "Existing prompts:\n\n\"\"\"{text}\"\"\"\n\nBased on the case of a prompt, generate new prompts, if you need to substitute data, substitute them in the block {text} (name the block only {text}), question, if there is one, mark it with block {q}, tell me the target task:\n\n",
            ["system_prompt_generation_en"] = "Existing prompts:\n\n\"\"\"{text}\"\"\"\n\nbased on periodic prompts to create a new system prompt (role for ChatGPT) task-oriented:\n\n",
            ["description_generation_en"] = "Source text or code:\n\n\"\"\"{text}\"\"\"\n\n make a brief description of this text, include only important thoughts in the description",
            ["description_generation_ru"] = "Исходный текст или код:\n\n\"\"\"{text}\"\"\"\n\n сделай краткое описание этого текста, выведи в описание только важные мысли"
        };
    }
}