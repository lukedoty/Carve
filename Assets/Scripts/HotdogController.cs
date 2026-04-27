using UnityEngine;

public class HotdogController : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField]
    private float hotdogForce = 10f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    
    void Start()
    {
        rb.AddForce(Vector3.forward * hotdogForce, ForceMode.Impulse);

        Destroy(gameObject, 10f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
