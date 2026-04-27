using UnityEngine;

public class Hotdog : Interactable
{
    public GameObject hotdog;
    public GameObject hotdogSpawnLocation;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        m_prompt = "INTERACT";
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override bool IsInteractable(PlayerInteract player)
    {
        return true;
    }

    public override void Interact()
    {
        Instantiate(hotdog, hotdogSpawnLocation.transform.position, Quaternion.identity).transform.localScale = Vector3.one * 0.08f;

        
    }
}
