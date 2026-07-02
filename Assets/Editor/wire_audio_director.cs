using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityAgentBridge.Editor;

namespace UnityAgentBridge.Editor.CustomTools
{
    // Adds a Crossroads.UI.AudioDirector to the scene and wires it into GameBootstrap.audioDirector.
    // The scene shipped without it, so no music/sfx played and the audio toggles had nothing to drive.
    // Uses type strings + SerializedObject so this editor script needs no compile-time reference to the
    // game/UI assemblies.
    public static class wire_audio_director
    {
        [McpTool("wire_audio_director", "Add an AudioDirector to the scene and wire GameBootstrap.audioDirector to it. target = the GameBootstrap GameObject (default 'Game').")]
        public static object Invoke(string target = "Game")
        {
            var go = GameObject.Find(target);
            if (go == null) throw new Exception("GameObject not found: " + target);

            Component bootstrap = null;
            foreach (var c in go.GetComponents<Component>())
                if (c != null && c.GetType().Name == "GameBootstrap") { bootstrap = c; break; }
            if (bootstrap == null) throw new Exception("no GameBootstrap component on " + target);

            var adType = Type.GetType("Crossroads.UI.AudioDirector, Crossroads.UI");
            if (adType == null) throw new Exception("could not resolve type Crossroads.UI.AudioDirector");

            var ad = go.GetComponent(adType);
            if (ad == null) ad = Undo.AddComponent(go, adType);

            var so = new SerializedObject(bootstrap);
            var prop = so.FindProperty("audioDirector");
            if (prop == null) throw new Exception("GameBootstrap has no serialized field 'audioDirector'");
            prop.objectReferenceValue = ad;
            so.ApplyModifiedProperties();

            EditorUtility.SetDirty(bootstrap);
            EditorSceneManager.MarkSceneDirty(go.scene);
            EditorSceneManager.SaveScene(go.scene);
            return new { ok = true, target, audioDirector = ad.GetType().Name, scene = go.scene.path };
        }
    }
}
