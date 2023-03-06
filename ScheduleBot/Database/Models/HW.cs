using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Database.Models;

public class HW
{
    public long Id { get; set; }
    public DateTime Deadline { get; set; }
    public DateTime? WasGivenDate { get; set; }
    public string Description { get; set; } = null!;

    public long SubjectId { get; set; }
    public Subject? Subject { get; set; }
    public long TeacherId { get; set; }
    public Teacher? Teacher { get; set; }
}

