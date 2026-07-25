namespace Al_Muzayyen.Models
{
    public class ShortLink
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string TargetUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }
}
