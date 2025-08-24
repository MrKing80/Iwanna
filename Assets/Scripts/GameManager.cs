using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// �Q�[���̑S�̓I�ȊǗ�������X�N���v�g
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager instance;             //�C���X�^���X��ێ�����ϐ�
    private Image _gameOverImage = default;
    private PlayerInput _playerAction = default;

    private void Awake()
    {
        // �C���X�^���X���܂����݂��Ȃ��ꍇ�A�������g���C���X�^���X�Ƃ���
        if (instance == null)
        {
            instance = this;
            
            // ���̃V�[���Ɉړ����Ă��V���O���g����j�����Ȃ�
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // ���łɃC���X�^���X�����݂���ꍇ�́A�������g��j������
            Destroy(gameObject);
        }

        _playerAction = new PlayerInput();
        _playerAction.Enable();

        Transform gameManagerChild = this.gameObject.transform.GetChild(0);
        Transform gameManagerGrandChild = gameManagerChild.transform.GetChild(0);
        _gameOverImage = gameManagerGrandChild.GetComponent<Image>();

    }

    private void Start()
    {
        _gameOverImage.enabled = false;
    }

    private void Update()
    {
        Retry();
    }

    private void OnDestroy()
    {
        _playerAction.Disable();
    }

    /// <summary>
    /// ���g���C���s������
    /// </summary>
    private void Retry()
    {
        //���g���C�L�[�������ꂽ��
        if (_playerAction.Player.Retry.WasPressedThisFrame())
        {
            //���݂̃V�[�������[�h
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

            _gameOverImage.enabled = false;
        }
    }

}
