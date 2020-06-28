using UnityEngine;
using UnityEditor;
using System.Collections;
using Assets.Scripts.TownTiles;
using UnityEngine.Tilemaps;

public class AnimatedWindMillTile : TownGameTile
{
    public Sprite[] sprites;
    public override bool GetTileAnimationData(Vector3Int position, ITilemap tilemap, ref TileAnimationData tileAnimationData)
    {
        tileAnimationData.animatedSprites = sprites;
        tileAnimationData.animationSpeed = 2;
        tileAnimationData.animationStartTime = 0;

        return true;
    }

#if UNITY_EDITOR
	// The following is a helper that adds a menu item to create a RoadTile Asset
	[MenuItem("Assets/Create/WindmillTile")]
	public static void CreateStructureTile()
	{
		string path = EditorUtility.SaveFilePanelInProject("Save Structure Tile", "New Structure Tile", "Asset", "Save Structure Tile", "Assets");
		if (path == "")
			return;
		AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<AnimatedWindMillTile>(), path);
	}
#endif
}
