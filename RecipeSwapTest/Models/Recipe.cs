using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RecipeSwapTest.Models;

public partial class Recipe
{
    [Key]
    [Column("RecipeID")]
    public int RecipeId { get; set; }

    [Column("UserID")]
    public int UserId { get; set; }

    [StringLength(100)]
    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string Ingredients { get; set; } = null!;

    public string Instructions { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime CreationDate { get; set; }

    public string? Image { get; set; }

    public bool IsVisible { get; set; }

    [InverseProperty("Recipe")]
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    [InverseProperty("Recipe")]
    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();

    [InverseProperty("Recipe")]
    public virtual ICollection<RecipeTag> RecipeTags { get; set; } = new List<RecipeTag>();

    [ForeignKey("UserId")]
    [InverseProperty("Recipes")]
    public virtual User User { get; set; } = null!;

    [InverseProperty("Recipe")]
    public virtual ICollection<Favorites> Favorites { get; set; } = new List<Favorites>();
}
