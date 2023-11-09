using System;
using System.IO;
using System.Threading.Tasks;
using Vosk;

namespace FractalGPT.SharpGPTLib.MLWithLLM.Speech
{
    /// <summary>
    /// Provides speech recognition using the Vosk library.
    /// </summary>
    [Serializable]
    public class SpeechRecognizer
    {
        /// <summary>
        /// Occurs when speech has been recognized.
        /// </summary>
        public event Action<string> SpeechRecognized;

        private readonly Model _model;
        private readonly float _sampleRate;


        /// <summary>
        /// Initializes a new instance of the <see cref="SpeechRecognizer"/> class.
        /// </summary>
        /// <param name="modelPath">The path to the Vosk model directory.</param>
        /// <param name="sampleRate">The sample rate of the audio to be recognized.</param>
        public SpeechRecognizer(string modelPath, float sampleRate)
        {
            _model = new Model(modelPath);
            _sampleRate = sampleRate;
        }

        /// <summary>
        /// Asynchronously recognizes speech from an audio file.
        /// </summary>
        /// <param name="audioPath">The path to the audio file to be processed.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task RecognizeSpeechAsync(string audioPath)
        {
            string[] s = { "{\n  \"text\" : \"" };

            await Task.Run(() =>
            {
                using (var recognizer = new VoskRecognizer(_model, _sampleRate))
                {
                    using (Stream source = File.OpenRead(audioPath))
                    {
                        byte[] buffer = new byte[4096];
                        int bytesRead;
                        while ((bytesRead = source.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            if (recognizer.AcceptWaveform(buffer, bytesRead))
                            {
                                var result = recognizer.Result();
                                SpeechRecognized?.Invoke(result.Split('"')[3].Split('"')[0]);
                            }
                        }
                    }
                    var finalResult = recognizer.FinalResult();

                    SpeechRecognized?.Invoke(finalResult.Split('"')[3].Split('"')[0]);
                }
            });
        }
    }
}
