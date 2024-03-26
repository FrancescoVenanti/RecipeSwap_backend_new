using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RecipeSwapTest.Models;

public partial class Like
{
    [Key]
    [Column("LikeID")]
    public int LikeId { get; set; }

    [Column("UserID")]
    public int UserId { get; set; }

    [Column("RecipeID")]
    public int RecipeId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LikeDate { get; set; }

    [ForeignKey("RecipeId")]
    [InverseProperty("Likes")]
    public virtual Recipe Recipe { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("Likes")]
    public virtual User User { get; set; } = null!;
}
