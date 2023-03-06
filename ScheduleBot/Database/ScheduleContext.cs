using Microsoft.EntityFrameworkCore;
using ScheduleBot.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Database;

public class ScheduleContext : DbContext
{
    public DbSet<Subject> Subjects { get; set; } = null!;
    public DbSet<Teacher> Teachers { get; set; } = null!;
    public DbSet<Conference> Conferences { get; set; } = null!;
    public DbSet<TimeLesson> TimeLessons { get; set; } = null!;
    //public DbSet<SchWeek> SchWeeks { get; set; } = null!;
    //public DbSet<SubjectTeacher> SubjectTeacher { get; set; } = null!;
    public DbSet<HW> HWs { get; set; } = null!;

    public ScheduleContext()
        => Database.EnsureCreated();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string connectionStr = "Server=(localdb)\\mssqllocaldb; Database=Schedule_v8; Trusted_Connection=True;";
        optionsBuilder.UseSqlServer(connectionStr);
    }
}
