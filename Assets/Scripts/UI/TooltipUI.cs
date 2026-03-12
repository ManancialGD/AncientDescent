using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AncientDescent.UI
{
    public class TooltipUI : MonoBehaviour
    {
        public static TooltipUI Instance { get; private set; }

        [SerializeField] private RectTransform root;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private Vector2 offset = new(15f, -15f);

        private RectTransform canvasRect;
        private Canvas canvas;

        private void Awake()
        {
            Instance = this;
            canvas = root?.GetComponentInParent<Canvas>();
            canvasRect = canvas?.GetComponent<RectTransform>();

            Hide();
        }

        private void LateUpdate()
        {
            if (!root.gameObject.activeSelf)
                return;

            if (Mouse.current == null)
                return;

            Vector2 mousePosition = Mouse.current.position.ReadValue();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                mousePosition,
                canvas.worldCamera,
                out Vector2 canvasPosition
            );

            UpdatePosition(canvasPosition);
        }

        private void UpdatePosition(Vector2 screenPosition)
        {
            Vector2 canvasSize = canvasRect.sizeDelta / 2;
            Vector2 tooltipSize = root.sizeDelta;

            Vector2 finalPos = screenPosition + offset;

            float maxNegativeY = -canvasSize.y + tooltipSize.y + offset.y;
            float maxPositiveY = canvasSize.y - offset.y;
            float maxNegativeX = -canvasSize.x + offset.x;
            float maxPositiveX = canvasSize.x - tooltipSize.x - offset.x;

            finalPos.y = Mathf.Clamp(finalPos.y, maxNegativeY, maxPositiveY);
            finalPos.x = Mathf.Clamp(finalPos.x, maxNegativeX, maxPositiveX);

            root.anchoredPosition = finalPos;
        }

        public void Show(string itemName, string description)
        {
            nameText.text = itemName;
            descriptionText.text = description;
            root.gameObject.SetActive(true);
        }

        public void Hide()
        {
            root.gameObject.SetActive(false);
        }
    }

}
