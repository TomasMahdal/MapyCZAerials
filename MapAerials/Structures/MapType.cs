using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapAerials.Structures
{
    /// <summary>
    /// Map type which is used by Mapy.cz API
    /// </summary>
    public class MapType
    {
        public int ID { get; private set; }
        public string InternalName { get; private set; }
        public string VisibleName { get; private set; }

        public MapType(int iD, string internalName, string visibleName)
        {
            ID = iD;
            InternalName = internalName;
            VisibleName = visibleName;
        }

        public override string ToString()
        {
            return VisibleName;
        }
    }
}
