using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Database.Initialize.Excel;

public class ExcelData
{
    [JsonProperty("range")]
    public string? Range { get; set; }

    [JsonProperty("majorDimension")]
    public string? MajorDimension { get; set; }

    [JsonProperty("values")]
    public List<List<string>> Values { get; set; } = null!;
}
