using UnityEngine;
using System.Collections.Generic;

// Manages particle pools for items.
public class ParticlePool : MonoBehaviour
{
    public ParticleSystem prefab;
    public int poolSize = 10;

    private Queue<ParticleSystem> pool;
    private Transform poolParent;

    /// Initializes the pool by instantiating the particle instances as direct children of the ParticlePool GameObject.
    public void InitializePool()
    {
        pool = new Queue<ParticleSystem>();
        
        poolParent = transform;

        for (int i = 0; i < poolSize; i++)
        {
            ParticleSystem particleInstance = Instantiate(prefab, poolParent);
            particleInstance.gameObject.SetActive(false);
            pool.Enqueue(particleInstance);
        }
    }

    public ParticleSystem Get()
    {
        ParticleSystem particle;
        if (pool.Count > 0)
            particle = pool.Dequeue();
        else
            particle = Instantiate(prefab, poolParent);

        particle.gameObject.SetActive(true);
        return particle;
    }

    public void ReturnToPool(ParticleSystem particle)
    {
        particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        particle.gameObject.SetActive(false);
        pool.Enqueue(particle);
    }
}