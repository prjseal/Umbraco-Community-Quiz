namespace Quiz.Site
{
    public static class Quiz
    {
        public const string AppName = "Quiz";
        public static class Tables
        {
            public const string QuestionTable = "Question";
            public const string QuizResultTable = "QuizResult";
            public const string NotificationTable = "Notification";
            public const string ReadNotificationTable = "ReadNotification";
        }

        public static class TokenReasons
        {
            public const string ForgottenPasswordRequest = "fpr";
        }
    }
}
