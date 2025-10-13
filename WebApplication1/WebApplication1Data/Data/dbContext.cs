using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication1Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplication1Data.models;


namespace WebApplication1Data.Data
{
    public class dbContext : IdentityDbContext<User>
    {
        public dbContext(DbContextOptions<dbContext> options)
        : base(options)
        {
        }
        public DbSet<Student> Students { get; set; } = default!;
        public DbSet<Teacher> Teachers { get; set; } = default!;
        public DbSet<Course> Courses { get; set; } = default!;
        public DbSet<Grade> Grades { get; set; } = default!;
        public DbSet<Material> Materials { get; set; } = default!;
        public DbSet<ScheduleItem> ScheduleItems { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Наприклад, щоб уникнути каскадного видалення між Student і Grade
            builder.Entity<Grade>()
                .HasOne(g => g.Student)
                .WithMany(s => s.Grades)
                .HasForeignKey(g => g.StudentId)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
