using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParticleManager : Singleton<ParticleManager>
{
    [System.Serializable]
    public class ParticleMapping
    {
        public ItemType itemType;
        public ParticleSystem particlePrefab;
        public int poolSize = 10;
        [HideInInspector] public ParticlePool pool; // This will be created automatically.
    }

    // Instead of manually referencing many ParticlePool objects, we simply fill in this list.
    public List<ParticleMapping> particleMappings;

    // Dictionary to quickly find a pool by item type.
    private Dictionary<ItemType, ParticlePool> poolDictionary;

    protected override void Awake()
    {
        base.Awake();
        poolDictionary = new Dictionary<ItemType, ParticlePool>();

        // Find or create a "Pools" GameObject as a child of the Manager (this GameObject).
        Transform poolsTransform = transform.Find("Pools");
        if (poolsTransform == null)
        {
            GameObject poolsContainer = new("Pools");
            poolsContainer.transform.SetParent(transform, false);
            poolsTransform = poolsContainer.transform;
        }

        // For each mapping, create a new GameObject that holds a ParticlePool component.
        foreach (var mapping in particleMappings)
        {
            if (mapping.particlePrefab != null)
            {
                GameObject poolObject = new(mapping.itemType.ToString() + "Pool");
                // Instead of setting this pool as a direct child of the Manager,
                // set it as a child of the "Pools" container.
                poolObject.transform.SetParent(poolsTransform, false);
                ParticlePool newPool = poolObject.AddComponent<ParticlePool>();
                newPool.prefab = mapping.particlePrefab;
                newPool.poolSize = mapping.poolSize;
                newPool.InitializePool(); // explicitly initialize the pool

                mapping.pool = newPool;
                poolDictionary.Add(mapping.itemType, newPool);
            }
            else
            {
                Debug.LogWarning("Particle prefab not assigned for ItemType: " + mapping.itemType);
            }
        }
    }

    public void PlayParticle(Item item)
    {
        if (item == null)
            return;

        if (poolDictionary.TryGetValue(item.itemType, out ParticlePool pool))
        {
            ParticleSystem particle = pool.Get();
            // Adjust position if necessary (here we set a fixed z to ensure proper layering).
            Vector3 spawnPosition = new(item.transform.position.x, item.transform.position.y, -10);
            particle.transform.position = spawnPosition;
            particle.Play();
            StartCoroutine(ReturnParticleAfterPlay(particle, pool));
        }
        else
        {
            Debug.LogWarning("No particle pool found for ItemType: " + item.itemType);
        }
    }

    private IEnumerator ReturnParticleAfterPlay(ParticleSystem particle, ParticlePool pool)
    {
        // Wait until the particle system finishes playing.
        yield return new WaitUntil(() => !particle.isPlaying);
        pool.ReturnToPool(particle);
    }
}