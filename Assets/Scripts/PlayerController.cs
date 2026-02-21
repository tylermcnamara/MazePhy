using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float force;

    private Rigidbody body_;
    private Vector2 moveInpVector;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        body_ = GetComponent<Rigidbody>(); // Get the Rigidbody component attached to the player object
    }

    void OnMove(InputValue moveValue)
    {
        moveInpVector = moveValue.Get<Vector2>(); // Get the input vector from the Input System
    }

    void OnLook(InputValue lookValue)
    {
        Vector2 lookInpVector = lookValue.Get<Vector2>(); // Get the look input vector from the Input System
        
    }

    void FixedUpdate()
    {
        Vector3 movementDir = new Vector3(-moveInpVector.x, 0, -moveInpVector.y); // Create a movement direction vector based on input
        body_.AddForce(movementDir * force, ForceMode.Force); // Ensures that movement is applied independently of sphere rotation
    }

}
