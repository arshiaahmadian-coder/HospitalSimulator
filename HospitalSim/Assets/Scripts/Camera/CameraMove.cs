using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float edgeSize;

    private float width;
    private float height;
    private Vector3 mousePos;

    private void Update()
    {
        height = Screen.height;
        width = Screen.width;
        mousePos = Input.mousePosition;
        Vector3 pos = transform.position;

        if (mousePos.x >= width - edgeSize)
            pos += transform.right * moveSpeed * Time.deltaTime;

        else if (mousePos.x <= edgeSize)
            pos -= transform.right * moveSpeed * Time.deltaTime;

        if (mousePos.y >= height - edgeSize)
            pos += transform.forward * moveSpeed * Time.deltaTime;

        else if (mousePos.y <= edgeSize)
            pos -= transform.forward * moveSpeed * Time.deltaTime;

        transform.position = pos;
    }
}
