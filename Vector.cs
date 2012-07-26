using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;

namespace FeatureGraph.SOM
{
    /// <summary>
    /// N-dimensional vector of numbers
    /// </summary>
    public sealed class Vector
    {
        List<double> m_Vector;
        private static Random r = new Random();

        /// <summary>
        /// Initiate a Vector using nums. Elements are copied.
        /// </summary>
        /// <param name="nums">Elements in vector</param>
        public Vector(IEnumerable<double> nums) {
            m_Vector = new List<double>(nums);
        }

        /// <summary>
        /// Vector elements are specified from left to right.
        /// </summary>
        /// <param name="nums">Elements in vector</param>
        public Vector(params double[] nums) {
            m_Vector = new List<double>(nums);
        }

        /// <summary>
        /// Create a Vector with specified dimension.
        /// The vector is randomly initialized using numbers between [0, 1.0).
        /// </summary>
        /// <param name="dimension">The dimension of vector</param>
        public Vector(int dimension) {
            Debug.Assert(dimension > 0, "invalid vector dimension");
            m_Vector = new List<double>(dimension);
            for (var i = 0; i != dimension; i++) {
                m_Vector.Add(r.NextDouble());
            }
        }

        /// <summary>
        /// Vector dimension
        /// </summary>
        public int Dimension {
            get { return m_Vector.Count; }
        }

        /// <summary>
        /// Indexer for accessing individual component
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public double this[int index] {
            get {
                // checking this for better error message
                if (index < 0 || index >= m_Vector.Count) {
                    throw new ArgumentOutOfRangeException("index out of range");
                }
                return m_Vector[index];
            }

            set {
                // checking this for better error message
                if (index < 0 || index >= m_Vector.Count) {
                    throw new ArgumentOutOfRangeException("index out of range");
                }
                m_Vector[index] = value;
            }
        }

        /// <summary>
        /// Perform action on each component of vector.
        /// </summary>
        /// <param name="action">The action performed on component.</param>
        public void ForEach(Action<double> action) {
            if (action == null) {
                return;
            }

            foreach (var i in m_Vector) {
                action(i);
            }
        }

        /// <summary>
        /// Add that vector to this. This function is destructive.
        /// </summary>
        /// <param name="that">A vector to add to this</param>
        /// <returns>This vector</returns>
        public Vector Add(Vector that) {
            var d = this.Dimension;
            if (that.Dimension != d) {
                throw new ArgumentException("dimensions not match");
            } else {
                for (var i = 0; i != d; i++) {
                    m_Vector[i] += that.m_Vector[i];
                }
            }
            return this;
        }

        /// <summary>
        /// Subtract that vector from this. This function is destructive.
        /// </summary>
        /// <param name="that">A vector to subtract from this.</param>
        /// <returns>This vector</returns>
        public Vector Subtract(Vector that) {
            var d = this.Dimension;
            if (that.Dimension != d) {
                throw new ArgumentException("dimensions not match");
            } else {
                for (var i = 0; i != d; i++) {
                    m_Vector[i] -= that.m_Vector[i];
                }
            }
            return this;
        }

        /// <summary>
        /// Multiply this vector by scalar m. This function is destructive.
        /// </summary>
        /// <param name="m">The scalar to multiple</param>
        /// <returns>This vector</returns>
        public Vector Multiply(double m) {
            for (var i = 0; i != this.Dimension; i++) {
                m_Vector[i] *= m;
            }
            return this;
        }

        /// <summary>
        /// Divides this vector by the specified scalar d. This function is destructive.
        /// </summary>
        /// <param name="d">The amount by which this vector is divided</param>
        /// <returns>This vector</returns>
        /// <remarks>Divide by zero is not checked.</remarks>
        public Vector Divide(double d) {
            for (var i = 0; i != this.Dimension; i++) {
                m_Vector[i] /= d;
            }
            return this;
        }

        /// <summary>
        /// Adds two vectors and returns the result as a new vector.
        /// </summary>
        /// <param name="left">The first vector to add</param>
        /// <param name="right">The second vector to add</param>
        /// <returns>The sum of two vectors</returns>
        public static Vector Add(Vector left, Vector right) {
            if (left.Dimension != right.Dimension) {
                throw new ArgumentException("dimensions not match");
            }
            return DoArithmetic(left, Numbers(right), (x, y) => x + y);
        }

        /// <summary>
        /// Subtracts the specified vector from another specified vector.
        /// </summary>
        /// <param name="left">The vector from which right is subtracted.</param>
        /// <param name="right">The vector to subtract from left.</param>
        /// <returns>The difference between left and right. </returns>
        public static Vector Subtract(Vector left, Vector right) {
            if (left.Dimension != right.Dimension) {
                throw new ArgumentException("dimensions not match");
            }
            return DoArithmetic(left, Numbers(right), (x, y) => x - y);
        }

        /// <summary>
        /// Multiplies the specified vector by the specified scalar and returns the resulting vector.
        /// </summary>
        /// <param name="vector">The vector to multiply.</param>
        /// <param name="m">The scalar to multiply.</param>
        /// <returns>The result of multiplying vector and m.</returns>
        public static Vector Multiply(Vector vector, double m) {
            return DoArithmetic(vector, SameNumbers(m, vector.Dimension), (x, y) => x * y);
        }

        /// <summary>
        /// Divides the specified vector by the specified scalar and returns the result as a vector.
        /// </summary>
        /// <param name="vector">The vector to divide.</param>
        /// <param name="d">The amount by which vector is divided.</param>
        /// <returns>The result of dividing vector by d.</returns>
        /// <remarks>Divide by zero is not checked.</remarks>
        public static Vector Divide(Vector vector, double d) {
            return DoArithmetic(vector, SameNumbers(d, vector.Dimension), (x, y) => x / y);
        }

        /// <summary>
        /// Apply op to each pair of left and right, collect the results.
        /// </summary>
        /// <param name="left">lhs of op</param>
        /// <param name="right">rhs of op</param>
        /// <param name="op">Operation function</param>
        /// <returns>A new Vector containing the results</returns>
        static Vector DoArithmetic(Vector left, IEnumerable<double> right, Func<double, double, double> op) {
            Vector result = new Vector(left.Dimension);
            for (var i = 0; i != left.Dimension; i++) {
                result[i] = op(left[i], right.ElementAt(i));
            }
            return result;
        }

        /// <summary>
        /// Repeat the same number n times.
        /// </summary>
        /// <param name="data">The number to be repeated</param>
        /// <param name="repeat">Repeat count</param>
        /// <returns>A collection containing the specified number of data.</returns>
        static IEnumerable<double> SameNumbers(double data, int repeat) {
            if (repeat < 0) {
                throw new ArgumentException("repeat count is negative");
            }
            for (var i = 0; i != repeat; i++) {
                yield return data;
            }
        }

        /// <summary>
        /// Return all numbers in a Vector.
        /// </summary>
        /// <param name="vector">The vector to be converted</param>
        /// <returns>A collection containing all numbers in vector.</returns>
        static IEnumerable<double> Numbers(Vector vector) {
            foreach (var i in vector.m_Vector) {
                yield return i;
            }
        }

        public override bool Equals(object obj) {
            Vector that = obj as Vector;
            if (that == null) {
                return false;
            }

            for (var i = 0; i != this.Dimension; i++) {
                if (this.m_Vector[i] != that.m_Vector[i]) {
                    return false;
                }
            }
            return true;
        }

        public override int GetHashCode() {
            int hash = 17;
            ForEach(d => hash = hash * 37 + (int)d);
            return hash;
        }
    }
}