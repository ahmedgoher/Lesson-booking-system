using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Al_Muzayyen.Models
{
    // التغيير هنا: الوراثة من IdentityDbContext<ApplicationUser> لتفعيل نظام الحسابات والأدوار
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // --- جداول حسابات وتفاصيل المستخدمين ---
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Student> Students { get; set; } // تم تعديل الاسم من Bookings إلى Students ليكون أوضح

        // --- جداول الصفوف والمكان والمجموعات ---
        public DbSet<Place> Places { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<Available_slot> Available_Slots { get; set; }
        public DbSet<Slot_time> Slot_Times { get; set; }

        // --- جداول المتابعة والغياب والمواد الشارحة ---
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<Video> Videos { get; set; }

        // --- جداول الامتحانات والتصحيح التلقائي ---
        public DbSet<Exam> Exams { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionOption> QuestionOptions { get; set; }
        public DbSet<StudentExam> StudentExams { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ضروري جداً لإنشاء جداول الـ Identity تلقائياً (AspNetUsers, AspNetRoles, etc.)
            base.OnModelCreating(modelBuilder);

            // كود منع الـ Cascade Delete لتجنب مشاكل Multiple Cascade Paths في SQL Server
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
            modelBuilder.Entity<QuestionOption>()
    .HasOne(o => o.Question)
    .WithMany(q => q.Options)
    .HasForeignKey(o => o.QuestionId)
    .OnDelete(DeleteBehavior.Cascade); // 👈 حذف الخيارات تلقائياً عند حذف السؤال
        }

    }
}