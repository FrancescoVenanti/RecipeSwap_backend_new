using System.ComponentModel.DataAnnotations;

namespace RecipeSwapTest.ViewModels
{
    public class LoginViewModel
    {
        [StringLength(50)]
        public string Username { get; set; } = null!;
        [StringLength(255)]
        public string PswHash { get; set; } = null!;
    }
}
