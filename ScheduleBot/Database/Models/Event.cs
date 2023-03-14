using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Database.Models;

public class Event : DbModelWithToken
{
    public DateTime Deadline { get; set; }
    public DateTime? WasGivenDate { get; set; }
    public string? Description { get; set; }
    public TypeOfEvent TypeOfEvent { get; set; }

    public long SubjectId { get; set; }
    public Subject? Subject { get; set; }
    public long TeacherId { get; set; }
    public Teacher? Teacher { get; set; }
}

[Flags]
public enum TypeOfEvent
{
    Homework, ControlWork, LaboratoryWork, Test, Exam, Examination, Credit, Meeting
}
