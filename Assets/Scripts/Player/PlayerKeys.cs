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

    /// <summary>
    /// Attempts to consume a key.
    /// </summary>
    /// <returns>Returns true if successful, false if the player has no keys.</returns>
    public bool ConsumeKey()
    {
        if (!NetworkManager.Singleton.IsServer || keys.Value <= 0) return false;
        keys.Value--;
        return true;
    }
}
