using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RecipeSwapTest.Models;

public partial class RecipeTag
{
    [Key]
    [Column("RecipeTagID")]
    public int RecipeTagId { get; set; }

    [Column("RecipeID")]
    public int RecipeId { get; set; }

    [Column("TagID")]
    public int TagId { get; set; }

    [ForeignKey("RecipeId")]
    [InverseProperty("RecipeTags")]
    public virtual Recipe Recipe { get; set; } = null!;

    [ForeignKey("TagId")]
    [InverseProperty("RecipeTags")]
    public virtual Tag Tag { get; set; } = null!;
}
