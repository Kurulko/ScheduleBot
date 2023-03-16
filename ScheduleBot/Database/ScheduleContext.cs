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
    public DbSet<Break> Breaks { get; set; } = null!;
    public DbSet<Event> Events { get; set; } = null!;
    public DbSet<TelegramChat> Chats { get; set; } = null!;
    public DbSet<Token> Tokens { get; set; } = null!;

    public ScheduleContext()
        => Database.EnsureCreated();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Token>().HasMany(t => t.Events).WithOne(hw => hw.Token).OnDelete(DeleteBehavior.NoAction);
        builder.Entity<Token>().HasMany(t => t.Teachers).WithOne(hw => hw.Token).OnDelete(DeleteBehavior.NoAction);
        builder.Entity<Token>().HasMany(t => t.Subjects).WithOne(hw => hw.Token).OnDelete(DeleteBehavior.NoAction);
        builder.Entity<Token>().HasMany(t => t.Conferences).WithOne(hw => hw.Token).OnDelete(DeleteBehavior.NoAction);
        builder.Entity<Token>().HasMany(t => t.TimeLessons).WithOne(hw => hw.Token).OnDelete(DeleteBehavior.NoAction);
        builder.Entity<Token>().HasMany(t => t.Breaks).WithOne(hw => hw.Token).OnDelete(DeleteBehavior.NoAction);
        builder.Entity<Token>().HasMany(t => t.Chats).WithOne(hw => hw.Token).OnDelete(DeleteBehavior.NoAction);

        base.OnModelCreating(builder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string connectionStr = "Server=(localdb)\\mssqllocaldb; Database=Schedule_v_3; Trusted_Connection=True; MultipleActiveResultSets=true";
        optionsBuilder.UseSqlServer(connectionStr);
    }
}
