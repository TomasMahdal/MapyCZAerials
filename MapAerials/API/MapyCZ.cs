using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapAerials.API
{
    /// <summary>
    /// work with Mapy.cz API
    /// if you want more info, look here: https://api.mapy.cz/
    /// </summary>
    class MapyCZ
    {
        /// <summary>
        /// List of all supported map types
        /// </summary>
        public static List<Structures.MapType> SupportedMapTypes
        {
            get {
                List<Structures.MapType> mapList = new List<Structures.MapType>();
                mapList.Add(new Structures.MapType(1, "ophoto-m", "letecká"));
                mapList.Add(new Structures.MapType(2, "turist-m", "turistická"));
                mapList.Add(new Structures.MapType(3, "zemepis-m", "zeměpisná mapa"));

                return mapList;
            }
        }
    }
}
