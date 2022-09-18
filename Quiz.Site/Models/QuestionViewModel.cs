using Quiz.Site.Validation;
using System.ComponentModel.DataAnnotations;

namespace Quiz.Site.Models
{
    public class QuestionViewModel
    {
        [Required(ErrorMessage = "The question must have an id")]
        [Display(Name = "Id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "You must enter the question text")]
        [Display(Name = "Question Text")]
        public string QuestionText { get; set; }

        [Display(Name = "Correct Answer")]
        [Required(ErrorMessage = "You must add a correct answer")]
        public string CorrectAnswer { get; set; }

        [Display(Name = "Wrong Answer 1")]
        [Required(ErrorMessage = "You must enter wrong answer 1")]
        public string WrongAnswer1 { get; set; }

        [Display(Name = "Wrong Answer 2")]
        [Required(ErrorMessage = "You must enter wrong answer 2")]
        public string WrongAnswer2 { get; set; }

        [Display(Name = "Wrong Answer 3")]
        [Required(ErrorMessage = "You must enter wrong answer 3")]
        public string WrongAnswer3 { get; set; }

        [Display(Name = "More Information Link")]
        public string MoreInfoLink { get; set; }

        public bool Consent { get; set; }
    }
}