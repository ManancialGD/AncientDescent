using System.Collections;
using UnityEngine;

public class TransitionUI : MonoBehaviour
{
    [SerializeField] private TransitionPanel[] panels;
    [SerializeField] private float transitionTime = 0.35f;

    private TransitionPanel? active;

    private TransitionPanel Get(TransitionDirection dir)
        => System.Array.Find(panels, p => p.direction == dir);

    public IEnumerator Close(TransitionDirection dir)
    {
        ResetAll();

        active = Get(dir);
        TransitionPanel p = active.Value;

        yield return ScaleRoutine(p.panel, p.openScale, p.closedScale);
    }

    public IEnumerator Open()
    {
        if (active == null)
            yield break;

        TransitionPanel current = active.Value;
        TransitionPanel opposite = Get(current.opposite);

        current.panel.localScale = current.openScale;
        opposite.panel.localScale = opposite.closedScale;

        yield return ScaleRoutine(
            opposite.panel,
            opposite.closedScale,
            opposite.openScale
        );

        active = null;
    }

    private void ResetAll()
    {
        foreach (var p in panels)
            p.panel.localScale = p.openScale;
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
