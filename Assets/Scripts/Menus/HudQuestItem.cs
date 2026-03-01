using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HudQuestItem : MonoBehaviour
{
    [SerializeField]
    private Image image;
    [SerializeField]
    private TextMeshProUGUI amountTxt;

    public void Initialize(QuestItemDefinition def, float amount)
    {
        if (image == null || amountTxt == null)
            return;
            
        if (def == null)
            return;

        image.sprite = def.Icon;

        if (amount <= 1)
            amountTxt.text = "";
        else
            amountTxt.text = $"{amount}x";
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        image ??= GetComponentInChildren<Image>();
        amountTxt ??= GetComponentInChildren<TextMeshProUGUI>();
    }
#endif
}
