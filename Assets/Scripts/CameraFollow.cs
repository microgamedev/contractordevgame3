using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform playerObject;
    [SerializeField] private Vector3 cameraOffsetPlay;
    [SerializeField] private Vector3 cameraOffsetFinish;

    private Vector3 cameraOffset;
    private float cameraSpeed;
    private float cameraSpeedPlay = 25f;
    private float cameraSpeedFinish = 5;

    private bool isFinish = false;

    private void Start()
    {
        cameraOffset = cameraOffsetPlay;
        cameraSpeed = cameraSpeedPlay;
    }

    private void FixedUpdate()
    {
        Vector3 newPosition = playerObject.position + cameraOffset;
        newPosition.x = cameraOffset.x;
        newPosition.y = cameraOffset.y;

        transform.position = Vector3.Lerp(transform.position, newPosition, cameraSpeed * Time.deltaTime);

        if(isFinish)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(45f, 0f, 0f), 1f * Time.deltaTime);
        }
    }

    public void FinishGate()
    {
        cameraOffset = cameraOffsetFinish;
        cameraSpeed = cameraSpeedFinish;
        isFinish = true;
    }
}