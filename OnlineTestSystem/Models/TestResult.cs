namespace OnlineTestSystem.Models
{
    public class TestResult
    {
        public int ResultId { get; set; }
        public int UserId { get; set; }
        public int Marks { get; set; }
        public DateTime TestDate { get; set; }
        public User User { get; set; }
    }
}
