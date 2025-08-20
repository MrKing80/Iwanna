using UnityEngine;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 0f;
    [SerializeField] private float _jumpForce = 0f;
    private Rigidbody2D _playerRigidbody2D = default;
    [SerializeField] private PlayerStatus _playerStatus = PlayerStatus.GROUND;   //�v���C���[�̃X�e�[�^�X���Ǘ�����ϐ�
    private Vector2 _assignmentVec = default;                   //�v���C���[�̌v�Z��̃W�����v���x���i�[����ϐ�
    private float _addGravity = 60f;                    //�d�͉����x
    private float _jumpTimer = 0f;                      //���Ԍv���p
    private float _lowerLimitTime = 0.1f;            //�W�����v�o�ߎ��Ԃ̉����l
    private bool _isJumpPressed = false;
    private bool _keyLock = false;
    private const int EXPONENT = 2;                                 //�ׂ��w��
    private const float INITIAL_TIMER_VALUE = 0.2f;

    /// <summary>
    /// �W�����v���̃v���C���[�̏��
    /// </summary>   
    private enum PlayerStatus
    {
        GROUND,     //�ڒn���
        JUMPING,    //�W�����v���
        FALLING     //�������
    }

    void Start()
    {
        _playerRigidbody2D = this.GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        //�X�y�[�X�L�[��������Ă�����ȉ��̏���
        if (Input.GetAxisRaw("Jump") > 0f)
        {
            //�W�����v�L�[�������ꂽ��Ԃɂ���
            _isJumpPressed = !_keyLock;
        }
        //������Ă��Ȃ���Έȉ��̏���
        else
        {
            //�W�����v�L�[������Ă��Ȃ���Ԃɂ���
            _isJumpPressed = false;

            //�L�[���b�N����
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
    /// �v���C���[�̃W�����v����
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

                //���W�����v
                if (_isJumpPressed || _lowerLimitTime > _jumpTimer)
                {
                    _assignmentVec.y = _jumpForce;
                    _assignmentVec.y -= _addGravity * Mathf.Pow(_jumpTimer, EXPONENT);
                }
                //��W�����v
                else
                {
                    _jumpTimer += Time.deltaTime;
                    _assignmentVec.y = _jumpForce;
                    _assignmentVec.y -= _addGravity * Mathf.Pow(_jumpTimer, EXPONENT);
                }

                GroundingJudgment();

                //���x��������
                SpeedAssignment(_assignmentVec);

                break;

            //�������
            case PlayerStatus.FALLING:

                //���Ԃ��v��
                _jumpTimer += Time.deltaTime;

                //y�����̑��x���[���ɂ���
                _assignmentVec.y = 0f;

                //y�����̑��x�����X�Ɍ��炵�Ă���
                _assignmentVec.y -= _addGravity * Mathf.Pow(_jumpTimer, EXPONENT);

                //���x��������
                SpeedAssignment(_assignmentVec);

                break;

            default:
                break;
        }

    }

    private void GroundingJudgment()
    {
        //�W�����v��Ԃ��痎����Ԃֈڍs
        if (0f > _playerRigidbody2D.linearVelocity.y)
        {
            //�X�e�[�^�X�𗎉���ԂɕύX
            _playerStatus = PlayerStatus.FALLING;

            //y�����̑��x���[���ɂ���
            _assignmentVec.y = 0f;

            //�v�����鎞�Ԃɏ����l����
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
            _jumpTimer = 0f;                        //������
            _keyLock = true;                        //�L�[���͂����b�N
        }
    }
}
