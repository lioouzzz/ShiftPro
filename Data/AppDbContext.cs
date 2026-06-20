using Microsoft.EntityFrameworkCore;
using ShiftPro.Models;

namespace ShiftPro.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet <Schedule>Schedules { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Schedule>()
                                   .HasOne(x => x.Employee)
                                   .WithMany(x => x.Schedules)
                                   .HasForeignKey(x => x.EmployeeId);

            //產生唯一索引 (unique Index)
            modelBuilder.Entity<Schedule>()
                        .HasIndex(x => new { x.EmployeeId, x.WorkDate })
                        .IsUnique();


            //modelBuilder.Entity<Employee>()
            //            .HasIndex(x => x.Account)
            //            .IsUnique();
        }



    }
}
