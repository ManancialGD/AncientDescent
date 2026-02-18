using UnityEngine;

public class BaseItem : ScriptableObject
{
    [field: SerializeField]
    public string ItemName { get; private set; }

    [field: SerializeField]
    public Sprite Icon { get; private set; }
}
