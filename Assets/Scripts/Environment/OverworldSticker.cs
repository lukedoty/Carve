using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class OverworldSticker : MonoBehaviour
{
    [SerializeField]
    private RegistryIDSelector<Sticker> m_stickerID;
    private bool m_collected = false;

    public void Start()
    {
        if (GameManager.Sticker.HasSticker(m_stickerID))
        {
            Destroy(gameObject);
        }
        SpriteRenderer stickerRenderer = GetComponentInChildren<SpriteRenderer>();

        if (stickerRenderer == null)
        {
            Debug.LogError($"Overworld sticker of ID \"{m_stickerID}\" has no child sprite.");
            return;
        }

        Sprite stickerSprite = GameManager.Sticker.Registry[m_stickerID].StickerImage;
        stickerRenderer.sprite = stickerSprite;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!m_collected)
            {
                StartCoroutine(Collect());
            }
        }
    }

    private IEnumerator Collect()
    {
        GameManager.Sticker.ObtainSticker(m_stickerID);
        Animator animator = GetComponentInChildren<Animator>();
        if (animator != null)
        {
            animator.SetBool("Collected", true);
            transform.rotation = Camera.main.transform.rotation;
        } else
        {
            Debug.LogError("Sticker animator not found");
            Destroy(gameObject);
        }
        yield return new WaitForSeconds(1);
        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }
}