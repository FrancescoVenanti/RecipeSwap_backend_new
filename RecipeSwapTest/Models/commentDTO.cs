namespace RecipeSwapTest.Models
{
    public class commentDTO
    {
        public int UserId { get; set; }
        public int RecipeId { get; set; }
        public string Comment1 { get; set; }
        public DateTime? CommentDate { get; set; } = DateTime.Now;
    }
}
