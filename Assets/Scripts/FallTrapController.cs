using UnityEngine;

public class FallTrapController : MonoBehaviour
{
    private const string PLAYER_TAG = "Player";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag(PLAYER_TAG))
        {
            this.gameObject.SetActive(false);
        }
    }
}
