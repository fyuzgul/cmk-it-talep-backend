namespace CMKITTalep.Entities
{
    public class Request
    {
        public int Id { get; set; }
        public int? SupportProviderId { get; set; } // Foreign Key to User (Destek Veren)
        public int RequestCreatorId { get; set; } // Foreign Key to User (Talep Açan)
        public string Description { get; set; } = string.Empty;
        public string? ScreenshotFilePath { get; set; } // File path for screenshot
        public int RequestStatusId { get; set; } // Foreign Key to RequestStatus
        public int RequestTypeId { get; set; } // Foreign Key to RequestType
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; } = false;

        // Navigation Properties
        public User? SupportProvider { get; set; } // Destek Veren
        public User? RequestCreator { get; set; } // Talep Açan
        public RequestStatus? RequestStatus { get; set; }
        public RequestType? RequestType { get; set; }
        
        // Collection navigation property for responses to this request
        public ICollection<RequestResponse> RequestResponses { get; set; } = new List<RequestResponse>();
    }
}
