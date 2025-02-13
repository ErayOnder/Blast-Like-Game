using UnityEngine;
using DG.Tweening;
using System.Collections;

public class RocketAnimation : Singleton<RocketAnimation>
{
    // Prefab for the rocket part (should have a SpriteRenderer)
    public GameObject rocketPartPrefab;
    
    // How far the rocket part should travel and how long the animation lasts.
    public float rocketPartTravelDistance = 10f;
    public float rocketPartDuration = 0.5f;
    
    // Particle prefabs for smoke (continuous trail) and star (at the end).
    public ParticleSystem smokeParticlePrefab;
    public ParticleSystem starParticlePrefab;
    
    // How often smoke is spawned along the moving rocket part.
    public float smokeSpawnInterval = 0.1f;
    
    // Sprites for rocket parts based on direction.
    public Sprite horizontalPartLeftSprite;
    public Sprite horizontalPartRightSprite;
    public Sprite verticalPartUpSprite;
    public Sprite verticalPartDownSprite;
    
    // Called to animate a rocket explosion.
    public void AnimateRocketExplosion(RocketItem rocket)
    {
        if (rocket == null)
            return;
        
        Vector3 origin = rocket.transform.position;
        
        // Depending on the rocket's orientation, create two rocket parts moving in opposite directions.
        if (rocket.RocketType == RocketType.Horizontal)
        {
            AnimateRocketPart(origin, Vector3.left, horizontalPartLeftSprite);
            AnimateRocketPart(origin, Vector3.right, horizontalPartRightSprite);
        }
        else // Vertical
        {
            AnimateRocketPart(origin, Vector3.up, verticalPartUpSprite);
            AnimateRocketPart(origin, Vector3.down, verticalPartDownSprite);
        }
        
        // Spawn a star particle effect at the rocket's origin to emphasize the explosion.
        if (starParticlePrefab != null)
        {
            ParticleSystem starPS = Instantiate(starParticlePrefab, origin, Quaternion.identity);
            starPS.Play();
            Destroy(starPS.gameObject, starPS.main.duration + starPS.main.startLifetime.constantMax);
        }
        
        // Finally, destroy the original rocket GameObject.
        Destroy(rocket.gameObject);
    }
    
    // Helper that instantiates and animates a single rocket part.
    private void AnimateRocketPart(Vector3 origin, Vector3 direction, Sprite partSprite)
    {
        GameObject rocketPart = Instantiate(rocketPartPrefab, origin, Quaternion.identity);
        SpriteRenderer sr = rocketPart.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sprite = partSprite;
        }
        
        Vector3 target = origin + direction * rocketPartTravelDistance;
        
        // Animate the part using DOTween.
        rocketPart.transform.DOMove(target, rocketPartDuration)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                // When the part reaches its destination, spawn a star particle effect.
                if (starParticlePrefab != null)
                {
                    ParticleSystem starPS = Instantiate(starParticlePrefab, target, Quaternion.identity);
                    starPS.Play();
                    Destroy(starPS.gameObject, starPS.main.duration + starPS.main.startLifetime.constantMax);
                }
                Destroy(rocketPart);
            });
        
        // Start spawning smoke particles along the rocket part's path.
        StartCoroutine(SpawnSmokeRoutine(rocketPart, direction, rocketPartDuration));
    }
    
    // Coroutine spawning smoke particles at fixed intervals along the moving rocket part.
    private IEnumerator SpawnSmokeRoutine(GameObject rocketPart, Vector3 direction, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            if (rocketPart == null)
                yield break;
            
            SpawnSmokeAtPosition(rocketPart.transform.position, direction);
            yield return new WaitForSeconds(smokeSpawnInterval);
            elapsed += smokeSpawnInterval;
        }
    }
    
    // Spawns a smoke particle behind the rocket part; the spawn position is adjusted slightly opposite to the direction of movement.
    private void SpawnSmokeAtPosition(Vector3 position, Vector3 direction)
    {
        if (smokeParticlePrefab == null)
            return;
        
        Vector3 spawnPos = position - direction * 0.2f;
        ParticleSystem smokePS = Instantiate(smokeParticlePrefab, spawnPos, Quaternion.identity);
        smokePS.Play();
        Destroy(smokePS.gameObject, smokePS.main.duration + smokePS.main.startLifetime.constantMax);
    }
}
