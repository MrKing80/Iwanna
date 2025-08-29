using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class KillerEventController : MonoBehaviour
{
    [SerializeField, Header("トラップ化するオブジェクト")]
    private GameObject[] _killers = default;

    [SerializeField, Header("トラップの種類")]
    private TrapType _trapType = default;

    [SerializeField, Header("トラップの射出速度")]
    private float _injectionspeed = 0f;

    [SerializeField, Header("トラップの移動速度")]
    private float _moveSpeed = 0f;

    [SerializeField, Header("トラップが移動する際の終点")]
    private Vector2 _moveEndPoint = Vector2.zero;

    [SerializeField, Header("トラップの効果音")]
    private AudioClip _killerSound = default;

    private Rigidbody2D[] _trapRigidbodys = default;
    private AudioSource _audioSource = default;
    private CircleCollider2D _collider2D = default;
    private Vector2 _myPosition = Vector2.zero;
    private bool _isTrapMoving = false;

    private enum TrapType
    {
        Pitfall,            //落とし穴
        FallingObjects,     //オブジェクトが落下してくる
        JumpOutObjects,     //オブジェクトが飛来してくる
        MoveObjects,        //オブジェクトが移動する
        ParallelTranslation, //オブジェクトが平行移動する
        TowardsThePlayer    //プレイヤーを狙って飛んでくる
    }

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _collider2D = GetComponent<CircleCollider2D>();

        _trapRigidbodys = new Rigidbody2D[_killers.Length];

        for (int i = 0; i < _killers.Length; i++)
        {
            _myPosition = _killers[i].transform.position;
        }
    }

    private void Update()
    {
        if (_isTrapMoving)
        {
            TrapMove();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (_trapType)
        {
            case TrapType.Pitfall:

                if (_killerSound != null)
                {
                    _audioSource.PlayOneShot(_killerSound);
                }

                for (int i = 0; i < _killers.Length; i++)
                {
                    _killers[i].gameObject.SetActive(false);
                }

                _collider2D.enabled = false;

                break;
            case TrapType.FallingObjects:

                if (_killerSound != null)
                {
                    _audioSource.PlayOneShot(_killerSound);
                }

                for (int i = 0; i < _killers.Length; i++)
                {
                    _trapRigidbodys[i] = _killers[i].GetComponent<Rigidbody2D>();

                    _trapRigidbodys[i].bodyType = RigidbodyType2D.Dynamic;

                    _trapRigidbodys[i].linearVelocity = Vector2.down * _injectionspeed;
                }

                _collider2D.enabled = false;

                break;
            case TrapType.JumpOutObjects:

                if (_killerSound != null)
                {
                    _audioSource.PlayOneShot(_killerSound);
                }

                for (int i = 0; i < _killers.Length; i++)
                {
                    _trapRigidbodys[i] = _killers[i].GetComponent<Rigidbody2D>();

                    _trapRigidbodys[i].bodyType = RigidbodyType2D.Dynamic;

                    _trapRigidbodys[i].linearVelocity = Vector2.up * _injectionspeed;
                }

                _collider2D.enabled = false;

                break;

            case TrapType.TowardsThePlayer:

                if (_killerSound != null)
                {
                    _audioSource.PlayOneShot(_killerSound);
                }

                for (int i = 0; i < _killers.Length; i++)
                {
                    _trapRigidbodys[i] = _killers[i].GetComponent<Rigidbody2D>();

                    _trapRigidbodys[i].bodyType = RigidbodyType2D.Dynamic;

                    _trapRigidbodys[i].linearVelocity = -_myPosition * _injectionspeed;
                }

                _collider2D.enabled = false;

                break;

            case TrapType.MoveObjects:

                if (_killerSound != null)
                {
                    _audioSource.PlayOneShot(_killerSound);
                }

                _isTrapMoving = true;

                _collider2D.enabled = false;

                break;
            case TrapType.ParallelTranslation:

                if (_killerSound != null)
                {
                    _audioSource.PlayOneShot(_killerSound);
                }

                _isTrapMoving = true;

                _collider2D.enabled = false;

                break;
            default:
                break;
        }
    }

    private void TrapMove()
    {
        for (int i = 0; i < _killers.Length; i++)
        {
            if (_trapType == TrapType.MoveObjects)
            {
                _killers[i].transform.position = Vector2.MoveTowards
                    (
                    _killers[i].transform.position,
                    _moveEndPoint,
                    _moveSpeed * Time.deltaTime
                    );

                if ((Vector2)_killers[i].transform.position == _moveEndPoint)
                {
                    _isTrapMoving = false;
                }
            }
            else if (_trapType == TrapType.ParallelTranslation)
            {
                _killers[i].transform.position = Vector2.MoveTowards
                    (
                    _killers[i].transform.position,
                    new Vector2(_moveEndPoint.x, _killers[i].transform.position.y),
                    _moveSpeed * Time.deltaTime
                    );

                if (_killers[i].transform.position.x == _moveEndPoint.x)
                {
                    _isTrapMoving = false;
                }

            }

        }

    }

    private void OnBecameInvisible()
    {
        if (_trapType != TrapType.Pitfall && _trapType != TrapType.MoveObjects)
        {
            for (int i = 0; i < _killers.Length; i++)
            {
                _killers[i].gameObject.SetActive(false);
            }
        }
    }
}
