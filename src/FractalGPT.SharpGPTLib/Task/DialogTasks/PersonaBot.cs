﻿using FractalGPT.SharpGPTLib.Prompts.PersonaChat;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FractalGPT.SharpGPTLib.Task.DialogTasks
{
    /// <summary>
    /// A bot based on the PersonaChat task, designed to interact with users by leveraging a persona chat model.
    /// This class manages the persona chat functionalities, including setting facts, generating, and sending answers.
    /// </summary>
    [Serializable]
    public class PersonaBot
    {
        /// <summary>
        /// Gets or sets the PersonaChat object used for creating chat prompts based on a set persona.
        /// </summary>
        public PersonaChat PersonaChatPromptCreator { get; set; }

        /// <summary>
        /// Delegate for asynchronously generating an answer based on the given input.
        /// </summary>
        public Func<string, Task<string>> AnswerGenAsync { get; set; }

        /// <summary>
        /// Token used to indicate the end of a generated response.
        /// </summary>
        public string TokenEnd { get; set; } = "<extra_id_0>";

        /// <summary>
        /// Default constructor.
        /// </summary>
        public PersonaBot() { }

        /// <summary>
        /// Initializes a new instance of the PersonaBot class with a specified PersonaChat.
        /// </summary>
        /// <param name="personaChat">The PersonaChat instance to use for generating chat prompts.</param>
        public PersonaBot(PersonaChat personaChat)
        {
            PersonaChatPromptCreator = personaChat;
            NewAnswer += PersonaBot_NewAnswer;
        }

        /// <summary>
        /// Sets the facts for the persona, which are used in the chat to define the bot's character.
        /// </summary>
        /// <param name="facts">A collection of facts about the persona.</param>
        public void SetFacts(IEnumerable<string> facts)
        {
            PersonaChatPromptCreator.Facts = Fact.GetFacts(facts);
        }

        /// <summary>
        /// Asynchronously generates an answer to the provided query based on the persona and the chat history.
        /// </summary>
        /// <param name="q">The query to respond to.</param>
        /// <returns>A task that represents the asynchronous operation, containing the bot's answer.</returns>
        public async Task<string> GetAnswer(string q)
        {
            PersonaChatPromptCreator.AddUserMessage(q);

            string input = PersonaChatPromptCreator.ToString() +
                PersonaChatPromptCreator.SepReplics + PersonaChatPromptCreator.BotTag + $" {TokenEnd}";

            string answer = await AnswerGenAsync(input);
            answer = answer.Replace(TokenEnd, "");
            PersonaChatPromptCreator.AddAssistantMessage(answer);
            return answer;
        }

        /// <summary>
        /// Sends an answer to the provided query by generating it and then invoking the NewAnswer event.
        /// </summary>
        /// <param name="q">The query to respond to.</param>
        public async void SendAnswer(string q)
        {
            string answer = await GetAnswer(q);
            NewAnswer(answer);
        }

        /// <summary>
        /// An event that is raised when a new answer is generated by the bot.
        /// </summary>
        public event Action<string> NewAnswer;

        // Placeholder method for the NewAnswer event.
        private void PersonaBot_NewAnswer(string obj) { }
    }
}
