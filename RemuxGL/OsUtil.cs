using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemuxGL
{
    public class OsUtil
    {
        public static bool IsWindows() => !IsUnix();

        public static bool IsUnix()
        {
            int id = (int)Environment.OSVersion.Platform;
            return (id == 4) || (id == 6) || (id == 128);
        }
    }
}
