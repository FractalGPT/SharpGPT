namespace FractalGPT.SharpGPTLib.Services.Prompts.FewShot.Models
{
    /// <summary>
    /// Represents an element for few-shot (QA Task) learning scenarios, encapsulating a prompt and its corresponding model output.
    /// </summary>
    [Serializable]
    public class QAFewShotElement : FewShotElement
    {
        public string Q { get; protected set; }

        public string QPartTag { get; protected set; } = "{q}";

        public QAFewShotElement(string basePrompt, string q, string a)
        {
            Q = q;
            ModelOutput = a;
            Prompt = basePrompt.Replace(QPartTag, Q);
        }


        public static QAFewShotElement[] GetArrayFewShots(string basePrompt, IEnumerable<string> qs, IEnumerable<string> answers)
        {
            string[] qArr = qs.ToArray();
            string[] aArr = answers.ToArray();

            if (qArr.Length != aArr.Length)
                throw new Exception("Mismach q and a");

            QAFewShotElement[] qAFewShotElements = new QAFewShotElement[qArr.Length];

            for (int i = 0; i < qArr.Length; i++)
                qAFewShotElements[i] = new QAFewShotElement(basePrompt, qArr[i], aArr[i]);

            return qAFewShotElements;
        }
    }
}
