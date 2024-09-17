using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class Gen : MonoBehaviour
{
    public static Gen Instance;
    public int generations;
    public TextMeshProUGUI txtGenerations;
    public GameObject prefab;
    public int poblationalive;
    public enum ScoreType{Nul=0, Dist=1, Tim=2}

    public int initialPoblation=30;
    public float mutationRate=.07f;

    public List<GameObject> drivers;
    public List<GameObject> newDrivers;

    public int bestScore = 5;
    public int lastScore = 2;    
    public int genMutateRate = 20;
    public int mutations = 5;

    

    private void Start()
    {
        Instance = this;
        poblationalive = initialPoblation;
        drivers = new List<GameObject>();
        newDrivers = new List<GameObject>();
        for (int i = 0; i < initialPoblation; i++)
        {
            GameObject nD= Instantiate(prefab);
            drivers.Add(nD);
        }
        
    }

    private void Update()
    {
        //Texto epocas
        if(poblationalive <= 0)
        {
            NextGeneration();
            DeleteOld();
            poblationalive = initialPoblation;
            generations++;
            
        }
    }

    void DeleteOld()
    {
        for (int i = 0; i < drivers.Count; i++)
        {
            Destroy(drivers[i]);
        }
        drivers.Clear();
        drivers = newDrivers;
    }

    public void NextGeneration()
    {
        drivers.Sort((x, y) => y.GetComponent<Brain>().score.CompareTo(x.GetComponent<Brain>().score));
        List<GameObject> nDrivers;
        nDrivers = new List<GameObject>();
               
        for (int i = 0; i < bestScore; i++)
        {
            nDrivers.Add(GetCopy(drivers[initialPoblation-1-i]));    
        }

        for (int i = 0; i < lastScore; i++)
        {
            nDrivers.Add(GetCopy(drivers[i]));
        }
        
        int k = bestScore+lastScore;

        while (k < initialPoblation  && k>0)
        {
            int i = Random.Range(0, k-1);
            int j = Random.Range(0, k-1);
            nDrivers.Add(Cross(nDrivers[i], nDrivers[j]));
            k++;
        }

        for (int i = 0; i < mutationRate; i++)
        {
            int a = Random.Range(0, initialPoblation-1);
            Brain n = nDrivers[a].GetComponent<Brain>();

            for (int j = 0; j < n.biases.Length; j++)
            {
                n.biases[j].Mutate(mutations);
            }

            for (int j = 0; j < n.weights.Length; j++)
            {
                n.weights[j].Mutate(mutations);
            }
            
        }
        
        newDrivers = nDrivers;
        
    }

    GameObject Cross(GameObject g1, GameObject g2)
    {
        GameObject newObject = Instantiate(prefab) as GameObject;
        GameObject r = newObject;
        r.GetComponent<Brain>().Init();
        Brain ia1 = g1.GetComponent<Brain>();
        Brain ia2 = g2.GetComponent<Brain>();

        for(int i = 0; i < ia1.biases.Length; i++)
        {
            r.GetComponent<Brain>().biases[i] = Matrix.SinglePointCross(ia1.biases[i], ia2.biases[i]);
        }

        for (int i = 0; i < ia1.weights.Length; i++)
        {
            r.GetComponent<Brain>().weights[i] = Matrix.SinglePointCross(ia1.weights[i], ia2.weights[i]);
        }
        return r;

    }
    
    GameObject GetCopy(GameObject obj)
    {
        GameObject copy = Instantiate(prefab) as GameObject;
        GameObject wCopy = copy;
        
        wCopy.GetComponent<Brain>().Init();
        Brain ia = obj.GetComponent<Brain>();
        Brain wIa = wCopy.GetComponent<Brain>();
        
        for (int i = 0; i < ia.biases.Length; i++)
        {
            wIa.biases[i] = ia.biases[i];
        }

        for (int i = 0; i < ia.weights.Length; i++)
        {
            wIa.weights[i] = ia.weights[i];
        }

        return wCopy;
    }   
    
    
    
}
