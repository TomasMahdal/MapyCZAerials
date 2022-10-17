using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapAerials.Structures
{
    /// <summary>
    /// Special links are used e.g. in LOTUS, which can handle multiple sources of aerials
    /// </summary>
    public class SpecialLink
    {
        /// <summary>
        /// URL of mapType
        /// </summary>
        public string Url { get; private set; }
        public string MapType { get; private set; }

        public SpecialLink(string url, string mapType)
        {
            Url = url;
            MapType = mapType;
        }
    }
}
