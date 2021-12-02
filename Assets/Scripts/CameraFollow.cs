using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform playerObject;
    [SerializeField] private Vector3 cameraOffsetPlay;
    [SerializeField] private Vector3 cameraOffsetFinish;

    private Vector3 cameraOffset;
    private float cameraSpeed;
    private float cameraSpeedPlay = 25f;
    private float cameraSpeedFinish = 1;

    private bool isFinish = false;
    private bool isFollow = true;

    private void Start()
    {
        cameraOffset = cameraOffsetPlay;
        cameraSpeed = cameraSpeedPlay;
    }

    private void FixedUpdate()
    {
        if(isFollow)
        {
            Vector3 newPosition = playerObject.position + cameraOffset;
            newPosition.x = cameraOffset.x;
            newPosition.y = cameraOffset.y;

            transform.position = Vector3.Lerp(transform.position, newPosition, cameraSpeed * Time.deltaTime);

            if (isFinish)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(50f, 0f, 0f), 0.5f * Time.deltaTime);
            }
        }
    }

    public void FinishGate()
    {
        cameraOffset = cameraOffsetFinish;
        cameraSpeed = cameraSpeedFinish;
        isFinish = true;
    }

    public void StopFollow()
    {
        isFollow = false;
    }
}