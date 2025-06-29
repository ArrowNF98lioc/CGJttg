using UnityEngine;

public class GalleryStateController : MonoBehaviour
{
    [Header("状态1时显示的图片")]
    public Sprite sprite1;
    [Header("状态2或3时显示的图片")]
    public Sprite sprite2;

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("[GalleryStateController] 未找到SpriteRenderer组件");
        }
    }

    void Update()
    {
        UpdateGallerySprite();
    }

    private void UpdateGallerySprite()
    {
        if (Player.Instance == null || spriteRenderer == null)
            return;
        int stage = Player.Instance.GetHealthStageNumber();
        if (stage == 1)
        {
            if (spriteRenderer.sprite != sprite1)
                spriteRenderer.sprite = sprite1;
        }
        else if (stage == 2 || stage == 3)
        {
            if (spriteRenderer.sprite != sprite2)
                spriteRenderer.sprite = sprite2;
        }
    }
} 