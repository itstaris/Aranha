using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    public float moveForce = 30f;      // Força aplicada
    public float maxSpeed = 5f;        // Velocidade máxima no eixo X
    public float stopFriction = 0.9f;  // Fator de desaceleração (entre 0 e 1)
    public float rotationSpeed = 10f;  // Velocidade de rotação

    private Rigidbody rb;
    private Quaternion targetRotation;

    // Ajuste esse offset para alinhar o modelo quando se move
    private Quaternion modelOffset = Quaternion.Euler(0, 180f, 0);

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;

        targetRotation = transform.rotation;
    }

    void FixedUpdate()
    {
        float inputX = Input.GetAxisRaw("Horizontal"); // -1, 0 ou 1
        Vector3 velocity = rb.velocity;

        if (inputX != 0)
        {
            if (Mathf.Abs(velocity.x) < maxSpeed)
            {
                rb.AddForce(new Vector3(inputX * moveForce, 0f, 0f), ForceMode.Force);
            }

            // Rotação suave na direção do movimento, aplica offset
            Vector3 moveDir = new Vector3(inputX, 0f, 0f);
            targetRotation = Quaternion.LookRotation(moveDir, Vector3.up) * modelOffset;
        }
        else
        {
            // Idle → olha para frente sem offset
            targetRotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);

            // Reduz velocidade no X
            velocity.x *= stopFriction;
            rb.velocity = new Vector3(velocity.x, velocity.y, velocity.z);
        }

        // Garante limite de velocidade
        velocity = rb.velocity;
        velocity.x = Mathf.Clamp(velocity.x, -maxSpeed, maxSpeed);
        rb.velocity = new Vector3(velocity.x, velocity.y, velocity.z);

        // Rotação suave
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
    }
}
