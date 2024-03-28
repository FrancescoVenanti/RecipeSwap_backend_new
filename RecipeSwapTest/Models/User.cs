using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RecipeSwapTest.Models;

[Index("Username", Name = "UQ__Users__536C85E47E9B8CE4", IsUnique = true)]
[Index("Email", Name = "UQ__Users__A9D10534BB7F8A1B", IsUnique = true)]
public partial class User
{
    [Key]
    [Column("UserID")]
    public int UserId { get; set; }

    [StringLength(50)]
    public string Username { get; set; } = null!;

    [StringLength(255)]
    public string Email { get; set; } = null!;

    public string? ProfilePicture { get; set; }

    public string? Bio { get; set; }

    public DateOnly RegistrationDate { get; set; }

    [StringLength(256)]
    public string PswHash { get; set; } = null!;

    [StringLength(50)]
    public string FirstName { get; set; } = null!;

    [StringLength(50)]
    public string LastName { get; set; } = null!;

    public bool VerifiedEmail { get; set; }

    public string? TokenConfirmEmail { get; set; }

    [StringLength(20)]
    public string Role { get; set; } = null!;

    [InverseProperty("User")]
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    [InverseProperty("FollowedUser")]
    public virtual ICollection<Follower> FollowerFollowedUsers { get; set; } = new List<Follower>();

    [InverseProperty("FollowerUser")]
    public virtual ICollection<Follower> FollowerFollowerUsers { get; set; } = new List<Follower>();

    [InverseProperty("User")]
    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();

    [InverseProperty("ReceiverUser")]
    public virtual ICollection<Message> MessageReceiverUsers { get; set; } = new List<Message>();

    [InverseProperty("SenderUser")]
    public virtual ICollection<Message> MessageSenderUsers { get; set; } = new List<Message>();

    [InverseProperty("User")]
    public virtual ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();

    [InverseProperty("User")]
    public ICollection<Favorites> Favorites { get; set; }= new List<Favorites>();
}
