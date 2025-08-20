using UnityEngine;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 0f;
    [SerializeField] private float _jumpForce = 0f;
    private Rigidbody2D _playerRigidbody2D = default;
    [SerializeField] private PlayerStatus _playerStatus = PlayerStatus.GROUND;   //プレイヤーのステータスを管理する変数
    private Vector2 _assignmentVec = default;                   //プレイヤーの計算後のジャンプ速度を格納する変数
    private float _addGravity = 60f;                    //重力加速度
    private float _jumpTimer = 0f;                      //時間計測用
    private float _lowerLimitTime = 0.1f;            //ジャンプ経過時間の下限値
    private bool _isJumpPressed = false;
    private bool _keyLock = false;
    private const int EXPONENT = 2;                                 //べき指数
    private const float INITIAL_TIMER_VALUE = 0.2f;

    /// <summary>
    /// ジャンプ時のプレイヤーの状態
    /// </summary>   
    private enum PlayerStatus
    {
        GROUND,     //接地状態
        JUMPING,    //ジャンプ状態
        FALLING     //落下状態
    }

    void Start()
    {
        _playerRigidbody2D = this.GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        //スペースキーが押されていたら以下の処理
        if (Input.GetAxisRaw("Jump") > 0f)
        {
            //ジャンプキーが押された状態にする
            _isJumpPressed = !_keyLock;
        }
        //押されていなければ以下の処理
        else
        {
            //ジャンプキーがされていない状態にする
            _isJumpPressed = false;

            //キーロック解除
            _keyLock = false;
        }

        PlayerMove();
        PlayerJump();
    }

    private void PlayerMove()
    {
        float moveX = Input.GetAxisRaw("Horizontal");

        Vector2 moveVelocity = Vector2.zero;

        moveVelocity.x = _moveSpeed * moveX;

        _playerRigidbody2D.linearVelocity = moveVelocity;
    }

    /// <summary>
    /// プレイヤーのジャンプ処理
    /// </summary>
    private void PlayerJump()
    {
        
        switch (_playerStatus)
        {
            case PlayerStatus.GROUND:

                if (_isJumpPressed)
                {
                    _playerStatus = PlayerStatus.JUMPING;
                }


                break;

            case PlayerStatus.JUMPING:

                _playerRigidbody2D.linearVelocity = Vector2.zero;

                _jumpTimer += Time.deltaTime;

                //小ジャンプ
                if (_isJumpPressed || _lowerLimitTime > _jumpTimer)
                {
                    _assignmentVec.y = _jumpForce;
                    _assignmentVec.y -= _addGravity * Mathf.Pow(_jumpTimer, EXPONENT);
                }
                //大ジャンプ
                else
                {
                    _jumpTimer += Time.deltaTime;
                    _assignmentVec.y = _jumpForce;
                    _assignmentVec.y -= _addGravity * Mathf.Pow(_jumpTimer, EXPONENT);
                }

                GroundingJudgment();

                //速度を代入する
                SpeedAssignment(_assignmentVec);

                break;

            //落下状態
            case PlayerStatus.FALLING:

                //時間を計測
                _jumpTimer += Time.deltaTime;

                //y方向の速度をゼロにする
                _assignmentVec.y = 0f;

                //y方向の速度を徐々に減らしていく
                _assignmentVec.y -= _addGravity * Mathf.Pow(_jumpTimer, EXPONENT);

                //速度を代入する
                SpeedAssignment(_assignmentVec);

                break;

            default:
                break;
        }

    }

    private void GroundingJudgment()
    {
        //ジャンプ状態から落下状態へ移行
        if (0f > _playerRigidbody2D.linearVelocity.y)
        {
            //ステータスを落下状態に変更
            _playerStatus = PlayerStatus.FALLING;

            //y方向の速度をゼロにする
            _assignmentVec.y = 0f;

            //計測する時間に初期値を代入
            _jumpTimer = INITIAL_TIMER_VALUE;
        }

    }

    private void SpeedAssignment(Vector2 velocity)
    {
        _playerRigidbody2D.linearVelocity = velocity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_playerStatus == PlayerStatus.FALLING && collision.gameObject.CompareTag("Floor"))
        {
            _playerStatus = PlayerStatus.GROUND;
            _jumpTimer = 0f;                        //初期化
            _keyLock = true;                        //キー入力をロック
        }
    }
}
