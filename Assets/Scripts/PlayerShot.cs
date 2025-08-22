using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShot : MonoBehaviour
{
    [SerializeField] private GameObject _bullet = default;
    private PlayerInput _playerAction = default;
    private BulletStatus _bulletStatus = default;
    private Vector2 _playerPosition = Vector2.zero;

    private void Awake()
    {
        _playerAction = new PlayerInput();
         
        _playerAction.Enable();
    }
    private void Start()
    {

    }

    private void Update()
    {
        PlayerShooting();
    }

    private void OnDisable()
    {
        _playerAction.Disable();
    }

    private void PlayerShooting()
    {
        GameObject newBullet;

        if (_playerAction.Player.Attack.IsPressed())
        {
            _playerPosition = this.transform.position;

            newBullet = Instantiate(_bullet, _playerPosition, Quaternion.identity);
            newBullet.name = "Injected";
            _bulletStatus = newBullet.GetComponent<BulletStatus>();

            print(_bulletStatus);
            _bulletStatus.BulletMove(this.transform.localScale.x);
        }
    }
}
