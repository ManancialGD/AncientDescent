using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using AncientDescent.UI;
using AncientDescent.Items;

namespace AncientDescent.Menus.HUD
{    
    public class HudItem : MonoBehaviour,
        IPointerEnterHandler,
        IPointerExitHandler
    {
        [SerializeField] protected Image iconImage;
        [SerializeField] protected TextMeshProUGUI countText;
    
        protected BaseItem itemData;
    
        public virtual void Initialize(BaseItem item, int count)
        {
            itemData = item;
    
            if (iconImage != null)
                iconImage.sprite = item.Icon;
    
            if (countText != null)
            {
                countText.text = count <= 1 ? "" : $"{count}x";
            }
        }
    
        public virtual void Clear()
        {
            itemData = null;
        }
    
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (itemData == null)
                return;
    
            TooltipUI.Instance.Show(
                itemData.ItemName,
                itemData.Description
            );
        }
    
        public void OnPointerExit(PointerEventData eventData)
        {
            TooltipUI.Instance.Hide();
        }
        
        private void OnDestroy()
        {
            TooltipUI.Instance.Hide();
        }
    
    #if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            iconImage ??= GetComponentInChildren<Image>();
            countText ??= GetComponentInChildren<TextMeshProUGUI>();
        }
    #endif
    }
}
