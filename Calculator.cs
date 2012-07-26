using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FeatureGraph.SOM
{
    /// <summary>
    /// Perform train actions during a train.
    /// </summary>
    public interface Calculator
    {
        /// <summary>
        /// Calculate the distance between two vectors
        /// </summary>
        /// <param name="cell">Vector x</param>
        /// <param name="input">Verctor y</param>
        /// <returns>The distance between them.</returns>
        double DistanceOf(Vector x, Vector y);

        /// <summary>
        /// Calculate the distance between two cells on map.
        /// This distance is not the distance of their weight vectors.
        /// </summary>
        /// <param name="x">Cell x</param>
        /// <param name="y">Cell y</param>
        /// <returns>The distance between them on map.</returns>
        double MapDistanceOf(Cell x, Cell y);

        /// <summary>
        /// Calculate the current neighbourhood radius.
        /// The radius should start large, and decreases over time.
        /// </summary>
        /// <param name="iteration">Current iteration</param>
        /// <param name="total">Total iterations of this training</param>
        /// <returns>Neighbourhood radius</returns>
        double NeighbourhoodRadius(int iteration, int total);

        /// <summary>
        /// Calculate the influence that current input has on cell.
        /// </summary>
        /// <param name="cell">A cell in map</param>
        /// <param name="distance">The distance between cell and current BMU</param>
        /// <param name="radius">The current neighbourhood radius</param>
        /// <returns>An influence between 0 and 1</returns>
        double Influence(Cell cell, double distance, double radius);

        /// <summary>
        /// Calculate the learning rate of current iteration.
        /// </summary>
        /// <param name="iteration">Current iteration</param>
        /// <param name="total">Total number of iterations in this training.</param>
        /// <returns>A learning rate between 0 and 1.</returns>
        double LearningRate(int iteration, int total);
    }
}