using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NeuralNetworks;
using UnityEngine.UI;

public class XOR : MonoBehaviour {


    [System.Serializable]
    public class XORData {
        public Text A;
        public Text B;
        public Text Out;
    }

    public int StepsPerUpdate = 1;

    public XORData[] Data;
    public XORData Test;

    public Text ErrorUI;
    public Text IterationUI;

    public NetworkUI NetworkVisual;
    public bool Train { get; set; }


    NeuralNetwork network;
    // Use this for initialization


    void Start()
    {
        network = new NeuralNetwork(2, 3, 1);
        network.SetMomentum(0f);
        NetworkVisual.SetNetwork(network);
   
    }

    int iteration = 0;
    void Update()
    {
        if (Train)
        {
            for (int i = 0; i < StepsPerUpdate; i++)
            {
                float error = 0f;

                foreach (var dat in Data)
                {
                    error += Mathf.Abs(float.Parse(dat.Out.text) - network.FeedForward(new double[] { float.Parse(dat.A.text), float.Parse(dat.B.text) })[0]);
                    network.Backpropagate(new double[] { float.Parse(dat.A.text), float.Parse(dat.B.text) }, new double[] { float.Parse(dat.Out.text) });
                }
                iteration++;

                ErrorUI.text = error.ToString("F4");
                IterationUI.text = iteration.ToString();
            }
        }
    }


    public void TestCustomData()
    {
        var outp = network.FeedForward(new double[] { float.Parse(Test.A.text), float.Parse(Test.B.text) });
        Test.Out.text = outp[0].ToString("F4");
    }


    public void ToggleTrain()
    {
        Train = !Train;
    }


    public void ResetNetwork()
    {
        iteration = 0;
        network.ResetNetwork();
    }


}
