using UnityEngine;
using DG.Tweening;
using System;

// Class role: Handles explosion animations for rocket items (horizontal and vertical).
public class RocketAnimation : MonoBehaviour
{
    public Item item;
    [SerializeField] private float animationSpeed = 0.5f;
    [SerializeField] private float splitDistance = 0.5f;

    [Header("Sprites")]
    [SerializeField] private Sprite horizontalLeftSprite;
    [SerializeField] private Sprite horizontalRightSprite;
    [SerializeField] private Sprite verticalUpSprite;
    [SerializeField] private Sprite verticalDownSprite;

    private SpriteRenderer originalSprite;
    private bool isAnimating = false;

    private void Awake()
    {
        DOTween.SetTweensCapacity(500, 50);
        originalSprite = GetComponentInChildren<SpriteRenderer>();
    }

    // Animate horizontal explosion.
    public void PlayHorizontalExplosionAnimation(Cell leftTarget, Cell rightTarget, System.Action onComplete)
    {
        if (isAnimating || item == null) return;
        isAnimating = true;

        GameObject leftPart = CreateRocketPart("LeftRocketPart", horizontalLeftSprite);
        GameObject rightPart = CreateRocketPart("RightRocketPart", horizontalRightSprite);

        originalSprite.enabled = false;

        Sequence seq = DOTween.Sequence();
        seq.Join(leftPart.transform.DOMove(transform.position + Vector3.left * splitDistance, animationSpeed * 0.3f));
        seq.Join(rightPart.transform.DOMove(transform.position + Vector3.right * splitDistance, animationSpeed * 0.3f));
        seq.Append(leftPart.transform.DOMove(leftTarget.transform.position, animationSpeed).SetEase(Ease.InCubic));
        seq.Join(rightPart.transform.DOMove(rightTarget.transform.position, animationSpeed).SetEase(Ease.InCubic));
        seq.OnComplete(() =>
        {
            Destroy(leftPart);
            Destroy(rightPart);
            isAnimating = false;
            onComplete?.Invoke();
        });
    }

    // Animate vertical explosion.
    public void PlayVerticalExplosionAnimation(Cell upTarget, Cell downTarget, System.Action onComplete)
    {
        if (isAnimating || item == null) return;
        isAnimating = true;

        GameObject upPart = CreateRocketPart("UpRocketPart", verticalUpSprite);
        GameObject downPart = CreateRocketPart("DownRocketPart", verticalDownSprite);

        originalSprite.enabled = false;

        Sequence seq = DOTween.Sequence();
        seq.Join(upPart.transform.DOMove(transform.position + Vector3.up * splitDistance, animationSpeed * 0.3f));
        seq.Join(downPart.transform.DOMove(transform.position + Vector3.down * splitDistance, animationSpeed * 0.3f));
        seq.Append(upPart.transform.DOMove(upTarget.transform.position, animationSpeed).SetEase(Ease.InCubic));
        seq.Join(downPart.transform.DOMove(downTarget.transform.position, animationSpeed).SetEase(Ease.InCubic));
        seq.OnComplete(() =>
        {
            Destroy(upPart);
            Destroy(downPart);
            isAnimating = false;
            onComplete?.Invoke();
        });
    }

    private GameObject CreateRocketPart(string name, Sprite sprite)
    {
        GameObject part = new(name);
        part.transform.SetParent(item.transform);
        part.transform.localPosition = Vector3.zero;

        SpriteRenderer sr = part.AddComponent<SpriteRenderer>();

        if (item is RocketItem rocketItem)
        {
            item.ApplySpriteRendererProperties(sr, sprite);
            rocketItem.ApplyExplosionSpriteRendererProperties(sr, sprite);
            sr.sortingLayerID = SortingLayer.NameToID("Rocket");
            sr.sortingOrder = 100;
        }
        
        return part;
    }

    private void OnDestroy()
    {
        DOTween.Kill(transform);
    }
}
