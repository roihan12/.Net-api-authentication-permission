namespace Application.Exceptions
{
    public class CustomValidationException : Exception
    {
        public List<string> ErrorMessages { get; set; }

        public string FriendlyErrorMessage { get; set; }
        public CustomValidationException(List<string> errorsMessages, string friendlyErrorMessage)
            : base(friendlyErrorMessage)
        {
            ErrorMessages = errorsMessages;
            FriendlyErrorMessage = friendlyErrorMessage;
        }
    }
}
