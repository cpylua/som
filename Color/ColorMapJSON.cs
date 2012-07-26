using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace FeatureGraph.SOM.Color
{
    /// <summary>
    /// Adapter class for converting a SOM map to JSON
    /// </summary>
    public sealed class ColorMapJSON
    {
        private Map m_Map;

        public ColorMapJSON(Map map) {
            m_Map = map;
        }

        public IEnumerable<Dictionary<string, dynamic>> cells {
            get {
                var result = new List<Dictionary<string, dynamic>>();
                m_Map.ForEachCell(cell => {
                    var attr = new Dictionary<string, dynamic>();
                    attr.Add("x", cell["x"]);
                    attr.Add("y", cell["y"]);
                    attr.Add("color", NVectorToColor(cell.Weight));
                    result.Add(attr);
                });
                return result;
            }
        }

        public string ToJSON() {
            var jss = new JavaScriptSerializer();
            return jss.Serialize(this);
        }


        string NVectorToColor(Vector vector) {
            var color = Vector.Multiply(vector, 255);
            return String.Format("#{0:X2}{1:X2}{2:X2}", (int)color[0], (int)color[1], (int)color[2]);
        }
    }
}