using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TileButtonPressed : MonoBehaviour {

    public int id = 0;

    void Start() {
        GetComponent<Button>().onClick.AddListener(() => { ButtonPressed(); });
    }

    public void ButtonPressed() {
        GetComponentInParent<CreateScrollList>().IsometricMap.selectedSprite = id;
    }
}
