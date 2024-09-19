using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;

public class Brain : MonoBehaviour
{
    public TextMeshProUGUI scoreTxt;
    public int layers =2;
    public int nodes =10;
    public Matrix[] weights;
    public Matrix[] biases;
    private Matrix inputs;
    private float acceleration;
    private float rotation;
    public Gen.ScoreType scoreType;
    public float score;
    public bool live = true;
    public Vector3 InitialPos;
    
    //ForFitness
    private Vector3 lastPosition;
    private float distanceTraveled = 0;
    private float accelerationPR = 0;
    private int accelerationProm = 0;
    private float timeSurvived = 0;
    private float pointsVisited = 0;
    private Queue<Transform> points = new Queue<Transform>();

    public Rigidbody rg;

    void Start()
    {
        rg = GetComponent<Rigidbody>();
        Init();
        
    }

    public void Init()
    {
        InitialPos = transform.position;
        weights = new Matrix[layers]; 
        biases = new Matrix[layers];
        inputs = new Matrix(1, 3);

        /*if (scoreType == Gen.ScoreType.Nul)
        {
            scoreType = (Gen.ScoreType)UnityEngine.Random.Range(1, 3);
        }*/

        for (int i = 0; i < layers; i++)
        {
            if (i == 0)
            {
                weights[i] = new Matrix(3, nodes);
                weights[i].RandomInitialize();
                biases[i] = new Matrix(1, 3);
                biases[i].RandomInitialize();
            }
            else if (i == layers - 1)
            {
                weights[i] = new Matrix(2, nodes);
                weights[i].RandomInitialize();
                biases[i] = new Matrix(1, 2);
                biases[i].RandomInitialize();
            }
            else
            {
                weights[i] = new Matrix(nodes, nodes);
                weights[i].RandomInitialize();
                biases[i] = new Matrix(1, nodes);
                biases[i].RandomInitialize();
            }
        }
    }
    
    void Update()
    {
        if (live)
        {
            float FD = GetComponent<Driver>().ForwardDistance;
            float RD = GetComponent<Driver>().RightDistance;
            float LD = GetComponent<Driver>().LeftDistance;
            
            inputs.SetAt(0,0,FD);
            inputs.SetAt(0,1,RD);
            inputs.SetAt(0,2,LD);
            Resolve();
            
            transform.Translate(Vector3.forward * acceleration);
            rg.velocity = Vector3.Normalize(rg.velocity);
            transform.eulerAngles = transform.eulerAngles + new Vector3(0, (rotation*90)*0.02f, 0);
 
            
            distanceTraveled += Vector3.Distance(transform.position, lastPosition);
            lastPosition = transform.position;
            accelerationPR += acceleration;
            accelerationProm++;
            timeSurvived += Time.deltaTime;
            SetScore();
            
        }
    }

    void Resolve()
    {
        Matrix result;
        result = Activation((inputs * weights[0]) + biases[0]);
        for (int i = 1; i < layers; i++)
        {
            //result = result * (Activation((pesos[i] * entradas) + biases[i]));
            result = (Activation((weights[i] * result.Transpose()) + biases[i]));
        }
        ActivationLast(result);
    }

    Matrix Activation(Matrix m)
    {
        for (int i = 0; i < m.rows; i++)
        {
            for (int j = 0; j < m.columns; j++)
            {
                m.SetAt(i, j, (float)Library.HyperbolicTangtent(m.GetAt(i, j)));
            }
            
        }
        
        return m;
    }

    void ActivationLast(Matrix m)
    {
        rotation = (float)(Library.HyperbolicTangtent(m.GetAt(0,0)));
        acceleration = Library.Sigmoid(m.GetAt(1,0));
    }
    
    void SetScore()//FitnessFunction
    {
        float FD = GetComponent<Driver>().ForwardDistance;
        float RD = GetComponent<Driver>().RightDistance;
        float LD = GetComponent<Driver>().LeftDistance;
        float s = (FD + LD) / 3;
        
         if (scoreType == Gen.ScoreType.Dist)
        {
            s += RD;
            float x = (distanceTraveled) * acceleration / 1000;
            s += ((timeSurvived*2) + (distanceTraveled*9))-(Mathf.Pow(x,2)*Mathf.PI);
        }
        else if (scoreType == Gen.ScoreType.Tim)
        {
            Vector3 v = new Vector3(FD, 0 , RD-LD);
            float x = timeSurvived * Mathf.Pow(v.x,5);
            s += ((timeSurvived*9) + (distanceTraveled*1.5f))+ x *2.5f;
        }
        else
        {   
            scoreType = (Gen.ScoreType)UnityEngine.Random.Range(1,3);
        }
        
        if (transform.position.z > InitialPos.z)
        {
            if (transform.position.x >10 || transform.position.x < -10)
            {
                s+= Mathf.Pow(Vector3.Distance(InitialPos,transform.position),2);
            }
            else
            {
                if (transform.position.z > 80)
                {
                    s += Vector3.Distance(InitialPos, transform.position) *8;
                }
                s += Vector3.Distance(InitialPos,transform.position)*.5f;
            }
        }
        if(Gen.Instance.generations > 50)
        {
            s += pointsVisited * 500;
        }

        score += Mathf.Pow(s,2);
        
        /*
         * 
        if (scoreType == Gen.ScoreType.Dist)
            {
                s += RD;
                float x = (distanceTraveled) * acceleration / 1000;
                s += ((timeSurvived*2) + (distanceTraveled*9))-(Mathf.Pow(x,2)*Mathf.PI);
            }
            else if (scoreType == Gen.ScoreType.Tim)
            {
                Vector3 v = new Vector3(FD, 0 , RD-LD);
                float x = timeSurvived * Mathf.Pow(v.x,5);
                s += ((timeSurvived*9) + (distanceTraveled*1.5f))+ x *2.5f;
            }
            else
            {   
                scoreType = (Gen.ScoreType)UnityEngine.Random.Range(1,3);
            }
        
            if (transform.position.z > InitialPos.z)
            {
                if (transform.position.x >10 || transform.position.x < -10)
                {
                    s+= Mathf.Pow(Vector3.Distance(InitialPos,transform.position),2);
                }
                else
                {
                    s+= Vector3.Distance(InitialPos,transform.position)*.5f;
                }
            }
        
            score += Mathf.Pow(s,2);
         */
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Border")
        {
            live =false;
            if (score < 1.865325e+09)
            {
                score /= 2;
            }
            Gen.Instance.poblationalive--;
        }

        if (other.tag == "Trigg")
        {
            Transform temp = other.transform;

            if (!points.Contains(temp))
            {
                points.Enqueue(temp);
                if (points.Count > 8)
                {
                    points.Dequeue();
                }
                pointsVisited++;
            }
            else
            {
                points.Clear();
                pointsVisited =0;

            }
            Gen.Instance.poblationalive--;
        }
    }
}
