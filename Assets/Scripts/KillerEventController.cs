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

    private Rigidbody2D[] _trapRigidbodys = default;
    private Vector2 _myPosition = Vector2.zero;
    private bool _isTrapMoving = false;

    private enum TrapType
    {
        Pitfall,            //落とし穴
        FallingObjects,     //オブジェクトが落下してくる
        JumpOutObjects,     //オブジェクトが飛来してくる
        MoveObjects,        //オブジェクトが移動する
        TowardsThePlayer    //プレイヤーを狙って飛んでくる
    }

    private void Start()
    {
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

                for (int i = 0; i < _killers.Length; i++)
                {
                    _killers[i].gameObject.SetActive(false);
                }

                break;
            case TrapType.FallingObjects:

                for (int i = 0; i < _killers.Length; i++)
                {
                    _trapRigidbodys[i] = _killers[i].GetComponent<Rigidbody2D>();

                    _trapRigidbodys[i].bodyType = RigidbodyType2D.Dynamic;

                    _trapRigidbodys[i].linearVelocity = Vector2.down * _injectionspeed;
                }

                break;
            case TrapType.JumpOutObjects:

                for (int i = 0; i < _killers.Length; i++)
                {
                    _trapRigidbodys[i] = _killers[i].GetComponent<Rigidbody2D>();

                    _trapRigidbodys[i].bodyType = RigidbodyType2D.Dynamic;

                    _trapRigidbodys[i].linearVelocity = Vector2.up * _injectionspeed;
                }

                break;

            case TrapType.TowardsThePlayer:

                for (int i = 0; i < _killers.Length; i++)
                {
                    _trapRigidbodys[i] = _killers[i].GetComponent<Rigidbody2D>();

                    _trapRigidbodys[i].bodyType = RigidbodyType2D.Dynamic;

                    _trapRigidbodys[i].linearVelocity = -_myPosition * _injectionspeed;
                }

                break;

            case TrapType.MoveObjects:

                _isTrapMoving = true;

                break;
            default:
                break;
        }
    }

    private void TrapMove()
    {
        for (int i = 0; i < _killers.Length; i++)
        {
            _killers[i].transform.position =
                    Vector2.MoveTowards(_killers[i].transform.position, _moveEndPoint, _moveSpeed * Time.deltaTime);

            if ((Vector2)_killers[i].transform.position == _moveEndPoint)
            {
                _isTrapMoving = false;
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
