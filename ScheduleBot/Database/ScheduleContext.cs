using Microsoft.EntityFrameworkCore;
using ScheduleBot.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace ScheduleBot.Database;

public class ScheduleContext : DbContext
{
    public DbSet<Subject> Subjects { get; set; } = null!;
    public DbSet<Teacher> Teachers { get; set; } = null!;
    public DbSet<Conference> Conferences { get; set; } = null!;
    public DbSet<TimeLesson> TimeLessons { get; set; } = null!;
    //public DbSet<SubjectTeacher> SubjectTeacher { get; set; } = null!;
    public DbSet<HW> HWs { get; set; } = null!;

    public ScheduleContext()
        => Database.EnsureCreated();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TimeLesson>()
            .HasOne(tl => tl.Subject)
            .WithMany(s => s.TimeLessons)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<TimeLesson>()
            .HasOne(tl => tl.Teacher)
            .WithMany(t => t.TimeLessons)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<TimeLesson>()
            .HasOne(tl => tl.Conference)
            .WithMany(c => c.TimeLessons)
            .OnDelete(DeleteBehavior.NoAction);

        base.OnModelCreating(modelBuilder);
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string connectionStr = "Server=(localdb)\\mssqllocaldb; Database=Schedule_v_1; Trusted_Connection=True;";
        optionsBuilder.UseSqlServer(connectionStr);
    }
}
