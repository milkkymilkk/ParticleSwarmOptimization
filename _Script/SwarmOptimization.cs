using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class SwarmOptimization : MonoBehaviour
{
    [SerializeField] GameObject particle;
    [SerializeField] GameObject testObject;
    [SerializeField] private int numberOfParticle = 10;
    GameObject[] particles = new GameObject[10];

    float[,] p_vector = new float[10, 3];
    float[] p_fitness = new float[10];
    float[,] x_vector = new float[10, 3];
    float[] x_fitness = new float[10];

    [SerializeField] private float w = 0.4f; //inertia weight factor
    [SerializeField] private float temporaryW;
    [SerializeField] private float c1 = .5f;
    [SerializeField] private float c2 = 1f;
    float[] globalBest = new float[3]; //current optimal value of the dimension"d" of the swarm -> global best
    float globalBestValue = 9999f;

    Vector3 testObjectPosition = new Vector3();

    // Start is called before the first frame update
    void Start()
    {
        temporaryW = w;
        resizeArray();
        for (int i=0; i<numberOfParticle; i++)
        {
            //initial particle
            GameObject _particle = Instantiate(particle);
            _particle.transform.position = new Vector3(UnityEngine.Random.Range(-4f, 4f), UnityEngine.Random.Range(-4f, 4f), UnityEngine.Random.Range(-4f, 4f));
            particles[i] = _particle;
            //assign velocity
            particles[i].GetComponent<Rigidbody>().velocity = new Vector3(UnityEngine.Random.Range(0f, 20f), UnityEngine.Random.Range(0f, 20f), UnityEngine.Random.Range(0f, 20f));
            particles[i].GetComponent<Rigidbody>().mass = UnityEngine.Random.Range(1f, 1.5f);
            particles[i].transform.localScale = particles[i].GetComponent<Rigidbody>().mass*particles[i].transform.localScale;
            //assign blank value
            p_vector[i, 0] = 0.0f;
            p_vector[i, 1] = 0.0f;
            p_vector[i, 2] = 0.0f;
            p_fitness[i] = 9999f;
            x_fitness[i] = 9999f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log("Time.deltaTime "+ Time.deltaTime);
        if (w > 0.5f)
        {
            w = w - Time.deltaTime*0.1f;
        }
        particleUpdate();
        if (testObject.transform.position != testObjectPosition)
        {
            for (int i = 0; i < numberOfParticle; i++)
            {
                globalBestValue = 9999f;
                w = temporaryW;
                p_vector[i, 0] = 0.0f;
                p_vector[i, 1] = 0.0f;
                p_vector[i, 2] = 0.0f;
                p_fitness[i] = 9999f;
                x_fitness[i] = 9999f;
                globalBestValue = 9999f;
            }
        }
        testObjectPosition = testObject.transform.position;
    }

    private void resizeArray()
    {
        Array.Resize(ref particles, numberOfParticle);
        ResizeArray(ref p_vector, numberOfParticle, 3);
        Array.Resize(ref p_fitness, numberOfParticle);
        ResizeArray(ref x_vector, numberOfParticle, 3);
        Array.Resize(ref x_fitness, numberOfParticle);
    }

    private void ResizeArray<T>(ref T[,] original, int newColNum, int newRowNum)
    {
        var newArray = new T[newColNum, newRowNum];
        original = newArray;
    }

    private void particleUpdate()
    {
        for (int i = 0; i < numberOfParticle; i++)
        {
            float x_v = particles[i].GetComponent<Rigidbody>().velocity.x;
            float y_v = particles[i].GetComponent<Rigidbody>().velocity.y;
            float z_v = particles[i].GetComponent<Rigidbody>().velocity.z;
            float x = (w * x_v) + (c1 * UnityEngine.Random.Range(0f, 1f) * (p_vector[i, 0] - particles[i].transform.position.x)) + (c2 * UnityEngine.Random.Range(0f, 1f) * (globalBest[0] - particles[i].transform.position.x));
            float y = (w * y_v) + (c1 * UnityEngine.Random.Range(0f, 1f) * (p_vector[i, 1] - particles[i].transform.position.y)) + (c2 * UnityEngine.Random.Range(0f, 1f) * (globalBest[1] - particles[i].transform.position.y));
            float z = (w * z_v) + (c1 * UnityEngine.Random.Range(0f, 1f) * (p_vector[i, 2] - particles[i].transform.position.z)) + (c2 * UnityEngine.Random.Range(0f, 1f) * (globalBest[2] - particles[i].transform.position.z));

            //Debug.Log("x" + x + "\n" + "y" + y + "\n" + "z" + z);
        
            particles[i].GetComponent<Rigidbody>().velocity = new Vector3(x, y, z);
            particles[i].transform.position = new Vector3(particles[i].transform.position.x + x*Time.deltaTime,
                                                        particles[i].transform.position.y + y*Time.deltaTime, 
                                                        particles[i].transform.position.z + z*Time.deltaTime);
            
            x = (particles[i].transform.position.x - testObject.transform.position.x) * (particles[i].transform.position.x - testObject.transform.position.x) 
                + (particles[i].transform.position.y - testObject.transform.position.y) * (particles[i].transform.position.y - testObject.transform.position.y)
                + (particles[i].transform.position.z - testObject.transform.position.z) * (particles[i].transform.position.z - testObject.transform.position.z);
            x_fitness[i] = x;
            x_vector[i, 0] = particles[i].transform.position.x;
            x_vector[i, 1] = particles[i].transform.position.y;
            x_vector[i, 2] = particles[i].transform.position.z;

            if (p_fitness[i] > x)
            {
                p_fitness[i] = x;
                p_vector[i, 0] = particles[i].transform.position.x;
                p_vector[i, 1] = particles[i].transform.position.y;
                p_vector[i, 2] = particles[i].transform.position.z;
            }

            if (globalBestValue > x_fitness.Min())
            {
                int min = Array.IndexOf(x_fitness, x_fitness.Min());
                globalBest[0] = x_vector[min, 0];
                globalBest[1] = x_vector[min, 1];
                globalBest[2] = x_vector[min, 2];
                globalBestValue = x_fitness.Min();
            }


            
                        
        }
    }
}
