using ScheduleBot.Database.Models;
using ScheduleBot.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ScheduleBot.Services.Common;

public class TeacherService : Service<Teacher>
{
    public override DbSet<Teacher> GetAllModels()
        => db.Teachers;
}