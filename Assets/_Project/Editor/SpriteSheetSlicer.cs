using System.Collections.Generic;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEngine;

namespace Isometric2DGame.EditorTools
{
    public static class SpriteSheetSlicer
    {
        private const int PixelsPerUnit = 64;

        [MenuItem("Tools/Slice Selected Sprite Sheets/Pivot Bottom")]
        private static void SliceSelectionBottom() => SliceSelection(SpriteAlignment.BottomCenter);
        [MenuItem("Tools/Slice Selected Sprite Sheets/Pivot Center")]
        private static void SliceSelectionCenter() => SliceSelection(SpriteAlignment.Center);
        
        private static void SliceSelection(SpriteAlignment alignment)
        {
            var factory = new SpriteDataProviderFactories();
            factory.Init();

            AssetDatabase.StartAssetEditing();

            int sliced = 0, skipped = 0;
            try
            {
                foreach (var obj in Selection.objects)
                {
                    var path = AssetDatabase.GetAssetPath(obj);
                    var importer = AssetImporter.GetAtPath(path) as TextureImporter;
                    if (importer == null)
                    {
                        Debug.LogWarning($"Not a texture, skipped: {path}");
                        skipped++;
                        continue;
                    }

                    importer.GetSourceTextureWidthAndHeight(out int width, out int height);

                    if (width % height != 0)
                    {
                        Debug.LogWarning($"Sheet is not a single row of square frames, skipped: {path}");
                        skipped++;
                        continue;
                    }

                    importer.textureType = TextureImporterType.Sprite;
                    importer.spriteImportMode = SpriteImportMode.Multiple;
                    importer.spritePixelsPerUnit = PixelsPerUnit;
                    importer.filterMode = FilterMode.Point;
                    importer.textureCompression = TextureImporterCompression.Uncompressed;
                    importer.mipmapEnabled = false;

                    var dataProvider = factory.GetSpriteEditorDataProviderFromObject(importer);
                    dataProvider.InitSpriteEditorDataProvider();

                    var oldSpriteRects = dataProvider.GetSpriteRects();
                    var oldIds = new Dictionary<string, GUID>();
                    foreach(var oldRect in oldSpriteRects)
                    {
                        oldIds[oldRect.name] = oldRect.spriteID;
                    }

                    var fileName = System.IO.Path.GetFileNameWithoutExtension(path);
                    var rects = CreateFrameRects(fileName, width, height, alignment, oldIds);
                    dataProvider.SetSpriteRects(rects);

                    var idProvider = dataProvider.GetDataProvider<ISpriteNameFileIdDataProvider>();
                    var idPairs = new List<SpriteNameFileIdPair>();
                    foreach (var rect in rects)
                    {
                        idPairs.Add(new SpriteNameFileIdPair(rect.name, rect.spriteID));
                    }
                    idProvider.SetNameFileIdPairs(idPairs);

                    dataProvider.Apply();

                    importer.SaveAndReimport();
                    sliced++;
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }

            Debug.Log($"Sprite sheets sliced: {sliced}, skipped: {skipped}");
        }

        private static SpriteRect[] CreateFrameRects(string fileName, int width, int height, SpriteAlignment alignment, Dictionary<string, GUID> oldIds)
        {
            var cell = height;
            var frameCount = width / height;
            var rects = new SpriteRect[frameCount];

            for (int i = 0; i < frameCount; i++)
            {
                var name = $"{fileName}_{i}";
                var spriteRect = new SpriteRect()
                {
                    name = name,
                    rect = new Rect(i * cell, 0, cell, cell),
                    alignment = alignment,
                    spriteID = oldIds.TryGetValue(name, out var id) ? id : GUID.Generate(),
                };

                rects[i] = spriteRect;
            }

            return rects;
        }
    }
}