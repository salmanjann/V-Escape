using UnityEngine;
using UnityEngine.UI;
public class Minimap : MonoBehaviour
{
    public RectTransform minimapBox;  // The UI image representing the minimap
    public RectTransform playerMarker; // The UI image representing the player's position
    public RectTransform trapDoor; // The UI image representing the player's position
    public Transform player; // The actual player in the world
    public Transform cameraTransform; // Drag the camera here


    private Vector2 minimapSize = new Vector2(300, 340); // Minimap size in UI
    private Vector2 worldSize = new Vector2(60, 68); // World size in units

    void Update()
    {
        if (player == null || cameraTransform == null) return;

        // Normalize player's position relative to world size (origin is at center)
        float normalizedX = (player.position.x + worldSize.x / 2) / worldSize.x;
        float normalizedY = (player.position.z + worldSize.y / 2) / worldSize.y;

        // Convert normalized position to minimap coordinates
        float minimapX = (normalizedX * minimapSize.x) - (minimapSize.x / 2);
        float minimapY = (normalizedY * minimapSize.y) - (minimapSize.y / 2);

        // Apply the position to the player marker
        playerMarker.localPosition = new Vector3(minimapX, minimapY, 0);

        float cameraYRotation = cameraTransform.eulerAngles.y;
        playerMarker.rotation = Quaternion.Euler(0, 0, -cameraYRotation + 180);
    }

    public void PlaceTrapDoor(int floorNumber)
    {
        if (trapDoor == null) return;

        // Get the RectTransform of the trapdoor
        RectTransform trapDoorRect = trapDoor.GetComponent<RectTransform>();

        // Define the offset to place the trapdoor near the bottom corners
        float offsetX = ((minimapSize.x + 60) / 2) - (trapDoorRect.sizeDelta.x / 2) - 10;
        float offsetY = ((-minimapSize.y -78) / 2) + (trapDoorRect.sizeDelta.y / 2) + 10;

        // Even floors → Bottom Left, Odd floors → Bottom Right
        if (floorNumber % 2 == 0)
        {
            trapDoorRect.anchoredPosition = new Vector2(-offsetX, offsetY);
        }
        else
        {
            trapDoorRect.anchoredPosition = new Vector2(offsetX, offsetY);
        }
    }

}
