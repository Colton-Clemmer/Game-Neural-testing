using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using NeuralNetworks;

public class AI_Manager_Controller : MonoBehaviour
{
    private string saveFile = "./network.dat";
    private NeuralNetwork _network = new NeuralNetwork(1, 40, 6);
    [SerializeField] private GameObject AiPrefab;
    public List<AI_Controller> AIs = new List<AI_Controller>();
    public int _numMaxAIs = 4;
    private List<AI_Controller> _aliveAI { get { return AIs.Where(ai => !ai.Dead).ToList(); } }
    private List<AI_Controller> _deadAI { get { return AIs.Where(ai => ai.Dead).ToList(); } }
    [SerializeField] private double _highestScore = 0;

    private void _addAI()
    {
        var ai = Instantiate(AiPrefab);
        var controller = ai.GetComponent<AI_Controller>();
        controller.Network = NeuralNetwork.Load(saveFile);
        ai.transform.position = new Vector3(UnityEngine.Random.value * 5, UnityEngine.Random.value * 5, 0);
        AIs.Add(controller);
    }

    void Start()
    {
        try {
            _network = NeuralNetwork.Load(saveFile);
        } catch (Exception) { }
        _network.Save(saveFile);
        _addAI();
    }

    void Update()
    {
        if (_aliveAI.Count() < _numMaxAIs)
        {
            _addAI();
        }
        var deadAi = _deadAI.FirstOrDefault();
        if (deadAi != null)
        {
            if (deadAi.AverageScore > _highestScore)
            {
                Debug.Log("New High Score: " + deadAi.AverageScore);
                _highestScore = deadAi.AverageScore;
                deadAi.Network.Save(saveFile);
            }
            AIs.Remove(deadAi);
            Destroy(deadAi.gameObject);
        }
    }
}
