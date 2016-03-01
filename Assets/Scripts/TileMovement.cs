using UnityEngine;
using System.Collections;

public class TileMovement : MonoBehaviour {

    public Vector3 gridPosition; // Default vector position  of the tile
    public bool    selected = false;

    private float  hoverHeight = 0.5f;
    private float  speed = 4f;

    void Update() {
        // If tile is selected hover above the normal grid position
        // If it isn't tile is either falling into position or there
        if (selected)  Hover();
        else           Fall();
    }

    void LateUpdate() {
        // Tile needs to be selected next frame to continue being the special one
        // Ensures only 1 tile is selected at once
        selected = false;
    }

    void Hover() {
        if (transform.position.y < gridPosition.y + hoverHeight) {
            float y = transform.position.y + speed * Time.deltaTime;
            transform.position = new Vector3(transform.position.x, y, transform.position.z);
        } else
            transform.position = new Vector3(gridPosition.x, gridPosition.y + hoverHeight, gridPosition.z);

    }

    void Fall() {
        // Fall into position or reset position if too low
        if (transform.position.y > gridPosition.y) {
            float y = transform.position.y - speed * Time.deltaTime;
            transform.position = new Vector3(transform.position.x, y, transform.position.z);
        } else
            transform.position = gridPosition;
    }


}
