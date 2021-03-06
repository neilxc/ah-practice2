﻿using System;
using Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    public class DataContext : IdentityDbContext<AppUser, IdentityRole<int>, int>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Value> Values { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<ActivityAttendee> ActivityAttendees { get; set; }
        public DbSet<FollowedPeople> FollowedPeople { get; set; }
        public DbSet<UserPhoto> UserPhotos { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Value>()
                .HasData(
                    new Value { Id = 1, Name = "Value 101" },
                    new Value { Id = 2, Name = "Value 102" },
                    new Value { Id = 3, Name = "Value 103" }
                );

            builder.Entity<ActivityAttendee>(b =>
            {
                b.HasKey(aa => new { aa.ActivityId, aa.AppUserId });

                b.HasOne(a => a.Activity)
                    .WithMany(x => x.Attendees)
                    .HasForeignKey(a => a.ActivityId)
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasOne(a => a.AppUser)
                    .WithMany(x => x.Activities)
                    .HasForeignKey(a => a.AppUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<FollowedPeople>(b =>
            {
                b.HasKey(k => new { k.ObserverId, k.TargetId });
                b.HasOne(o => o.Observer)
                    .WithMany(f => f.Followers)
                    .HasForeignKey(o => o.ObserverId)
                    .OnDelete(DeleteBehavior.Restrict);
                b.HasOne(t => t.Target)
                    .WithMany(f => f.Following)
                    .HasForeignKey(t => t.TargetId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
