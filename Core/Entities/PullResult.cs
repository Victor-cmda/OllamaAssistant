namespace Core.Entities
{
    public class PullResult
    {
        public bool Success { get; }
        public PullProgress Progress { get; }
        public string ErrorMessage { get; }

        public PullResult(bool success, PullProgress progress = null, string errorMessage = null)
        {
            Success = success;
            Progress = progress;
            ErrorMessage = errorMessage;
        }
    }
}
