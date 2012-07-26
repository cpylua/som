using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FeatureGraph.SOM
{
    /// <summary>
    /// A cell in SOM map
    /// </summary>
    public class Cell
    {
        Dictionary<string, dynamic> m_AuxData;
        Vector m_Weight;

        public Cell(Vector weight) {
            m_Weight = weight;
            m_AuxData = new Dictionary<string, dynamic>();
        }

        public Vector Weight {
            get { return m_Weight; }
        }

        /// <summary>
        /// Indexer for auxiliary data assosiated with this cell.
        /// </summary>
        /// <param name="key">Key of the assosiated data.</param>
        /// <returns>The value of the auxiliary data under key</returns>
        public dynamic this[string key] {
            get {
                if (m_AuxData.ContainsKey(key)) {
                    return m_AuxData[key];
                } else {
                    throw new KeyNotFoundException(String.Format("Key {0} not found", key));
                }
            }

            set {
                if (m_AuxData.ContainsKey(key)) {
                    m_AuxData[key] = value;
                } else {
                    m_AuxData.Add(key, value);
                }
            }
        }

        public override bool Equals(object obj) {
            Cell that = obj as Cell;
            if (that == null) {
                return false;
            }

            if (!m_Weight.Equals(that.m_Weight)) {
                return false;
            }

            foreach (var key in this.m_AuxData.Keys) {
                var keys = that.m_AuxData.Keys;
                if (!keys.Contains(key) || !this.m_AuxData[key].Equals(that.m_AuxData[key])) {
                    return false;
                }
            }
            return true;
        }

        public override int GetHashCode() {
            int hash = 17;
            hash = hash * 37 + m_Weight.GetHashCode();
            foreach (var key in this.m_AuxData.Keys) {
                hash = hash * 37 + key.GetHashCode();
                hash = hash * 37 + this.m_AuxData[key].GetHashCode();
            }
            return hash;
        }
    }
}