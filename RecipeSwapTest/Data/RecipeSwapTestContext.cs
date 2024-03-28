using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using RecipeSwapTest.Models;

namespace RecipeSwapTest.Data;

public partial class RecipeSwapTestContext : DbContext
{
    public RecipeSwapTestContext()
    {
    }

    public RecipeSwapTestContext(DbContextOptions<RecipeSwapTestContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Follower> Followers { get; set; }

    public virtual DbSet<Like> Likes { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Recipe> Recipes { get; set; }

    public virtual DbSet<RecipeTag> RecipeTags { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Favorites> Favorites { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-CL58GCD\\SQLEXPRESS;Initial Catalog=RecipeSwapTest;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK__Comments__C3B4DFAA7D7362CB");

            entity.Property(e => e.CommentDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Recipe).WithMany(p => p.Comments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Comments__Recipe__4222D4EF");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Comments__UserID__412EB0B6");
        });

        modelBuilder.Entity<Follower>(entity =>
        {
            entity.HasKey(e => e.FollowerId).HasName("PK__Follower__E85940F9CBF7F806");

            entity.Property(e => e.FollowDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.FollowedUser).WithMany(p => p.FollowerFollowedUsers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Followers__Follo__4BAC3F29");

            entity.HasOne(d => d.FollowerUser).WithMany(p => p.FollowerFollowerUsers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Followers__Follo__4AB81AF0");
        });

        modelBuilder.Entity<Like>(entity =>
        {
            entity.HasKey(e => e.LikeId).HasName("PK__Likes__A2922CF41339BDF1");

            entity.Property(e => e.LikeDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Recipe).WithMany(p => p.Likes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Likes__RecipeID__46E78A0C");

            entity.HasOne(d => d.User).WithMany(p => p.Likes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Likes__UserID__45F365D3");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("PK__Messages__C87C037CA4DB1617");

            entity.Property(e => e.IsRead).HasDefaultValue(false);
            entity.Property(e => e.Timestamp).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.ReceiverUser).WithMany(p => p.MessageReceiverUsers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Messages__Receiv__5812160E");

            entity.HasOne(d => d.SenderUser).WithMany(p => p.MessageSenderUsers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Messages__Sender__571DF1D5");
        });

        modelBuilder.Entity<Recipe>(entity =>
        {
            entity.HasKey(e => e.RecipeId).HasName("PK__Recipes__FDD988D0D7074590");

            entity.Property(e => e.CreationDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsVisible).HasDefaultValue(true);

            entity.HasOne(d => d.User).WithMany(p => p.Recipes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Recipes__UserID__3D5E1FD2");
        });

        modelBuilder.Entity<RecipeTag>(entity =>
        {
            entity.HasKey(e => e.RecipeTagId).HasName("PK__RecipeTa__C4311431BE019E71");

            entity.HasOne(d => d.Recipe).WithMany(p => p.RecipeTags)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RecipeTag__Recip__5165187F");

            entity.HasOne(d => d.Tag).WithMany(p => p.RecipeTags)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RecipeTag__TagID__52593CB8");
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.TagId).HasName("PK__Tags__657CFA4CDF616762");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC0F399540");

            entity.Property(e => e.RegistrationDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Role).HasDefaultValueSql("(user_name())");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
