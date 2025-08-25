using UnityEngine;

/// <summary>
/// プレイヤーの攻撃を管理するスクリプト
/// </summary>
public class PlayerShot : MonoBehaviour
{
    [SerializeField] private GameObject _bullet = default;  //銃弾オブジェクト
    private PlayerInput _playerAction = default;
    private BulletController _bulletStatus = default;
    private Vector2 _playerPosition = Vector2.zero;         //プレイヤーのポジション

    private void Awake()
    {
        _playerAction = new PlayerInput();

        //InputSystemを有効化
        _playerAction.Enable();
    }

    private void Update()
    {
        PlayerShooting();
    }

    private void OnDisable()
    {
        //InputSystemを無効化
        _playerAction.Disable();
    }

    /// <summary>
    /// プレイヤーの銃弾を撃つ処理
    /// </summary>
    private void PlayerShooting()
    {
        GameObject newBullet = default;     //新たに生成した銃弾を格納する変数   

        //攻撃ボタンが入力されたか
        if (_playerAction.Player.Attack.WasPressedThisFrame())
        {
            //プレイヤーの位置を参照
            _playerPosition = this.transform.position;

            //銃弾を生成し、名前を変更
            newBullet = Instantiate(_bullet, _playerPosition, Quaternion.identity);
            newBullet.name = "Injected";
            _bulletStatus = newBullet.GetComponent<BulletController>();

            //銃弾を飛ばす
            _bulletStatus.BulletMove(this.transform.localScale.x);
        }
    }
}
