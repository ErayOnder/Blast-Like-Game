using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Manages particle pools for items.
public class ParticleManager : Singleton<ParticleManager>
{
    [System.Serializable]
    public class ParticleMapping
    {
        public ItemType itemType;
        public ParticleSystem particlePrefab;
        public int poolSize = 10;
        [HideInInspector] public ParticlePool pool;
    }

    public List<ParticleMapping> particleMappings;
    private Dictionary<ItemType, ParticlePool> poolDictionary;

    // Sets up particle pools from the provided mappings.
    protected override void Awake()
    {
        base.Awake();
        poolDictionary = new Dictionary<ItemType, ParticlePool>();

        Transform poolsTransform = transform.Find("Pools");
        if (poolsTransform == null)
        {
            GameObject poolsContainer = new("Pools");
            poolsContainer.transform.SetParent(transform, false);
            poolsTransform = poolsContainer.transform;
        }

        foreach (var mapping in particleMappings)
        {
            if (mapping.particlePrefab != null)
            {
                GameObject poolObject = new(mapping.itemType.ToString() + "Pool");
                poolObject.transform.SetParent(poolsTransform, false);
                ParticlePool newPool = poolObject.AddComponent<ParticlePool>();
                newPool.prefab = mapping.particlePrefab;
                newPool.poolSize = mapping.poolSize;
                newPool.InitializePool();

                mapping.pool = newPool;
                poolDictionary.Add(mapping.itemType, newPool);
            }
            else
            {
                Debug.LogWarning("No particle prefab for ItemType: " + mapping.itemType);
            }
        }
    }

    // Plays the particle effect for the given item.
    public void PlayParticle(Item item)
    {
        if (item == null)
            return;

        if (poolDictionary.TryGetValue(item.itemType, out ParticlePool pool))
        {
            ParticleSystem particle = pool.Get();
            Vector3 spawnPosition = new(item.transform.position.x, item.transform.position.y, -10);
            particle.transform.position = spawnPosition;
            particle.Play();
            StartCoroutine(ReturnParticleAfterPlay(particle, pool));
        }
        else
        {
            Debug.LogWarning("No pool for ItemType: " + item.itemType);
        }
    }

    // Returns a particle to its pool once it has finished playing.
    private IEnumerator ReturnParticleAfterPlay(ParticleSystem particle, ParticlePool pool)
    {
        yield return new WaitUntil(() => !particle.isPlaying);
        pool.ReturnToPool(particle);
    }
}