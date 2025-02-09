namespace Core.Entities
{
    public class PullProgress
    {
        public string Status { get; set; }
        public long Completed { get; set; }
        public long Total { get; set; }
        public string DownloadedSize { get; set; }
        public string TotalSize { get; set; }
        public int ProgressPercentage { get; set; }
    }
}
