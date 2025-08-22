using UnityEngine;

public class BulletStatus : MonoBehaviour
{
    private Rigidbody2D _bulletRigidbody2D = default;
    private float _bulletSpeed = 300f;

    private void Awake()
    {
        _bulletRigidbody2D = this.GetComponent<Rigidbody2D>();
    }

    public void BulletMove(float moveX)
    {
        _bulletRigidbody2D.AddForce(Vector2.one * moveX * _bulletSpeed);
    }
}
