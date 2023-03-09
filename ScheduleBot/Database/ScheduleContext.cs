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
    public DbSet<HW> HWs { get; set; } = null!;
    public DbSet<TelegramChat> Chats { get; set; } = null!;

    public ScheduleContext()
        => Database.EnsureCreated();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string connectionStr = "Server=(localdb)\\mssqllocaldb; Database=Schedule_v_3; Trusted_Connection=True; MultipleActiveResultSets=true";
        optionsBuilder.UseSqlServer(connectionStr);
    }
}
