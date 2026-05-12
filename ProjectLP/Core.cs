using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectLP;

namespace ProjectLP
{
    public static class Core
    {
        public static UlianenkoprEntities Context = new UlianenkoprEntities();
        public static User CurrentUser { get; set; }
    }
}
