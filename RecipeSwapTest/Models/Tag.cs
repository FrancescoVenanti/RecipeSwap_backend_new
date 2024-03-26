using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RecipeSwapTest.Models;

[Index("TagName", Name = "UQ__Tags__BDE0FD1D5801735F", IsUnique = true)]
public partial class Tag
{
    [Key]
    [Column("TagID")]
    public int TagId { get; set; }

    [StringLength(50)]
    public string TagName { get; set; } = null!;

    [InverseProperty("Tag")]
    public virtual ICollection<RecipeTag> RecipeTags { get; set; } = new List<RecipeTag>();
}
