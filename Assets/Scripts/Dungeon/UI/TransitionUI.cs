using System.Collections;
using UnityEngine;

public class TransitionUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private RectTransform up;
    [SerializeField] private RectTransform down;
    [SerializeField] private RectTransform left;
    [SerializeField] private RectTransform right;

    [SerializeField] private float transitionTime = 0.35f;

    private RectTransform activePanel;

    private void ResetPanels()
    {
        up.localScale = new Vector3(1, 0, 1);
        down.localScale = new Vector3(1, 0, 1);
        left.localScale = new Vector3(0, 1, 1);
        right.localScale = new Vector3(0, 1, 1);
    }

    private RectTransform GetPanel(TransitionDirection dir)
    {
        return dir switch
        {
            TransitionDirection.Up => up,
            TransitionDirection.Down => down,
            TransitionDirection.Left => left,
            TransitionDirection.Right => right,
            _ => null
        };
    }

    public IEnumerator Close(TransitionDirection dir)
    {
        ResetPanels();
        activePanel = GetPanel(dir);

        Vector3 from = activePanel.localScale;
        Vector3 to = new(1, 1, 1);

        yield return ScaleRoutine(activePanel, from, to);
    }

    public IEnumerator Open()
    {
        if (activePanel == null)
            yield break;

        Vector3 to = activePanel == left || activePanel == right
            ? new Vector3(0, 1, 1)
            : new Vector3(1, 0, 1);

        yield return ScaleRoutine(activePanel, activePanel.localScale, to);
        activePanel = null;
    }

    private IEnumerator ScaleRoutine(RectTransform panel, Vector3 from, Vector3 to)
    {
        float t = 0f;
        while (t < transitionTime)
        {
            t += Time.deltaTime;
            panel.localScale = Vector3.Lerp(from, to, t / transitionTime);
            yield return null;
        }
        panel.localScale = to;
    }
}
