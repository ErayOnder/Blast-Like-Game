using UnityEngine;
using System.Collections.Generic;

public class ParticlePool : MonoBehaviour
{
    public ParticleSystem prefab;
    public int poolSize = 10;

    private Queue<ParticleSystem> pool;
    private Transform poolParent;

    /// <summary>
    /// Initializes the pool by instantiating the particle instances as direct children of the ParticlePool GameObject.
    /// </summary>
    public void InitializePool()
    {
        pool = new Queue<ParticleSystem>();
        
        // Instead of creating a separate "Pool" container, use this GameObject directly as the parent.
        poolParent = transform;

        for (int i = 0; i < poolSize; i++)
        {
            // Instantiate the particle as a child of poolParent (i.e., the ParticlePool GameObject).
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
            // In case the pool is empty, instantiate a new particle under the poolParent as well.
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