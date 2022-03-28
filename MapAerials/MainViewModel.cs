using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapAerials
{
    class MainViewModel
    {
        /// <summary>
        /// Supported map types by Mapy.cz API
        /// </summary>
        public List<Structures.MapType> SupportedMapTypes
        {
            get
            {
                return API.MapyCZ.SupportedMapTypes;
            }
        }
    }
}
