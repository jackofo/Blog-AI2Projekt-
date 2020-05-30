using System;
using System.Collections.Generic;
using System.Text;
using Blog.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Blog.Data
{
	public class ApplicationDbContext : IdentityDbContext
	{
		public DbSet<Post> Posts { get; set; }
		public DbSet<Tag> Tags { get; set; }

		public DbSet<PostsTags> PostsTags { get; set; }
		public DbSet<Category> Categories { get; set; }
		public DbSet<Comment> Comments { get; set; }

		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}

		public ApplicationDbContext() : base()
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// any guid
			const string ADMIN_ID = "a18be9c0-aa65-4af8-bd17-00bd9344e575";
			const string USER_ID = "4089681A-6623-4F50-973B-F3A9DD58F60C";
			modelBuilder.Entity<IdentityRole>().HasData(
				new IdentityRole
				{
					Id = ADMIN_ID,
					Name = "admin",
					NormalizedName = "admin".ToUpper()
				},
				new IdentityRole
				{
					Id = USER_ID,
					Name = "user",
					NormalizedName = "user".ToUpper()
				}
			);

			var hasher = new PasswordHasher<IdentityUser>();
			modelBuilder.Entity<IdentityUser>().HasData(
				new IdentityUser
				{
					Id = ADMIN_ID,
					UserName = "admin@fake.com",
					NormalizedUserName = "admin@fake.com".ToUpper(),
					Email = "admin@fake.com",
					NormalizedEmail = "admin@fake.com".ToUpper(),
					EmailConfirmed = true,
					PasswordHash = hasher.HashPassword(null, "admin"),
					SecurityStamp = string.Empty
				},
				new IdentityUser
				{
					Id = USER_ID,
					UserName = "user@fake.com",
					NormalizedUserName = "user@fake.com".ToUpper(),
					Email = "user@fake.com",
					NormalizedEmail = "user@fake.com".ToUpper(),
					EmailConfirmed = true,
					PasswordHash = hasher.HashPassword(null, "user"),
					SecurityStamp = string.Empty
				}
			);

			modelBuilder.Entity<IdentityUserRole<string>>().HasData(
				new IdentityUserRole<string>
				{
					RoleId = ADMIN_ID,
					UserId = ADMIN_ID
				},
				new IdentityUserRole<string>
				{
					RoleId = USER_ID,
					UserId = USER_ID
				}
			);

			modelBuilder.Entity<Category>().HasData(
				new Category
				{
					Id = Guid.NewGuid(),
					Name = "Annoucement"
				}
			);
		}
	}
}
