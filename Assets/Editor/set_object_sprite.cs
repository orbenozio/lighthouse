using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityAgentBridge.Editor;

namespace UnityAgentBridge.Editor.CustomTools
{
    // Assigns a Sprite (by asset path) to a named scene GameObject's Image. Used to give the gameplay
    // backdrop ("Background") an atmospheric image instead of a flat color - something the generic
    // set_property cannot do (it cannot resolve an object reference from a path).
    public static class set_object_sprite
    {
        [McpTool("set_object_sprite", "Assign a Sprite (by asset path) to a named GameObject's Image; tint=hex like FFFFFF optional. Saves the scene.")]
        public static object Invoke(string target = "", string spritePath = "", string tint = "FFFFFF")
        {
            if (string.IsNullOrEmpty(target)) throw new Exception("target is required");
            if (string.IsNullOrEmpty(spritePath)) throw new Exception("spritePath is required");

            var go = GameObject.Find(target);
            if (go == null) throw new Exception("GameObject not found: " + target);
            var img = go.GetComponent<Image>();
            if (img == null) throw new Exception("no Image component on " + target);

            var importer = AssetImporter.GetAtPath(spritePath) as TextureImporter;
            if (importer == null) throw new Exception("no texture importer at " + spritePath);
            if (importer.textureType != TextureImporterType.Sprite)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.SaveAndReimport();
            }
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
            if (sprite == null) throw new Exception("could not load a Sprite from " + spritePath);

            img.sprite = sprite;
            img.type = Image.Type.Simple;
            img.preserveAspect = false;
            if (ColorUtility.TryParseHtmlString("#" + tint, out var c)) img.color = c;

            EditorUtility.SetDirty(img);
            EditorSceneManager.MarkSceneDirty(go.scene);
            EditorSceneManager.SaveScene(go.scene);
            return new { ok = true, target, spritePath, sprite = sprite.name, scene = go.scene.path };
        }
    }
}
