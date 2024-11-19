using System.Collections;
using UnityEngine;
using static ArcadeCar;

public class AutoCarController : MonoBehaviour
{
    public float moveSpeed = 5f;           // Tốc độ di chuyển xe
    public float reverseSpeed = 3f;        // Tốc độ lùi
    public float turnSpeed = 30f;          // Tốc độ quay (góc lái)
    public float maxTurnAngle = 30f;       // Góc lái tối đa (quay trái hoặc phải)

    private Rigidbody rb;                  // Tham chiếu đến Rigidbody của xe
    private Axle frontAxle;                // Trục bánh trước
    private Axle rearAxle;                 // Trục bánh sau

    void Start()
    {
        // Lấy Rigidbody từ đối tượng hiện tại
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("Không tìm thấy Rigidbody trên đối tượng này!");
            return;
        }

        // Lấy các trục bánh xe (Axle) từ ArcadeCar
        var arcadeCar = GetComponent<ArcadeCar>();
        if (arcadeCar != null && arcadeCar.axles.Length >= 2)
        {
            frontAxle = arcadeCar.axles[0]; // Trục bánh trước
            rearAxle = arcadeCar.axles[1];  // Trục bánh sau
        }
        else
        {
            Debug.LogError("Cần ít nhất 2 trục (Axle) để điều khiển xe.");
            return;
        }

        // Bắt đầu điều khiển tự động
        StartCoroutine(AutomateCarMovement());
    }

    // Coroutine điều khiển tự động
    private IEnumerator AutomateCarMovement()
    {
        while (true)
        {
            // Di chuyển thẳng 2 giây
            yield return StartCoroutine(MoveForward(2f));

            // Quay trái 2 giây
            yield return StartCoroutine(TurnLeft(2f));

            // Quay phải 2 giây
            yield return StartCoroutine(TurnRight(2f));

            // Lùi 2 giây
            yield return StartCoroutine(MoveBackward(2f));
        }
    }

    // Di chuyển thẳng
    private IEnumerator MoveForward(float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            // Tạo vector di chuyển thẳng
            Vector3 moveDirection = transform.forward * moveSpeed;

            // Điều khiển vận tốc xe
            rb.velocity = new Vector3(moveDirection.x, rb.velocity.y, moveDirection.z);

            // Quay bánh xe (giữ cho góc lái là 0 khi đi thẳng)
            frontAxle.steerAngle = 0f;

            timer += Time.deltaTime;
            yield return null;
        }

        rb.velocity = Vector3.zero; // Dừng di chuyển
    }

    // Di chuyển lùi
    private IEnumerator MoveBackward(float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            // Tạo vector di chuyển lùi
            Vector3 moveDirection = -transform.forward * reverseSpeed;

            // Điều khiển vận tốc xe
            rb.velocity = new Vector3(moveDirection.x, rb.velocity.y, moveDirection.z);

            // Quay bánh xe (giữ cho góc lái là 0 khi đi lùi)
            frontAxle.steerAngle = 0f;

            timer += Time.deltaTime;
            yield return null;
        }

        rb.velocity = Vector3.zero; // Dừng di chuyển
    }

    // Quay trái
    private IEnumerator TurnLeft(float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            // Quay xe sang trái
            frontAxle.steerAngle = -maxTurnAngle;  // Điều chỉnh góc lái sang trái

            // Giữ xe di chuyển thẳng khi quay
            rb.velocity = transform.forward * moveSpeed;

            timer += Time.deltaTime;
            yield return null;
        }

        frontAxle.steerAngle = 0f; // Dừng quay sau khi hoàn thành
    }

    // Quay phải
    private IEnumerator TurnRight(float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            // Quay xe sang phải
            frontAxle.steerAngle = maxTurnAngle;  // Điều chỉnh góc lái sang phải

            // Giữ xe di chuyển thẳng khi quay
            rb.velocity = transform.forward * moveSpeed;

            timer += Time.deltaTime;
            yield return null;
        }

        frontAxle.steerAngle = 0f; // Dừng quay sau khi hoàn thành
    }
}
