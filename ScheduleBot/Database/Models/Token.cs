using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Database.Models;

public class Token : DbModel
{
    public string Name { get; set; } = null!;
    
    public IEnumerable<Subject>? Subjects { get; set; }
    public IEnumerable<Teacher>? Teachers { get; set; }
    public IEnumerable<TimeLesson>? TimeLessons { get; set; }
    public IEnumerable<Conference>? Conferences { get; set; }
    public IEnumerable<Break>? Breaks { get; set; }
    public IEnumerable<Event>? Events { get; set; }
    public IEnumerable<TelegramChat>? Chats { get; set; }

}
