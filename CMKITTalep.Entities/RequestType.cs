namespace CMKITTalep.Entities
{
    public class RequestType
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int SupportTypeId { get; set; } // Foreign Key to SupportType
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; } = false;

        // Navigation Property
        public SupportType? SupportType { get; set; }
    }
}
