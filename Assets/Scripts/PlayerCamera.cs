using UnityEngine;

public class PlayerCamera : MonoBehaviour
{

    public Transform target;                   // The player ball
    public float followSpeed = 5f;
    public float rotationSpeed = 2.5f;

    private Rigidbody targetRb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (target != null)
            targetRb = target.GetComponent<Rigidbody>();
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + new Vector3(0f, 5f, 0f); // 10 units directly above
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        Quaternion overheadRotation = Quaternion.Euler(90f, 180f, 0f); // Look straight down
        transform.rotation = Quaternion.Slerp(transform.rotation, overheadRotation, rotationSpeed * Time.deltaTime);
    }

            // Update is called once per frame
    void Update()
    {
        
    }
}
