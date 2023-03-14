using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Extensions;

public static class StringExtensions
{
    public static IEnumerable<string> DevideStrIfMoreMaxLength(this string str, int maxLength)
    {
        int strLength = str.Length;
        if(strLength > maxLength)
        {
            int count = Convert.ToInt32(Math.Ceiling((double)strLength / maxLength));

            IList<string> res = new List<string>();
            for (int i = 0, sumOfLength = 0; i < count; i++, sumOfLength += maxLength)
            {
                if(i != count - 1)
                    res.Add(string.Concat(str.Skip(i * maxLength).Take(maxLength)));
                else
                    res.Add(string.Concat(str.Skip(i * maxLength).Take(strLength - sumOfLength)));
            }

            return res;
        }

        return new List<string> { str };
    }
}
