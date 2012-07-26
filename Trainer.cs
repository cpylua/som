using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;

namespace FeatureGraph.SOM
{
    public delegate void TrainCompleteHandler(TrainCompleteArgs args);

    public delegate void TrainStepHandler(TrainStepArgs args);

    /// <summary>
    /// TrainStepHandler argument
    /// </summary>
    public class TrainStepArgs
    {
        private readonly Map m_Map;

        public TrainStepArgs(Map map) {
            m_Map = map;
        }

        public Map MapSnapshot {
            get { return m_Map; }
        }
    }

    /// <summary>
    /// TrainCompleteHandler argument
    /// </summary>
    public class TrainCompleteArgs
    {
        private readonly Map m_Map;

        public TrainCompleteArgs(Map map) {
            m_Map = map;
        }

        public Map ResultMap {
            get { return m_Map; }
        }
    }

    /// <summary>
    /// Train a map using inputs for N iterations.
    /// </summary>
    public sealed class Trainer
    {
        Map m_Map;
        IEnumerable<Vector> m_Inputs;
        Calculator m_Calculator;
        int m_TotalIterations;
        static Random r = new Random();


        /// <summary>
        /// Setup a train.
        /// </summary>
        /// <param name="map">SOM map</param>
        /// <param name="calc">Perform various calculations</param>
        public Trainer(Map map, Calculator calc) {
            m_Map = map;
            m_Calculator = calc;
        }

        /// <summary>
        /// Start this training.
        /// </summary>
        /// <param name="inputs">Train data</param>
        /// <param name="iterations">Number of iterations</param>
        /// <param name="done">Called when training finished</param>
        /// <param name="step">Celled before starting a new iteration</param>
        public void Start(IEnumerable<Vector> inputs, int iterations, TrainCompleteHandler done = null, TrainStepHandler step = null) {
            if (iterations <= 0) {
                throw new ArgumentException("number of iterations is negative");
            }
            m_TotalIterations = iterations;
            m_Inputs = inputs;

            for (var i = 1; i <= m_TotalIterations; i++) {
                if (step != null) {
                    step(new TrainStepArgs(m_Map));
                }
                ApplyInput(i);
            }

            if (done != null) {
                done(new TrainCompleteArgs(m_Map));
            }
        }

        /// <summary>
        /// Perform one iteration of training
        /// </summary>
        /// <param name="iteration">Current iteration</param>
        void ApplyInput(int iteration) {
            Vector input = Choice();
            Cell bmu = FindBMU(input);
            double radius = m_Calculator.NeighbourhoodRadius(iteration, m_TotalIterations);

            m_Map.ForEachCell(cell => {
                double distance = m_Calculator.MapDistanceOf(cell, bmu);
                // Adjuct cells within neighbourbood radius
                if (distance < radius) {
                    AjustCellWeight(cell, input, distance, radius, iteration);
                }
            });
        }

        /// <summary>
        /// Randomly choose a vector from a set of vectors.
        /// </summary>
        /// <returns>A randomly selected vector</returns>
        Vector Choice() {
            int i = r.Next(m_Inputs.Count());
            return m_Inputs.ElementAt(i);
        }

        /// <summary>
        /// Find the Best Matching Unit of input in map 
        /// </summary>
        /// <param name="input">Input vector</param>
        /// <returns>A cell in map</returns>
        Cell FindBMU(Vector input) {
            double minDistance = double.MaxValue;
            Cell bmu = null;
            m_Map.ForEachCell(cell => {
                double d = m_Calculator.DistanceOf(cell.Weight, input);
                if (d < minDistance) {
                    minDistance = d;
                    bmu = cell;
                }
            });
            Debug.Assert(bmu != null, "bmu not found");
            return bmu;
        }

        /// <summary>
        /// Ajust a cell's weight in map. 
        /// </summary>
        /// <param name="cell">The cell to be adjusted</param>
        /// <param name="input">The input vector</param>
        /// <param name="distance">The map distance between cell and current bmu cell</param>
        /// <param name="radius">Current neighbourhood radius</param>
        /// <param name="iteration">Current iteration</param>
        void AjustCellWeight(Cell cell, Vector input, double distance, double radius, int iteration) {
            /*
             * W(t+1) = W(t) + THETA(t) * L(t) * (V(t) - W(t))
             * t is current iteration
             * W(t) is old weight to be adjusted
             * W(t+1) is the new weight
             * THETA(t) is the neighbourhood influence function
             * L(t) is the learning rate
             * V(t) current input vector
             * */
            Vector w = cell.Weight;
            double theta = m_Calculator.Influence(cell, distance, radius);
            double learn = m_Calculator.LearningRate(iteration, m_TotalIterations);
            Vector diff = Vector.Subtract(input, w);
            w.Add(diff.Multiply(theta * learn));
        }
    }
}