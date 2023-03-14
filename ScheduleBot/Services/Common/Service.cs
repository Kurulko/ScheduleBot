using Microsoft.EntityFrameworkCore;
using ScheduleBot.Database;
using ScheduleBot.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Services;

public abstract class Service<T> where T : DbModel
{
    protected ScheduleContext db = new();

    public abstract DbSet<T> GetAllModels();
    public virtual IEnumerable<T> GetModels()
        => GetAllModels().ToList();

    public T? GetModelById(long id)
        => GetModels().FirstOrDefault(b => b.Id == id);

    public void UpdateModel(T model)
    {
        GetAllModels().Update(model);
        db.SaveChanges();
    }
    public void AddModel(T model)
    {
        GetAllModels().Add(model);
        db.SaveChanges();
    }
    public void AddModels(IEnumerable<T> models)
    {
        GetAllModels().AddRange(models);
        db.SaveChanges();
    }
    public void RemoveModel(T model)
    {
        GetAllModels().Remove(model);
        db.SaveChanges();
    }
    public void RemoveModels(IEnumerable<T> models)
    {
        GetAllModels().RemoveRange(models);
        db.SaveChanges();
    }
    public bool AnyModels()
        => GetModels().Any();
}
