namespace Quiz.Site.Models
{
    public class PlayerRecord
    {
        public string MemberId { get; set; }
        public string Name { get; set; }
        public string AvatarUrl { get; set; }
        public int Correct { get; set; }
        public int Total { get; set; }
        public int Quizzes { get; set; }
        public float Percentage => Correct > 0 ? (float)Correct / (float)Total : 0;
    }
}
