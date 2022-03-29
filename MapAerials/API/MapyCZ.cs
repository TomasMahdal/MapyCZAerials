using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

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

        /// <summary>
        /// get aerials Bitmap from Mapy.cz server
        /// </summary>
        /// <param name="x">axis x</param>
        /// <param name="y">asis y</param>
        /// <param name="z">axis z</param>
        /// <param name="mapType">used type of map</param>
        /// <returns></returns>
        public static Bitmap getAerials(string x, string y, string z, Structures.MapType mapType)
        {
            string url = String.Format("https://mapserver.mapy.cz/{0}/{1}-{2}-{3}", mapType.InternalName, z, x, y);
            Console.WriteLine(url);
            WebRequest request = System.Net.WebRequest.Create(url);
            WebResponse response = request.GetResponse();
            return new Bitmap(response.GetResponseStream());
        }
    }
}
