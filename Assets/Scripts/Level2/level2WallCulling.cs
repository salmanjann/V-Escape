using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class level2WallCulling : MonoBehaviour
{
    private Renderer wallRenderer;

    void Start()
    {
        wallRenderer = GetComponent<Renderer>(); // Get the Renderer component
    }

    void Update()
    {
        if (wallRenderer != null)
        {
            gameObject.SetActive(IsVisibleFrom(wallRenderer, Camera.main));
        }
    }

    bool IsVisibleFrom(Renderer renderer, Camera camera)
    {
        if (camera == null) return false;

        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
    }
}
