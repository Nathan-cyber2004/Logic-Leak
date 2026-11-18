using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class Question
{
    public string tag, prompt, code, answer, explanation;
    public List<string> wrong;

    // Builds a question and removes duplicate or accidentally-correct wrong answers.
    public static Question Make(string tag, string prompt, string code,
                                string answer, IEnumerable<string> wrong, string why)
    {
        return new Question {
            tag = tag, prompt = prompt, code = code, answer = answer, explanation = why,
            wrong = wrong.Distinct().Where(w => w != answer).ToList()
        };
    }
}