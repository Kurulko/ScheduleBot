using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Extensions;

public static class EnumerableExtensions
{
    public static bool IsNotNullOrEmpty<T>(this IEnumerable<T>? values)
        => values is not null && values.Any();
}
