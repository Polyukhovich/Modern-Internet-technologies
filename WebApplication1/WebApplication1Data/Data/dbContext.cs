using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplication1Data.Models;
using WebApplication1Data.models;

namespace WebApplication1Data.Data
{
    public class dbContext : IdentityDbContext<User>
    {
        public dbContext(DbContextOptions<dbContext> options) : base(options)
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

            // Лише найнеобхідніші індекси для полів, по яких часто шукаємо
            builder.Entity<ScheduleItem>()
                .HasIndex(s => s.GroupName);

            builder.Entity<Student>()
                .HasIndex(s => s.GroupName);

            // Лише конфігурації, де потрібна спеціальна поведінка
            // Зв'язок Material -> User з колекцією
            builder.Entity<Material>()
                .HasOne(m => m.UploadedBy)
                .WithMany(u => u.UploadedMaterials)
                .HasForeignKey(m => m.UploadedById)
                .OnDelete(DeleteBehavior.Restrict);

            // Специфічні налаштування видалення
            builder.Entity<Course>()
                .HasOne(c => c.Teacher)
                .WithMany(t => t.Courses)
                .HasForeignKey(c => c.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}