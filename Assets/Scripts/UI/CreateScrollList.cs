using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using System.Collections;

public class CreateScrollList : MonoBehaviour {

    public GameObject TileButton;
    public IsometricMapGenerator IsometricMap;

	void Update () {
        // Wait until tiles are loaded before populating list
        if (IsometricMap.GetComponent<IsometricMapGenerator>().tiles != null) {
            PopulateList();
            enabled = false; // Turn this off when job done.
        }
	}

    /// <summary> Grabs sprites from resources folder and creates buttons for them </summary>
    void PopulateList() {
        int i = 0;
        foreach (Sprite tile in IsometricMap.GetComponent<IsometricMapGenerator>().tiles) {
            GameObject newButton = Instantiate(TileButton) as GameObject;
            newButton.transform.SetParent(transform);
            newButton.GetComponent<Image>().sprite = tile;
            newButton.GetComponent<TileButtonPressed>().id = i++;
        }
    }
}
