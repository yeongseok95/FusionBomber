using UnityEngine;
using Fusion;

public enum ItemType { Power, Count, Speed }

public class ItemPickup : NetworkBehaviour
{
    [SerializeField] private ItemType type;

    public override void FixedUpdateNetwork()
    {
        if (Object.HasStateAuthority)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, 0.4f);
            foreach (var hit in hits)
            {
                if (hit.CompareTag("Player"))
                {
                    var pc = hit.GetComponent<PlayerController>();
                    if (pc != null)
                    {
                        pc.ApplyItem(type);
                        Runner.Despawn(Object);
                        break;
                    }
                }
            }
        }
    }
}
