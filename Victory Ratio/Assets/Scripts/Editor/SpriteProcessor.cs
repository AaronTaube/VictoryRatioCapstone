﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SpriteProcessor : AssetPostprocessor
{
    void OnPostprocessTexture(Texture2D texture)
    {
		string lowerCaseAssetPath = assetPath.ToLower();
		bool isInSpritesDirectory = lowerCaseAssetPath.IndexOf("/sprites/") != -1;

		if (isInSpritesDirectory)
		{
			TextureImporter textureImporter = (TextureImporter) assetImporter;
			textureImporter.textureType = TextureImporterType.Sprite;
			textureImporter.spritePixelsPerUnit = 128;
			textureImporter.filterMode = FilterMode.Point;
			textureImporter.textureCompression = TextureImporterCompression.Uncompressed;//Don't yet know if this works...
			textureImporter.SaveAndReimport();
		}
    }
}
