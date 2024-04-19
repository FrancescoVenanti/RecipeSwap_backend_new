namespace RecipeSwapTest.Models
{
    public class messageDto
    {
        public int SenderUserId { get; set; }
        public int ReceiverUserId { get; set; }
        public string MessageContent { get; set; }
        public DateTime Timestamp { get; set; }

        public bool? IsRead { get; set; }
    }
}
