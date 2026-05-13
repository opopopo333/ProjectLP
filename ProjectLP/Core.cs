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
        public static UlianenkoprEntities2 Context = new UlianenkoprEntities2();
        public static Users CurrentUser { get; set; }
    }
}
