using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace VisGitCore.Helpers
{
    internal static class Date
    {
        public static DateTime ToDateTime(this DateTimeOffset? dateTimeOffset) => ((DateTimeOffset)dateTimeOffset).DateTime;

        public static DateTimeOffset? ToDateTimeOffset(this DateTime dateTime) => dateTime.ToUniversalTime();
    }
}