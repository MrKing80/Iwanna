using UnityEngine;
using UnityEngine.SceneManagement;
public class CleaJugeScript : MonoBehaviour
{
    private const string CLEAR_SCENE_NAME = "ClearScene";
    private const string PLAYER_TAG = "Player";
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(PLAYER_TAG))
        {
            SceneManager.LoadScene(CLEAR_SCENE_NAME);
        }
    }
}
