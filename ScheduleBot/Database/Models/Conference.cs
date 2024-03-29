﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ScheduleBot.Database.Models;

public class Conference : DbModelWithToken
{
    public string? Link { get; set; }

    public IEnumerable<TimeLesson>? TimeLessons { get; set; }
    public long? SubjectId { get; set; }
    public Subject? Subject { get; set; }
    public long? TeacherId { get; set; }
    public Teacher? Teacher { get; set; }
}
