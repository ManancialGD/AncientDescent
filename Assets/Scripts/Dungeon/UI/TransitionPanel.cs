using UnityEngine;

[System.Serializable]
public struct TransitionPanel
{
    public TransitionDirection direction;
    public RectTransform panel;
    public Vector3 closedScale;
    public Vector3 openScale;
    public TransitionDirection opposite;
}
