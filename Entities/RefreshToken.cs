namespace AuthService.Entities
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string TokenHash { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public string? ReplacedByTokenHash { get; set; }
        public string? DeviceInfo { get; set; }
        public bool IsActive => ReplacedByTokenHash == null 
            && ExpiresAt > DateTime.UtcNow;
    }
}
