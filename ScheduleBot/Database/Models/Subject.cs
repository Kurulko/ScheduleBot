using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Database.Models;

public class Subject
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    public IEnumerable<Conference>? Conferences { get; set; }
    public IEnumerable<Teacher>? Teachers { get; set; }

    public IEnumerable<HW>? HWs { get; set; }
}

