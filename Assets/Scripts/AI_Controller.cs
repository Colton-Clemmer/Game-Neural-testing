using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NeuralNetworks;
using System;
using System.Linq;

public class AI_Controller : MonoBehaviour
{
    /*
        Network Details:
        Input:
            Hunger
        Output:
            Movement (4)
            Eat
            Cry
     */

    public NeuralNetwork Network = new NeuralNetwork(1, 40, 6);
    public DateTime TimeBorn = DateTime.Now;
    public int Hunger = 100;
    public bool Dead;

    private int _numInputNeurons = 1;
    private int _numOutputNeurons = 6;
    private int _numHiddenNeurons = 40;

    private int _hungerDepleteMillis = 10;
    private DateTime _lastHungerDeplete = DateTime.Now;
    private DateTime _lastScoreMarked = DateTime.Now;
    private int _scoreMarkMillis = 200;

    private List<double> _scores = new List<double>();
    
    [SerializeField] private bool _shouldFeed;

    public double Score
    {
        get { return ((double) Hunger) / 100.0; }
    }

    public double AverageScore { get { return _scores.Average(); } }

    // Start is called before the first frame update
    void Start()
    {
        var input = (int) (UnityEngine.Random.value * 100);
        var output = (int) (UnityEngine.Random.value * _numOutputNeurons);
        var outputArray = new double[_numOutputNeurons];
        outputArray[output] = 1;
        for (var i = 0; i < 100; i++)
        {
            Network.Backpropagate(new double[] { input }, outputArray);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Hunger <= 0)
        {
            Debug.Log("Dead :(");
            Dead = true;
        }
        if (DateTime.Now > _lastHungerDeplete.AddMilliseconds(_hungerDepleteMillis))
        {
            Hunger--;
            _lastHungerDeplete = DateTime.Now;
        }
        if (DateTime.Now > _lastScoreMarked.AddMilliseconds(_scoreMarkMillis))
        {
            _scores.Add(Score);
            _lastScoreMarked = DateTime.Now;
        }
        var result = Network.FeedForward(Hunger);
        if (result[5] > .8)
        {
            Cry();
        }
    }

    public void Cry()
    {
        // Debug.Log("Waaaah");
        Feed();
    }

    public void Feed()
    {
        Hunger = 100;
    }
}
