using UnityEngine;

[CreateAssetMenu(fileName = "NewDecorationAsset", menuName = "Decoration/Decoration Asset")]
public class DecorationAsset : ScriptableObject
{
    public GameObject prefab;
    public Vector2 area;
    public Ground.Zone zone;
    public Vector3 offset;

    [Range(0, 1)]
    public float chances;
}
