using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HudItem : MonoBehaviour
{
    [SerializeField] protected Image iconImage;
    [SerializeField] protected TextMeshProUGUI countText;

    public virtual void Initialize(Sprite icon, int count)
    {
        if (iconImage != null)
            iconImage.sprite = icon;

        if (countText != null)
        {
            if (count <= 1)
                countText.text = "";
            else
                countText.text = $"{count}x";
        }
    }

    // Optional: override for additional setup (e.g., tooltips)
    public virtual void Clear()
    {
        // Optionally reset visuals
    }

#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        iconImage ??= GetComponentInChildren<Image>();
        countText ??= GetComponentInChildren<TextMeshProUGUI>();
    }
#endif
}
