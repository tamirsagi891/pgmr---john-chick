#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Tilemaps;

[CustomGridBrush(false, true, false, "ObjectBrush")]
[CreateAssetMenu(fileName = "GameObjectBrush", menuName = "Brushes/ObjectBrush", order = 1)]
public class ObjectBrush : GridBrush
{
    public GameObject brushPrefab;

    public override void Paint(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
    {
        // Don't allow editing palettes
        if (brushTarget.layer == 31)
            return;

        Vector3 cellSize = gridLayout.cellSize;
        Vector3 offset = cellSize / 2; // Calculate offset to center

        // Instantiate prefab at the correct location
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(brushPrefab);
        if (instance != null)
        {
            Undo.MoveGameObjectToScene(instance, brushTarget.scene, "Paint Prefabs");
            Undo.RegisterCreatedObjectUndo(instance, "Paint Prefabs");
            instance.transform.SetParent(brushTarget.transform);
            instance.transform.position = gridLayout.CellToWorld(position) + offset;
        }
    }


    public override void Erase(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
    {
        // Don't allow editing palettes
        if (brushTarget.layer == 31)
            return;

        // Find all prefab instances at the location and delete them
        BoundsInt bounds = new BoundsInt(position, Vector3Int.one);
        var objectsInCell = Physics2D.OverlapBoxAll(gridLayout.CellToWorld(position), new Vector2(1, 1), 0);
        foreach (var objectInCell in objectsInCell)
        {
            // Make sure to only delete objects of the correct prefab
            if (PrefabUtility.GetCorrespondingObjectFromSource(objectInCell.gameObject) == brushPrefab)
            {
                Undo.DestroyObjectImmediate(objectInCell.gameObject);
            }
        }
    }
}
#endif