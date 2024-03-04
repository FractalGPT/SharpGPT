using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FractalGPT.SharpGPTLib.Prompts.PersonaChat
{
    /// <summary>
    /// Represents a persona chat context, including facts about the persona and methods for managing conversation flow.
    /// This class is used to build and manage the structure of conversations for a persona-based chatbot.
    /// </summary>
    [Serializable]
    public class PersonaChat
    {
        /// <summary>
        /// Gets or sets the list of facts that define the persona.
        /// </summary>
        public List<Fact> Facts { get; set; }

        /// <summary>
        /// Gets or sets the context of the conversation, including the bot and user tags, and messages.
        /// </summary>
        public PersonaContext Context { get; set; } = new PersonaContext(3);

        /// <summary>
        /// Gets or sets the token that marks the start of the conversation.
        /// </summary>
        public string StartToken { get; set; } = "<CS6>";

        /// <summary>
        /// Gets or sets the initial phrase to start the conversation.
        /// </summary>
        public string StartConversation { get; set; } = "Start";

        /// <summary>
        /// Gets or sets the separator token for replicating messages in the conversation.
        /// </summary>
        public string SepReplics { get; set; } = "\n";

        /// <summary>
        /// Gets or sets the tag used to denote messages from the bot.
        /// </summary>
        public string BotTag
        {
            get
            {
                return Context.BotTag;
            }
            set
            {
                Context.BotTag = value;
            }
        }

        /// <summary>
        /// Gets or sets the tag used to denote messages from the user.
        /// </summary>
        public string UserTag
        {
            get
            {
                return Context.UserTag;
            }
            set
            {
                Context.UserTag = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the PersonaChat class with a default fact.
        /// </summary>
        public PersonaChat()
        {
            Facts = new List<Fact>() { new Fact("I`m robot") };
        }

        /// <summary>
        /// Initializes a new instance of the PersonaChat class with a specified list of facts.
        /// </summary>
        /// <param name="facts">The facts defining the persona.</param>
        public PersonaChat(IEnumerable<Fact> facts) =>
            Facts = facts.ToList();

        /// <summary>
        /// Initializes a new instance of the PersonaChat class with facts specified as strings.
        /// </summary>
        /// <param name="facts">The facts defining the persona, provided as strings.</param>
        public PersonaChat(IEnumerable<string> facts) =>
            Facts = Fact.GetFacts(facts);

        /// <summary>
        /// Adds a message from the assistant (bot) to the conversation context.
        /// </summary>
        /// <param name="mess">The message to add.</param>
        public void AddAssistantMessage(string mess) =>
            Context.AddAssistantMessage(mess);

        /// <summary>
        /// Adds a message from the user to the conversation context.
        /// </summary>
        /// <param name="mess">The message to add.</param>
        public void AddUserMessage(string mess) =>
            Context.AddUserMessage(mess);

        /// <summary>
        /// Returns a string representation of the current conversation, including persona facts and messages.
        /// </summary>
        /// <returns>A string that represents the current state of the conversation.</returns>
        public override string ToString()
        {
            StringBuilder prompt = new StringBuilder();
            prompt.Append(StartToken);

            foreach (var fact in Facts)
            {
                prompt.Append(fact.FactBody.Trim('.'));
                prompt.Append(". ");
            }

            prompt.Append(StartConversation);

            foreach (var message in Context.Messages)
            {
                prompt.Append(message);
                prompt.Append(SepReplics);
            }

            return prompt.ToString();
        }
    }
}
