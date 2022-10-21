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
        private static string API_endpoint = "https://mapserver.mapy.cz/{0}/{1}-{2}-{3}";

        /// <summary>
        /// List of all supported map types
        /// </summary>
        public static List<Structures.MapType> SupportedMapTypes
        {
            get {
                List<Structures.MapType> mapList = new List<Structures.MapType>();
                mapList.Add(new Structures.MapType(1, "ophoto-m", Languages.GetString("map_ophoto-m")));
                mapList.Add(new Structures.MapType(2, "turist-m", Languages.GetString("map_turist-m")));
                mapList.Add(new Structures.MapType(3, "zemepis-m", Languages.GetString("map_zemepis-m")));

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
            try
            {
                // generate URL
                string url = String.Format(API_endpoint, mapType.InternalName, z, x, y);

                // get response
                WebRequest request = System.Net.WebRequest.Create(url);
                WebResponse response = request.GetResponse();

                // convert image data to bitmap
                return new Bitmap(response.GetResponseStream());
            } catch (System.Net.WebException)
            {
                return null;
            }
        }
    }
}
