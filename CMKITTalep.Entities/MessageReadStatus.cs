namespace CMKITTalep.Entities
{
    public class MessageReadStatus
    {
        public int Id { get; set; }
        public int MessageId { get; set; } // Foreign Key to RequestResponse
        public int UserId { get; set; } // Foreign Key to User
        public DateTime ReadAt { get; set; } = DateTime.UtcNow;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; } = false;

        // Navigation Properties
        public RequestResponse? Message { get; set; }
        public User? User { get; set; }
    }
}
