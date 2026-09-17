using UnityEngine;
using UnityEngine.UI;

public class StageCarousel : MonoBehaviour
{
    public ScrollRect scrollRect;
    public RectTransform content;
    public int stageCount = 3;

    private int currentIndex = 0;

    public void Next()
    {
        if (currentIndex < stageCount - 1)
        {
            currentIndex++;
            SnapToCurrent();
        }
    }

    public void Prev()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            SnapToCurrent();
        }
    }

    void SnapToCurrent()
    {
        float cardWidth = content.GetChild(0).GetComponent<RectTransform>().rect.width;
        float spacing = content.GetComponent<HorizontalLayoutGroup>().spacing;
        float step = cardWidth + spacing;

        float contentWidth = content.rect.width;
        float viewportWidth = scrollRect.viewport.rect.width;

        // How far the content can scroll in total
        float scrollableWidth = contentWidth - viewportWidth;

        if (scrollableWidth <= 0f)
        {
            // Everything fits — nothing to scroll
            return;
        }

        // Target scroll distance in pixels
        float targetPixels = currentIndex * step;

        // ScrollRect uses 0..1 normalized horizontal position
        float normalized = targetPixels / scrollableWidth;
        normalized = Mathf.Clamp01(normalized);

        scrollRect.horizontalNormalizedPosition = normalized;
    }
}