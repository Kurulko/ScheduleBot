using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Database.Models;

public class Teacher : DbModelWithToken
{
    public string? FirstName { get; set; }
    public string LastName { get; set; } = null!;
    public string? FatherName { get; set; }

    public IEnumerable<Conference>? Conferences { get; set; }
    public IEnumerable<Subject>? Subjects { get; set; }
    public IEnumerable<Event>? Events { get; set; }
}
