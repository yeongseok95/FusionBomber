using UnityEngine;
using Fusion;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private NetworkPrefabRef bombPrefab;
    
    [Networked] public int BombPower { get; set; } = 1;
    [Networked] public int MaxBombs { get; set; } = 1;
    [Networked] public float CurrentSpeed { get; set; } = 5f;
    [Networked] private int _activeBombs { get; set; }
    [Networked] public NetworkBool IsDead { get; set; }

    [SerializeField] private GameObject winUI;
    [SerializeField] private GameObject loseUI;

    private Vector3 _targetPosition;
    private bool _isMoving;

    public override void Spawned()
    {
        _targetPosition = transform.position;
        if (Object.HasStateAuthority)
        {
            BombPower = 1;
            MaxBombs = 1;
            CurrentSpeed = 5f;
            IsDead = false;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (IsDead) return;

        if (GetInput(out NetworkInputData data))
        {
            // 이동 처리 (격자 스냅핑)
            if (!_isMoving && data.direction.magnitude > 0)
            {
                Vector3 moveDir = new Vector3(Mathf.Round(data.direction.x), 0, Mathf.Round(data.direction.y));
                if (moveDir.magnitude > 0)
                {
                    _targetPosition = transform.position + moveDir;
                    _isMoving = true;
                }
            }

            // 폭탄 설치
            if (data.isBombPressed && _activeBombs < MaxBombs)
            {
                Rpc_SpawnBomb(new Vector3(Mathf.Round(transform.position.x), 0, Mathf.Round(transform.position.z)));
            }
        }

        if (_isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetPosition, CurrentSpeed * Runner.DeltaTime);
            if (Vector3.Distance(transform.position, _targetPosition) < 0.01f)
            {
                transform.position = _targetPosition;
                _isMoving = false;
            }
        }
    }

    public void OnExplosionHit()
    {
        if (Object.HasStateAuthority && !IsDead)
        {
            IsDead = true;
            // 로컬 UI 표시 (사망한 본인은 패배)
            if (loseUI != null) loseUI.SetActive(true);
            // 서버에 알림 등 추가 로직 가능
        }
    }

    public void ShowWinUI()
    {
        if (Object.HasInputAuthority && winUI != null) winUI.SetActive(true);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void Rpc_SpawnBomb(Vector3 pos)
    {
        var bombObj = Runner.Spawn(bombPrefab, pos, Quaternion.identity, Object.InputAuthority);
        var bomb = bombObj.GetComponent<BombController>();
        if (bomb != null)
        {
            bomb.Setup(BombPower, this);
            _activeBombs++;
        }
    }

    public void OnBombExploded()
    {
        if (Object.HasStateAuthority) _activeBombs--;
    }

    public void ApplyItem(ItemType type)
    {
        if (!Object.HasStateAuthority) return;
        
        switch (type)
        {
            case ItemType.Power: BombPower++; break;
            case ItemType.Count: MaxBombs++; break;
            case ItemType.Speed: CurrentSpeed += 0.5f; break;
        }
    }
}

public struct NetworkInputData : INetworkInput
{
    public Vector2 direction;
    public NetworkBool isBombPressed;
}