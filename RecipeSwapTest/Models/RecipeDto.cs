namespace RecipeSwapTest.Models
{
    public class RecipeDto
    {
        public int UserId { get; set; }
        public string Title { get; set; }
        public string Instructions { get; set; }
        public string Ingredients { get; set; }
        public string Description { get; set; }

        public IFormFile? Image { get; set; }
    }
}
