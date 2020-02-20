using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace NeuralNetworks
{
    [Serializable]
    public class NeuralNetwork
    {
        public int Layer1Size;
        public int Layer2Size;
        public int Layer3Size;
        public int BatchSize;
        public int BatchCounter;
        public double Step;
        public double Momentum;

        public double MaxWeight;
        public double MinWeight;

        public double Bias1, Bias2, Bias1Error, Bias2Error, BiasMomentum1, BiasMomentum2;
        public Weight[] Weights1, Weights2;
        public Neuron[] Neurons1, Neurons2, Neurons3;

        public NeuralNetwork(int Layer1Size, int Layer2Size, int Layer3Size)
        {
            this.Layer1Size = Layer1Size;
            this.Layer2Size = Layer2Size;
            this.Layer3Size = Layer3Size;
            this.SetMomentum(0.8f);
            this.SetStepSize(1f);
            this.SetBatchSize(1);

            this.Neurons1 = new Neuron[Layer1Size];
            this.Neurons2 = new Neuron[Layer2Size];
            this.Neurons3 = new Neuron[Layer3Size];
            this.Weights1 = new Weight[Layer1Size * Layer2Size];
            this.Weights2 = new Weight[Layer2Size * Layer3Size];

            ResetNetwork();

        }


        public void SetMomentum(double momentum)
        {
            if (momentum > 1f)
                throw new Exception("Momentum will grow with each training? This should not be allowed!");

            this.Momentum = momentum;
        }


        public void SetStepSize(double step)
        {
            this.Step = step;
        }


        public void SetBatchSize(int size)
        {
            this.BatchSize = size;
            this.BatchCounter = 0;
        }


        public float[] FeedForward(params double[] input)
        {
            var output = new float[Layer3Size];

            //Reset
            for (int i = 0; i < Layer3Size; i++)
                Neurons3[i].Value = 0f;
            for (int i = 0; i < Layer2Size; i++)
                Neurons2[i].Value = 0f;


            //For each neuron in layer1
            for(int i=0; i<Layer1Size; i++)
            {
                //for each neuron in layer2
                for(int j=0; j<Layer2Size; j++)
                {
                    int weightID = CalculateIndex(i, j, Layer2Size);
                    Neurons2[j].Value += Weights1[weightID].Value * input[i];
                }
            }

            //for each neuron in layer2
            for (int j = 0; j < Layer2Size; j++)
            {
                Neurons2[j].Value += Bias1;
            }

            //For each neuron in layer2
            for (int i = 0; i < Layer2Size; i++)
            {
                //for each neuron in layer3
                for (int j = 0; j < Layer3Size; j++)
                {
                    int weightID = CalculateIndex(i, j, Layer3Size);
                    Neurons3[j].Value += Weights2[weightID].Value * Functions.Sigmoid(Neurons2[i].Value);
                }
            }

            //for each neuron in layer3
            for (int j = 0; j < Layer3Size; j++)
            {
                Neurons3[j].Value += Bias2;
            }


            //Put values in output
            //for each neuron in layer3
            for (int j = 0; j < Layer3Size; j++)
            {
                output[j] = (float)Functions.Sigmoid(Neurons3[j].Value);
            }

            return output;
        }


        public void Backpropagate(double[] input, double[] target)
        {

            var output = FeedForward(input);

            //for each neuron in layer3
            for (int j = 0; j < Layer3Size; j++)
            {
                Neurons3[j].Gradient = -(target[j] - output[j]) * Functions.SigmoidDerivative(Neurons3[j].Value);
            }

            //For each neuron in layer2
            for (int i = 0; i < Layer2Size; i++)
            {
                Neurons2[i].Gradient = 0f;

                //for each neuron in layer3
                for (int j = 0; j < Layer3Size; j++)
                {
                    int weightID = CalculateIndex(i, j, Layer3Size);
                    Neurons2[i].Gradient += Neurons3[j].Gradient * Weights2[weightID].Value * Functions.SigmoidDerivative(Neurons2[i].Value);
                    Weights2[weightID].Error += Neurons3[j].Gradient * Functions.Sigmoid(Neurons2[i].Value);
                }
            }

            //for each neuron in layer3
            for (int j = 0; j < Layer3Size; j++)
            {
                Bias2Error += Neurons3[j].Gradient;
            }

            //For each neuron in layer1
            for (int i = 0; i < Layer1Size; i++)
            {
                //for each neuron in layer2
                for (int j = 0; j < Layer2Size; j++)
                {
                    int weightID = CalculateIndex(i, j, Layer2Size);
                    Neurons1[i].Gradient += Neurons2[j].Gradient * Weights1[weightID].Value * Functions.SigmoidDerivative(Neurons1[i].Value);
                    Weights1[weightID].Error += Neurons2[j].Gradient * input[i];
                }
            }

            //for each neuron in layer2
            for (int j = 0; j < Layer2Size; j++)
            {
                Bias1Error += Neurons2[j].Gradient;
            }

            BatchCounter++;

            if(BatchCounter >= BatchSize)
            {

                for (int i = 0; i < Weights1.Length; i++)
                {
                    var before = Weights1[i].Value;
                    Weights1[i].Value -= Weights1[i].Error * 1f / (float)BatchSize * Step + Weights1[i].Momentum;
                    Weights1[i].Error = 0f;
                    Weights1[i].Momentum = -(before - Weights1[i].Value) * Momentum;
                }

                for (int i = 0; i < Weights2.Length; i++)
                {

                    var before = Weights2[i].Value;
                    Weights2[i].Value -= Weights2[i].Error * 1f/(float)BatchSize *Step + Weights2[i].Momentum;
                    Weights2[i].Error = 0f;
                    Weights2[i].Momentum = -(before - Weights2[i].Value) * Momentum;
                }

                var before1 = Bias1;
                var before2 = Bias2;

                Bias1 -= Bias1Error * 1f / (float)BatchSize * Step + BiasMomentum1;
                Bias2 -= Bias2Error * 1f / (float)BatchSize * Step + BiasMomentum2;


                BiasMomentum1 = (Bias1 - before1) * Momentum;
                BiasMomentum2 = (Bias2 - before2) * Momentum;

                Bias1Error = 0f;
                Bias2Error = 0f;

                BatchCounter = 0;

                MaxWeight = Math.Max(Weights1.Max(t => t.Value), Weights2.Max(t=>t.Value));
                MinWeight = Math.Min(Weights1.Min(t => t.Value), Weights2.Min(t => t.Value));

            }

        }


        public void ResetNetwork()
        {
            for (int i = 0; i < Weights1.Length; i++)
                Weights1[i].Value = Functions.rand.NextGaussian() * 0.1f;

            for (int i = 0; i < Weights2.Length; i++)
                Weights2[i].Value = Functions.rand.NextGaussian() * 0.1f;

            MaxWeight = Math.Max(Weights1.Max(t => t.Value), Weights2.Max(t => t.Value));
            MinWeight = Math.Min(Weights1.Min(t => t.Value), Weights2.Min(t => t.Value));

            Bias1 = Functions.rand.NextGaussian() * 0.1f;
            Bias2 = Functions.rand.NextGaussian() * 0.1f;
        }



        public int CalculateIndex(int fromID, int toID, int toLayerSize)
        {
            return fromID * toLayerSize + toID;
        }



        public void Save(string path)
        {
            try
            {
                var fs = File.Open(path, FileMode.OpenOrCreate);
                var bf = new BinaryFormatter();
                bf.Serialize(fs, this);
                fs.Close();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }



        public static NeuralNetwork Load(string path)
        {
            NeuralNetwork nn = null;
            try
            {
                var fs = File.Open(path, FileMode.Open);
                var bf = new BinaryFormatter();
                nn = (NeuralNetwork)bf.Deserialize(fs);
                fs.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return nn;
        }
    }
}
