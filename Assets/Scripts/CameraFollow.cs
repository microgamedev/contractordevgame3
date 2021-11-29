using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform playerObject;
    [SerializeField] private Vector3 cameraOffset;

    private void FixedUpdate()
    {
        Vector3 newPosition = playerObject.position + cameraOffset;
        newPosition.x = cameraOffset.x;
        newPosition.y = cameraOffset.y;

        transform.position = Vector3.Lerp(transform.position, newPosition, 25f * Time.deltaTime);
    }
}