using UnityEngine;
using Fusion;

public class DestructibleBlock : NetworkBehaviour
{
    [SerializeField] private NetworkPrefabRef[] itemPrefabs;
    [SerializeField] [Range(0, 1)] private float dropChance = 0.3f;

    public void RequestDestruction()
    {
        if (Object.HasStateAuthority)
        {
            if (UnityEngine.Random.value < dropChance && itemPrefabs.Length > 0)
            {
                var item = itemPrefabs[UnityEngine.Random.Range(0, itemPrefabs.Length)];
                Runner.Spawn(item, transform.position, Quaternion.identity);
            }
            Runner.Despawn(Object);
        }
    }
}
