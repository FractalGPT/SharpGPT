using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;


namespace FractalGPT.SharpGPTLib.MLWithLLM.Speech
{
    /// <summary>
    /// Wrapper class for the Speech Synthesizer functionality.
    /// </summary>
    public class SpeechSynthesizerWrapper
    {
        private dynamic _speechSynthesizer;

        /// <summary>
        /// Initializes a new instance of the SpeechSynthesizerWrapper class.
        /// </summary>
        public SpeechSynthesizerWrapper()
        {
            _speechSynthesizer = Activator.CreateInstance(Type.GetTypeFromProgID("SAPI.SpVoice"));
        }

        /// <summary>
        /// Lists available voices.
        /// </summary>
        /// <returns>List of descriptions of available voices.</returns>
        public List<string> ListVoices()
        {
            var voicesList = new List<string>();
            foreach (var voice in _speechSynthesizer.GetVoices())
            {
                voicesList.Add(voice.GetDescription());
            }
            return voicesList;
        }

        /// <summary>
        /// Sets the voice of the synthesizer.
        /// </summary>
        /// <param name="voiceIdentifier">Voice identifier, which can be an index (int) or a string.</param>
        public void SetVoice(dynamic voiceIdentifier)
        {
            if (voiceIdentifier is int)
            {
                _speechSynthesizer.Voice = _speechSynthesizer.GetVoices().Item(voiceIdentifier);
            }
            else if (voiceIdentifier is string)
            {
                foreach (dynamic voice in _speechSynthesizer.GetVoices())
                {
                    if (voice.GetDescription() == voiceIdentifier)
                    {
                        _speechSynthesizer.Voice = voice;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Speaks the specified text.
        /// </summary>
        /// <param name="text">Text to be spoken.</param>
        public async void Speak(string text)
        {
             _speechSynthesizer.Speak(text);
        }

        /// <summary>
        /// Releases resources used by the SpeechSynthesizer.
        /// </summary>
        public void ReleaseResources()
        {
            Marshal.ReleaseComObject(_speechSynthesizer);
        }
    }

}
