namespace Quiz.Site.Models;

public class AnswerModel
{
    public int CorrectAnswer { get; set; }
    public int SubmittedAnswer { get; set; }
    public bool IsCorrect => CorrectAnswer == SubmittedAnswer;
}