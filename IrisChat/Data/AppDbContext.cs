﻿using IrisChat.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace IrisChat.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected AppDbContext() { }

        public DbSet<ForumThread> ForumThreads { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().ToTable("AspNetUsers");

            modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
            modelBuilder.Entity<ForumThread>().HasQueryFilter(t => !t.IsDeleted);
            modelBuilder.Entity<Post>().HasQueryFilter(p => !p.IsDeleted);
            modelBuilder.Entity<Category>().HasQueryFilter(c => !c.IsDeleted);

            // Ограниченное удаление постов при удалении ветки
            modelBuilder.Entity<Post>()
                .HasOne(p => p.ForumThread)
                .WithMany(t => t.Posts)
                .HasForeignKey(p => p.ThreadId)
                .OnDelete(DeleteBehavior.Restrict);

            // Ограниченное удаление веток при удалении категории
            modelBuilder.Entity<ForumThread>()
                .HasOne(t => t.Category)
                .WithMany(c => c.ForumThreads)
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Ограниченное удаление постов при удалении пользователя
            modelBuilder.Entity<Post>()
                .HasOne(p => p.Author)
                .WithMany(u => u.Posts)
                .HasForeignKey(p => p.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);


            // Ограниченное удаление веток при удалении пользователя
            modelBuilder.Entity<ForumThread>()
                .HasOne(t => t.Author)
                .WithMany(u => u.ForumThreads)
                .HasForeignKey(t => t.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
