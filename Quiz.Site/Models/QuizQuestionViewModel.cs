using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Quiz.Site.Models;

public class QuizQuestionViewModel
{
    public int? QuestionId { get; set; }
    public int? CorrectAnswerPosition { get; set; }
    public string? QuestionText { get; set; }
    public List<SelectListItem>? Answers { get; set; }

    public bool? IsCorrect { get; set; }
    public string? MoreInfoLink { get; set; }

    [Required]
    public string Answer { get; set; }
}
