using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Assets.Scripts;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Assets.Scripts.TownTiles
{
	class StructureTile : TownGameTile
	{
		

#if UNITY_EDITOR
		// The following is a helper that adds a menu item to create a RoadTile Asset
		[MenuItem("Assets/Create/StructureTile")]
		public static void CreateStructureTile()
		{
			string path = EditorUtility.SaveFilePanelInProject("Save Structure Tile", "New Structure Tile", "Asset", "Save Structure Tile", "Assets");
			if (path == "")
				return;
			AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<StructureTile>(), path);
		}
		#endif
	}


}
