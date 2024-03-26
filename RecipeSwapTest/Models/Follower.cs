using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RecipeSwapTest.Models;

public partial class Follower
{
    [Key]
    [Column("FollowerID")]
    public int FollowerId { get; set; }

    [Column("FollowerUserID")]
    public int FollowerUserId { get; set; }

    [Column("FollowedUserID")]
    public int FollowedUserId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FollowDate { get; set; }

    [ForeignKey("FollowedUserId")]
    [InverseProperty("FollowerFollowedUsers")]
    public virtual User FollowedUser { get; set; } = null!;

    [ForeignKey("FollowerUserId")]
    [InverseProperty("FollowerFollowerUsers")]
    public virtual User FollowerUser { get; set; } = null!;
}
