using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace FractalGPT.SharpGPTLib.API.LocalServer
{
    [Serializable]
    public class BaseAPI
    {
        public string Host { get; set; }

        public BaseAPI(string host = "http://127.0.0.1:8080/") 
        {
            Host = host;
        }

        public HttpStatusCode SetLLM(string modelName, string modelType) 
        {
        
        }

        public string TextGeneration(string prompt, int maxLen = 50, double temperature = 0.6, int topK = 15, double topP = 0.8, int noRepeatNgramSize = 3) { }
    }
}
