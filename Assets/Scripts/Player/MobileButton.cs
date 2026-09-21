using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MobileButton : MonoBehaviour,
    IPointerDownHandler, IPointerUpHandler
{
    public enum ButtonType { Left, Right, Jump, Hide }

    [SerializeField] private ButtonType buttonType;
    [SerializeField] private PhiController phi;

    [Header("Sprite Swap (optional)")]
    [SerializeField] private Image iconImage;        // the Image showing the icon
    [SerializeField] private Sprite normalSprite;    // default icon
    [SerializeField] private Sprite activeSprite;    // icon to show when active

    private void Start()
    {
        if (iconImage == null)
            iconImage = GetComponent<Image>();

        // Set the initial sprite
        if (iconImage != null && normalSprite != null)
            iconImage.sprite = normalSprite;
    }

    private void Update()
    {
        // Only the Hide button swaps based on Phi's state
        if (buttonType != ButtonType.Hide || phi == null || iconImage == null)
            return;

        Sprite desired = phi.IsHiding ? activeSprite : normalSprite;
        if (desired != null && iconImage.sprite != desired)
            iconImage.sprite = desired;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (phi == null) return;

        switch (buttonType)
        {
            case ButtonType.Left: phi.ExternalHorizontal = -1f; break;
            case ButtonType.Right: phi.ExternalHorizontal = 1f; break;
            case ButtonType.Jump: phi.ExternalJumpPressed = true; break;
            case ButtonType.Hide: phi.ExternalHidePressed = true; break;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (phi == null) return;
        if (buttonType == ButtonType.Left || buttonType == ButtonType.Right)
            phi.ExternalHorizontal = 0f;
    }
}