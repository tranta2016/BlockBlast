using UnityEngine;

public class Cell : MonoBehaviour
{
    // Kéo sprite vào theo cùng thứ tự màu:
    // 0 đỏ, 1 cam, 2 vàng, 3 xanh lá, 4 lam nhạt, 5 xanh dương, 6 xanh đậm, 7 tím, 8 hồng, 9 xám
    [SerializeField] private Sprite[] normalSprites;
    [SerializeField] private Sprite[] highlightSprites;

    private SpriteRenderer spriteRenderer;
    private int colorIndex;

    private void Awake()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    // Chỉ ghi nhớ màu. Phải gọi trước Normal(), Highlight() hoặc Hover().
    public void SetColor(int index)
    {
        colorIndex = index;
    }

    public void Normal()
    {
        gameObject.SetActive(true);
        spriteRenderer.color = Color.white;
        spriteRenderer.sprite = Pick(normalSprites);
    }

    public void Highlight()
    {
        gameObject.SetActive(true);
        spriteRenderer.color = Color.white;
        spriteRenderer.sprite = Pick(highlightSprites);
    }

    public void Hover()
    {
        gameObject.SetActive(true);
        spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        spriteRenderer.sprite = Pick(normalSprites);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private Sprite Pick(Sprite[] sprites)
    {
        if (sprites == null || sprites.Length == 0) return null;
        return sprites[Mathf.Clamp(colorIndex, 0, sprites.Length - 1)];
    }
}
