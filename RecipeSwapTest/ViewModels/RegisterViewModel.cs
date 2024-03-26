using System.ComponentModel.DataAnnotations;
namespace RecipeSwapTest.ViewModels
{
    public class RegisterViewModel
    {
        [StringLength(50)]
        public string Username { get; set; } = null!;
        [StringLength(255)]
        public string Email { get; set; } = null!;
        [StringLength(256)]
        public string PswHash { get; set; } = null!;
        [StringLength(255)]
        public string ConfirmPsw { get; set; } = null!;
        [StringLength(50)]
        public string FirstName { get; set; } = null!;
        [StringLength(50)]
        public string LastName { get; set; } = null!;
        [StringLength(255)]
        public string? ProfilePicture { get; set; }
        public string? Bio { get; set; }
        public string? TokenConfirmEmail { get; set; }

    }
}
