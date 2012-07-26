using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FeatureGraph.SOM
{
    public class Map
    {
        IEnumerable<Cell> m_Cells;

        public Map(IEnumerable<Cell> cells) {
            m_Cells = cells;
        }

        /// <summary>
        /// Performs a specific action on each cell.
        /// </summary>
        /// <param name="action">The action to perform on each cell.</param>
        public void ForEachCell(Action<Cell> action) {
            if (action == null) {
                return;
            }
            foreach (var cell in m_Cells) {
                action(cell);
            }
        }
    }
}