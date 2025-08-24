using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.Assertions.Must;

/// <summary>
/// �v���C���[�̎��S�������s���X�N���v�g
/// </summary>
public class PlayerDeath : MonoBehaviour
{
    [SerializeField] private GameObject _blood = default;   //���S���ɔ�юU��I�u�W�F�N�g
    private GameOverGenerate _gameOverGnerate = default;
    private GameObject _gameOverManager = default;
    private const int MAX_GENERATE_OBJECTS = 75;            //��юU��I�u�W�F�N�g�̍ő吔
    private Vector2 _playerPosition = Vector2.zero;         //�v���C���[�̃|�W�V����
    private const float MIN_FORCE = 5f;                     //�I�u�W�F�N�g���΂��ŏ��̗�
    private const float MAX_FORCE = 12.5f;                  //�I�u�W�F�N�g���΂��ő�̗�
    private const string KILLER_TAG = "Killer";     //�v���C���[���E���I�u�W�F�N�g�̃^�O��
    private const string GAMEOVER_TAG = "GameController";
    private bool _isDeath = false;                  //���񂾂��ǂ���

    private void Awake()
    {
        _gameOverManager = GameObject.FindGameObjectWithTag(GAMEOVER_TAG);

        _gameOverGnerate = _gameOverManager.GetComponent<GameOverGenerate>();
    }

    private void Start()
    {
        _isDeath = false;
    }

    private void FixedUpdate()
    {
        if (_isDeath)
        {
            DeathPlayer();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //�L���[�I�u�W�F�N�g�ɐG�ꂽ��
        if (collision.gameObject.CompareTag(KILLER_TAG))
        {
            //���S�����true�ɕύX
            _isDeath = true;
        }
    }

    /// <summary>
    /// �J�����O�ɏo���珈�����s��
    /// </summary>
    private void OnBecameInvisible()
    {
        //���S�����true�ɕύX
        _isDeath = true;
    }

    /// <summary>
    /// ���S�����Ƃ��̏������s��
    /// </summary>
    private void DeathPlayer()
    {
        print("���Ȃ��͎��ɂ܂���");
        GenetateBlood();
        _gameOverGnerate.GameOverUI();
        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// ���S���ɃI�u�W�F�N�g���΂�����
    /// </summary>
    private void GenetateBlood()
    {
        GameObject generatedBlood = default;    //������̃I�u�W�F�N�g���i�[����ϐ�

        //�I�u�W�F�N�g�𐶐�����
        for (int i = 0; i < MAX_GENERATE_OBJECTS; i++)
        {
            //�v���C���[�̃|�W�V�������i�[
            _playerPosition = transform.position;

            //�I�u�W�F�N�g�𐶐�
            generatedBlood = Instantiate(_blood, _playerPosition, Quaternion.identity);
            Rigidbody2D bloodrig2D = generatedBlood.GetComponent<Rigidbody2D>();

            //��΂������������_���Ɍ��߂�
            Vector2 randomDirection = Random.insideUnitCircle.normalized;

            //��΂��X�s�[�h�������_���Ɍ��߂�
            float randomSpeed = Random.Range(MIN_FORCE, MAX_FORCE);

            //�����_���Ɍ��߂��l���g���ăI�u�W�F�N�g���΂�
            bloodrig2D.AddForce(randomDirection * randomSpeed, ForceMode2D.Impulse);
        }
    }
}
