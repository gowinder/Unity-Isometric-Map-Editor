using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

    public float CameraScrollSpeed = 5f;

    void Update() {
        moveCamera();
    }

    void moveCamera() {
        // Moves camera with WASD/Arrows
        var verSpd = Input.GetAxisRaw("Vertical") * CameraScrollSpeed * Time.deltaTime;
        var horSpd = Input.GetAxisRaw("Horizontal") * CameraScrollSpeed * Time.deltaTime;
        if (Input.GetKey("up"))    transform.position += new Vector3(0, verSpd);
        if (Input.GetKey("left"))  transform.position += new Vector3(horSpd, 0);
        if (Input.GetKey("right")) transform.position += new Vector3(horSpd, 0);
        if (Input.GetKey("down"))  transform.position += new Vector3(0, verSpd);
    }
}
