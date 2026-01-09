using Unity.Netcode;

public class PlayerKeys : NetworkBehaviour
{
    private NetworkVariable<int> keys = new(0);

    public bool HasKey => keys.Value > 0;

    [ServerRpc]
    public void AddKeyServerRpc()
    {
        keys.Value++;
    }

    public bool ConsumeKey()
    {
        if (!IsServer || keys.Value <= 0) return false;
        keys.Value--;
        return true;
    }
}
