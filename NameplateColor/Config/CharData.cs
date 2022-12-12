using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NameplateColor.Config
{

    public class CharaData
    {
        public readonly string name;
        public readonly string world;

        public CharaData(string name, string world)
        {
            this.name = name;
            this.world = world;
        }
    }
}
