using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Player playerObject;
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
            var sample = playerObject.SplineSample;
            if (sample != null)
            {
                float angle = sample.rotation.eulerAngles.y; //Получаем текущее угол вращение игрока
                var rotatedOffset = Quaternion.Euler(0f, angle, 0f) * cameraOffset; //Вращаем сдвиг по углу вращению игрока

                Vector3 newPosition = sample.position + rotatedOffset + Vector3.up * playerObject.SplineYOffset;

                var angles = transform.eulerAngles; //Вращаем камеру чтобы она смотрела по направлению движения
                angles.y = angle;
                transform.eulerAngles = angles;
                transform.position = Vector3.Lerp(transform.position, newPosition, cameraSpeed * Time.deltaTime);

                if (isFinish)
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(47.5f, 0f, 0f), 0.5f * Time.deltaTime);
                }
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