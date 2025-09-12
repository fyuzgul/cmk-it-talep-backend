namespace CMKITTalep.Entities
{
    public class RequestResponse
    {
        public int Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? FilePath { get; set; } // Dosya yolu
        public int RequestId { get; set; } // Foreign Key to Request
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; } = false;

        // Navigation Property
        public Request? Request { get; set; }
    }
}
