using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RecipeSwapTest.Models;

public partial class Comment
{
    [Key]
    [Column("CommentID")]
    public int CommentId { get; set; }

    [Column("UserID")]
    public int UserId { get; set; }

    [Column("RecipeID")]
    public int RecipeId { get; set; }

    [Column("Comment")]
    public string Comment1 { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? CommentDate { get; set; }

    [ForeignKey("RecipeId")]
    [InverseProperty("Comments")]
    public virtual Recipe Recipe { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("Comments")]
    public virtual User User { get; set; } = null!;
}
