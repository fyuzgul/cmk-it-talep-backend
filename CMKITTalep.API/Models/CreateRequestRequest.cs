namespace CMKITTalep.API.Models
{
    public class CreateRequestRequest
    {
        public int RequestCreatorId { get; set; }
        public int RequestTypeId { get; set; }
        public int PriorityLevelId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? ScreenshotFilePath { get; set; }
        public string? ScreenshotBase64 { get; set; }
        public string? ScreenshotFileName { get; set; }
        public string? ScreenshotMimeType { get; set; }
        public List<int> CCUserIds { get; set; } = new List<int>();
    }
}
