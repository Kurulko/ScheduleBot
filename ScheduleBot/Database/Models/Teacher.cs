﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Database.Models;

public class Teacher
{
    public long Id { get; set; }
    public string? FirstName { get; set; }
    public string LastName { get; set; } = null!;
    public string? FatherName { get; set; }

    public long TokenId { get; set; }
    public Token? Token { get; set; }

    public IEnumerable<Conference>? Conferences { get; set; }
    public IEnumerable<Subject>? Subjects { get; set; }
    public IEnumerable<HW>? HWs { get; set; }
}
