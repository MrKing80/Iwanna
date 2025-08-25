using UnityEngine;

/// <summary>
/// �v���C���[�̍U�����Ǘ�����X�N���v�g
/// </summary>
public class PlayerShot : MonoBehaviour
{
    [SerializeField] private GameObject _bullet = default;  //�e�e�I�u�W�F�N�g
    private PlayerInput _playerAction = default;
    private BulletController _bulletStatus = default;
    private Vector2 _playerPosition = Vector2.zero;         //�v���C���[�̃|�W�V����

    private void Awake()
    {
        _playerAction = new PlayerInput();

        //InputSystem��L����
        _playerAction.Enable();
    }

    private void Update()
    {
        PlayerShooting();
    }

    private void OnDisable()
    {
        //InputSystem�𖳌���
        _playerAction.Disable();
    }

    /// <summary>
    /// �v���C���[�̏e�e��������
    /// </summary>
    private void PlayerShooting()
    {
        GameObject newBullet = default;     //�V���ɐ��������e�e���i�[����ϐ�   

        //�U���{�^�������͂��ꂽ��
        if (_playerAction.Player.Attack.WasPressedThisFrame())
        {
            //�v���C���[�̈ʒu���Q��
            _playerPosition = this.transform.position;

            //�e�e�𐶐����A���O��ύX
            newBullet = Instantiate(_bullet, _playerPosition, Quaternion.identity);
            newBullet.name = "Injected";
            _bulletStatus = newBullet.GetComponent<BulletController>();

            //�e�e���΂�
            _bulletStatus.BulletMove(this.transform.localScale.x);
        }
    }
}
