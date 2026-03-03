using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TooltipUI : MonoBehaviour
{
    public static TooltipUI Instance { get; private set; }

    [SerializeField] private RectTransform root;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Vector2 offset = new Vector2(15f, -15f);

    private RectTransform canvasRect;

    private void Awake()
    {
        Instance = this;
        canvasRect = root?.GetComponentInParent<Canvas>()?.GetComponent<RectTransform>();
        Hide();
    }

    private void LateUpdate()
    {
        if (!root.gameObject.activeSelf)
            return;

        if (Mouse.current == null)
            return;

        Vector2 mousePosition = Mouse.current.position.ReadValue();

        UpdatePosition(mousePosition);
    }

    private void UpdatePosition(Vector2 screenPosition)
    {
        Vector2 finalPos = screenPosition + offset;
        root.position = finalPos;
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
