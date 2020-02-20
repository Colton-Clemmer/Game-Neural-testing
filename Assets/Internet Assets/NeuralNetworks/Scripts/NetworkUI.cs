using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NeuralNetworks;

public class NetworkUI : MonoBehaviour {

    public Color NeuronColor = Color.white;
    public Gradient WeightsGradient;

    public Vector2[] Spacing;

    public float Radius = 0.1f;
    public Vector3 Pos = Vector3.one * 0.5f;

    NeuralNetwork Network; 
    public Material LineMat;



    public void SetNetwork(NeuralNetwork nn )
    {

        this.Network = nn;
    }


    Vector3 GetNeuronPosition(int index, int layer, int numNeurons )
    {
        return new Vector3((Pos.x + Spacing[layer].y * layer) * Screen.width, (Pos.y + Spacing[layer].x * index) * Screen.height );
    }


    public void OnPostRender()
    {

        if (Network == null)
            return;



        GL.PushMatrix();
        LineMat.SetPass(0);
        GL.LoadPixelMatrix ();


        //DRAW WEIGHTS
        for (int i = 0; i < Network.Layer1Size; i++)
        {
            Vector3 nL1 = GetNeuronPosition(i, 0, Network.Layer1Size);
            DrawWeights(nL1, i, 1, Network.Layer2Size);
        }

        for (int i = 0; i < Network.Layer2Size; i++)
        {
            Vector3 nL2 = GetNeuronPosition(i, 1, Network.Layer2Size);
            DrawWeights(nL2, i, 2, Network.Layer3Size);

        }

        //DRAW CIRCLES / NEURONS
        for (int i = 0; i < Network.Layer1Size; i++)
        {
            Vector3 nL1 = GetNeuronPosition(i, 0, Network.Layer1Size);
            DrawCircle(nL1, Radius);
        }

        for (int i = 0; i < Network.Layer2Size; i++)
        {
            Vector3 nL2 = GetNeuronPosition(i, 1, Network.Layer2Size);
            DrawCircle(nL2, Radius);

        }
        for (int i = 0; i < Network.Layer3Size; i++)
        {
            Vector3 nL3 = GetNeuronPosition(i, 2, Network.Layer3Size);
            DrawCircle(nL3, Radius);
        }




        GL.PopMatrix();

    }


    public void DrawWeights(Vector3 from, int fromNeuron, int toLayerNum, int toLayerSize)
    {
        GL.Begin(GL.LINES);

        for (int j = 0; j < toLayerSize; j++)
        {

            double weight = Network.Weights1[fromNeuron * toLayerSize + j].Value;
            if (toLayerNum == 2)
                weight = Network.Weights2[fromNeuron * toLayerSize + j].Value;

            double normalT = (weight - Network.MinWeight) / (Network.MaxWeight - Network.MinWeight);

            GL.Color(WeightsGradient.Evaluate((float)normalT));
            GL.Vertex(from);
            GL.Vertex(GetNeuronPosition(j, toLayerNum, toLayerSize));
        }
        GL.End();
    }


    public void DrawCircle(Vector3 pos, float radius, int segments = 20)
    {
        GL.Begin(GL.LINES);
        GL.Color(NeuronColor);
        float inc = (Mathf.PI * 2) / segments;
        for(int i=0; i<=segments; i++)
        {
            float angle = inc * i;
            var vec1 = new Vector3(Mathf.Sin(angle), Mathf.Cos(angle)) * radius + pos;
            var vec2 = new Vector3(Mathf.Sin(angle+inc), Mathf.Cos(angle+inc)) * radius + pos;

            GL.Vertex(vec1);
            GL.Vertex(vec2);

        }
        GL.End();

    }
}
