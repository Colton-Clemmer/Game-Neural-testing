using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeuralNetworks
{
    [Serializable]

    public struct Weight
    {

        public double Value;
        public double Error;
        public double Momentum;
    }
}
