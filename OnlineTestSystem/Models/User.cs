namespace OnlineTestSystem.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string? RollNumber { get; set; }
        public string Name { get; set; }
        public string? Class { get; set; }
        public string? Course { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
}
