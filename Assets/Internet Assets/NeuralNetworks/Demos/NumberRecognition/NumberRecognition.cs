using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using NeuralNetworks;
using UnityEngine.UI;

public class NumberRecognition : MonoBehaviour {

    public NetworkUI NetworkUIReference;

    public Text IterationUI;
    public Text CounterUI;
    public Text CorrectUI;
    public Text PercentUI;
    public Text ResultUI;

    public int Sample = 100;
    public int StepPerUpdate = 10;
    public Draw Drawer;

    MinstRead minstRead;
    NeuralNetwork network;


    double[][] target;
    int correct, counter, iterations;
    bool Train;

    void Start()
    {
        Application.runInBackground = true;

        network = new NeuralNetwork(784, 30, 10);
        minstRead = new MinstRead("C:/Users/Drakce/Desktop/train-labels.idx1-ubyte", "C:/Users/Drakce/Desktop/train-images.idx3-ubyte");

        var spacing = new Vector2[] { new Vector2(0.001f, 0.1f), new Vector2(0.027f, 0.1f), new Vector2(0.087f, 0.1f) };
        NetworkUIReference.SetNetwork(network);


        target = new double[10][];
        for (int i = 0; i < 10; i++)
        {
            target[i] = new double[10];
            for (int j = 0; j < 10; j++)
                target[i][j] = i == j ? 1 : 0;
        }

        network.SetBatchSize(20);
        network.SetMomentum(0.5);
        network.SetStepSize(3);
    }



    void Update()
    {

        if (Train)
        {
            for (int i = 0; i < StepPerUpdate; i++)
            {
                int pictureID = Random.Range(0, minstRead.Images.Count);
                DigitImage picture = minstRead.Images[pictureID];

                var outp = network.FeedForward(picture.values); //Test whther prediction is true before training
                network.Backpropagate(picture.values, target[picture.label]); //Train

                //Check
                int num = -1;
                outp.GetHighestValue(out num);
                bool isCorrect = picture.label == num;

                if (isCorrect)
                    correct++;

                counter++;
                iterations++;

                if (counter > Sample)
                {
                    PercentUI.text = ((float)correct / (float)Sample * 100f).ToString("F3");
                    counter = 0;
                    correct = 0;
                }
            }
        }


        IterationUI.text = iterations.ToString();
        CounterUI.text = counter.ToString();
        CorrectUI.text = correct.ToString();
    }


    public void ToggleTrain()
    {
        Train = !Train;
    }


    public void Reset()
    {
        network.ResetNetwork();
        counter = 0;
        correct = 0;
        iterations = 0;

    }


    public void Save()
    {
        network.Save("C:/Users/Drakce/Desktop/NumberRecognition.txt");
    }


    public void Load()
    {
        network = NeuralNetwork.Load("C:/Users/Drakce/Desktop/NumberRecognition.txt");
        NetworkUIReference.SetNetwork(network);
    }


    public void ConvertToNumber()
    {
        var img = new DigitImage(Drawer.Tex.GetPixels32(), 0, Drawer.ImageSize, true);
        var outp = network.FeedForward(img.values);

        int index = 0;
        float val = outp.GetHighestValue(out index);
        ResultUI.text = "Number is: " + index + " percent: " + (val * 100).ToString("F2");
    }


    public void ClearImage()
    {
        Drawer.Reset();
    }
}
