namespace CMKITTalep.Entities
{
    public class SupportType
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; } = false;

        // Navigation Property - One SupportType can have many RequestTypes
        public ICollection<RequestType> RequestTypes { get; set; } = new List<RequestType>();
    }
}
