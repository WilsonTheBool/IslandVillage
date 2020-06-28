using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using Assets.Scripts;

namespace Assets.Scripts.TownTiles
{
	public class ResourceTile : TownGameTile
	{
		public Sprite[] spriteForNone;
		public Sprite[] spriteForLittle;
		public Sprite[] spriteForHalf;
		public Sprite[] spriteForFull;

		public void UpdateAnimationState(ResourceObject owner)
		{

		}

		

	#if UNITY_EDITOR
		// The following is a helper that adds a menu item to create a RoadTile Asset
		[MenuItem("Assets/Create/ResourceTile")]
		
		public static void CreateStructureTile()
		{
			string path = EditorUtility.SaveFilePanelInProject("Save Resource Tile", "New Resource Tile", "Asset", "Save Resource Tile", "Assets");
			if (path == "")
				return;
			AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<ResourceTile>(), path);
		}
	#endif
	}
}
