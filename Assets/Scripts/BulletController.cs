using UnityEngine;

/// <summary>
/// �e�e���Ǘ�����X�N���v�g
/// </summary>
public class BulletController : MonoBehaviour
{
    private Rigidbody2D _bulletRigidbody2D = default;
    private float _bulletSpeed = 300f;                  //�e�e�̔��ˑ��x

    private void Awake()
    {
        _bulletRigidbody2D = this.GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// �e�e�̈ړ�����
    /// </summary>
    /// <param name="moveX">�ړ�����(�v���C���[�̌���)</param>
    public void BulletMove(float moveX)
    {
        _bulletRigidbody2D.AddForce(Vector2.one * moveX * _bulletSpeed);
    }
}
