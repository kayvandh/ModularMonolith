using FluentResults;

namespace Framework.FluentResultsAddOn
{
    public class FluentError : IError
    {
        public string ErrorCode { get; }
        public string ErrorMessage { get; }
        public string Message => $"{ErrorCode}: {ErrorMessage}";
        public List<IError> Reasons => new();
        public Dictionary<string, object> Metadata { get; } = new();

        public FluentError(string errorCode, string errorMessage)
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }
    }
}
