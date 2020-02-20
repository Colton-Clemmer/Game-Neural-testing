using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeuralNetworks
{
    public static class Functions
    {
        public static Random rand = new Random(0);


        public static double Sigmoid(double z)
        {
            return 1f / (1 + Math.Exp(-z));
        }

        public static double SigmoidDerivative(double z)
        {
            double ez = Math.Exp(z);
            return ez / Math.Pow(1 + ez, 2);
        }


        public static double GetRandom(double min, double max)
        {
            double scale = max - min;
            return rand.NextDouble() * scale + min;
        }


        public static float GetHighestValue(this float[] list, out int index)
        {
            float min = float.MinValue+1;
            index = -1;
            for (int k = 0; k < list.Length; k++)
            {
                if (min < list[k])
                {
                    min = list[k];
                    index = k;
                }
            }
            return min;
        }


        public static double NextGaussian(this Random r, double mu = 0, double sigma = 1)
        {
            var u1 = r.NextDouble();
            var u2 = r.NextDouble();

            var rand_std_normal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);

            var rand_normal = mu + sigma * rand_std_normal;

            return rand_normal;
        }
    }
}
