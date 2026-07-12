using Microsoft.EntityFrameworkCore;

namespace Al_Muzayyen.Models
{
    // تم تغيير الاسم إلى AppDbContext لتجنب التعارض مع كلاس النظام System.AppContext
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Admin> Admins { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Available_slot> Available_Slots { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<Place> Places { get; set; }
        public DbSet<Slot_time> Slot_Times { get; set; }
        public DbSet<Video> Videos { get; set; }

        // إضافة هذا الجزء لضمان استقرار العلاقات في الـ SQL Server ومنع مشاكل الـ Cascade Delete
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // التصحيح هنا: استخدام modelBuilder.Model مباشرة
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}