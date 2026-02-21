using UnityEngine;

public class FinishGateController : MonoBehaviour
{
    public float liftHeight = 2f;
    public float liftSpeed = 3f;

    private Vector3 originalPosition;
    private Vector3 liftedPosition;
    private bool lifting = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalPosition = transform.position;
        liftedPosition = originalPosition + Vector3.right * liftHeight;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            lifting = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            lifting = false;
    }
    // Update is called once per frame
    void Update()
    {
        Vector3 target = lifting ? liftedPosition : originalPosition;
        transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * liftSpeed);
    }
}
