using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;

namespace FeatureGraph.SOM.Color
{
    public sealed class ColorSOM : Calculator
    {
        private int m_MapWidth, m_MapHeight;
        private Trainer m_Trainer;
        private double MAX_RADIUS;
        private double RADIUS_CONSTANT;
        private const double MAX_LEARNING_RATE = 0.5;

        private const int VECTOR_DIMENSION = 3;

        public ColorSOM(int width, int height) {
            m_MapHeight = height;
            m_MapWidth = width;
            MAX_RADIUS = (height > width ? width : height) / 2.0;
            RADIUS_CONSTANT = 1.0 / Math.Log(MAX_RADIUS);

            InitializeTrainer();
        }

        public void Start(IEnumerable<Vector> inputs, int iterations, TrainCompleteHandler done) {
            m_Trainer.Start(inputs, iterations, done);
        }

        /// <summary>
        /// Calculate the Euclidean distance between two vectors
        /// </summary>
        /// <param name="cell">Vector x</param>
        /// <param name="input">Verctor y</param>
        /// <returns>The distance between them.</returns>
        public double DistanceOf(Vector x, Vector y) {
            Debug.Assert(x.Dimension == y.Dimension, "vector dimensions not match");

            var diff = Vector.Subtract(x, y);
            double distance = 0;
            for (var i = 0; i != diff.Dimension; i++) {
                distance += Math.Pow(diff[i], 2);
            }
            return Math.Sqrt(distance);
        }

        /// <summary>
        /// Calculate the Euclidean distance between two cells on map.
        /// This distance is not the distance of their weight vectors.
        /// </summary>
        /// <param name="x">Cell x</param>
        /// <param name="y">Cell y</param>
        /// <returns>The distance between them on map.</returns>
        public double MapDistanceOf(Cell x, Cell y) {
            int x1 = x["x"];
            int y1 = x["y"];
            int x2 = y["x"];
            int y2 = y["y"];
            return Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
        }

        /// <summary>
        /// Calculate the current neighbourhood radius.
        /// The radius should start large, and decreases over time.
        /// </summary>
        /// <param name="iteration">Current iteration</param>
        /// <param name="total">Total iterations of this training</param>
        /// <returns>Neighbourhood radius</returns>
        public double NeighbourhoodRadius(int iteration, int total) {
            return MAX_RADIUS * Math.Exp(-iteration / (total * RADIUS_CONSTANT));
        }

        /// <summary>
        /// Calculate the influence that current input has on cell.
        /// </summary>
        /// <param name="cell">A cell in map</param>
        /// <param name="distance">The distance between cell and current BMU</param>
        /// <param name="radius">The current neighbourhood radius</param>
        /// <returns>An influence between 0 and 1</returns>
        public double Influence(Cell cell, double distance, double radius) {
            return Math.Exp(-Math.Pow(distance, 2) / Math.Pow(radius, 2));
        }

        /// <summary>
        /// Calculate the learning rate of current iteration.
        /// </summary>
        /// <param name="iteration">Current iteration</param>
        /// <param name="total">Total number of iterations in this training.</param>
        /// <returns>A learning rate between 0 and 1.</returns>
        public double LearningRate(int iteration, int total) {
            return MAX_LEARNING_RATE * Math.Exp(-(double)iteration / (double)total);
        }

        void InitializeTrainer() {
            var cells = new List<Cell>();
            for (var i = 0; i != m_MapWidth; i++) {
                for (var j = 0; j != m_MapHeight; j++) {
                    Vector vec = new Vector(VECTOR_DIMENSION);
                    Cell cell = new Cell(vec);
                    cell["x"] = i;
                    cell["y"] = j;
                    cells.Add(cell);
                }
            }
            var map = new Map(cells);
            m_Trainer = new Trainer(map, this);
        }
    }
}