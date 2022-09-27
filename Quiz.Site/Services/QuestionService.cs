using Microsoft.AspNetCore.Mvc.Rendering;
using Quiz.Site.Models;

namespace Quiz.Site.Services;
public class QuestionService : IQuestionService
{
    private readonly IQuestionRepository _questionRepository;

    public QuestionService(IQuestionRepository questionRepository)
    {
        _questionRepository = questionRepository;
    }

    public List<QuizQuestionViewModel> GetListOfQuestions(int[] questionIds)
    {
        List<QuizQuestionViewModel> quizQuestions;
        var questions = _questionRepository.GetByIds(questionIds);

        quizQuestions = new List<QuizQuestionViewModel>();
        if (questions != null && questions.Any())
        {
            foreach (var question in questions)
            {
                List<SelectListItem> answers = new List<SelectListItem>();
                var wrongCount = 0;
                for (var i = 0; i < 4; i++)
                {
                    if (question.CorrectAnswerPosition == i)
                    {
                        answers.Add(new SelectListItem() { Value = i.ToString(), Text = question.CorrectAnswer });
                    }
                    else
                    {
                        wrongCount++;
                        switch (wrongCount)
                        {
                            case 1:
                                answers.Add(new SelectListItem() { Value = i.ToString(), Text = question.WrongAnswer1 });
                                break;
                            case 2:
                                answers.Add(new SelectListItem() { Value = i.ToString(), Text = question.WrongAnswer2 });
                                break;
                            case 3:
                                answers.Add(new SelectListItem() { Value = i.ToString(), Text = question.WrongAnswer3 });
                                break;
                        }
                    }
                }

                var quizQuestion = new QuizQuestionViewModel();

                quizQuestion.QuestionId = question.Id;
                quizQuestion.QuestionText = question.QuestionText;
                quizQuestion.CorrectAnswerPosition = question.CorrectAnswerPosition;
                quizQuestion.Answers = answers;
                quizQuestion.MoreInfoLink = question.MoreInfoLink;

                quizQuestions.Add(quizQuestion);
            }
        }

        return quizQuestions;
    }
}
