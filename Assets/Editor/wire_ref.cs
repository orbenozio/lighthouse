using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityAgentBridge.Editor;

namespace UnityAgentBridge.Editor.CustomTools
{
    // Sets a serialized OBJECT-REFERENCE field on a component to another scene object's component
    // (or the GameObject itself). set_property can only set value types; this handles references,
    // via SerializedObject + type-name resolution (no compile-time deps on game/UI assemblies).
    public static class wire_ref
    {
        [McpTool("wire_ref", "Wire a serialized reference field. host = GameObject holding the component; hostComponent = its type simple name (e.g. CardView); field = serialized field name; value = target GameObject; valueComponent = optional component type on the value (e.g. UnityEngine.UI.Image). Saves the scene.")]
        public static object Invoke(string host = "", string hostComponent = "", string field = "", string value = "", string valueComponent = "")
        {
            if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(hostComponent) || string.IsNullOrEmpty(field) || string.IsNullOrEmpty(value))
                throw new Exception("host, hostComponent, field, value are all required");

            var hostGo = GameObject.Find(host);
            if (hostGo == null) throw new Exception("host GameObject not found: " + host);
            Component comp = null;
            foreach (var c in hostGo.GetComponents<Component>())
                if (c != null && (c.GetType().Name == hostComponent || c.GetType().FullName == hostComponent)) { comp = c; break; }
            if (comp == null) throw new Exception("component '" + hostComponent + "' not found on " + host);

            var valueGo = GameObject.Find(value);
            if (valueGo == null) throw new Exception("value GameObject not found: " + value);

            UnityEngine.Object refObj = valueGo;
            if (!string.IsNullOrEmpty(valueComponent))
            {
                var vt = ResolveType(valueComponent);
                if (vt == null) throw new Exception("could not resolve valueComponent type: " + valueComponent);
                var vc = valueGo.GetComponent(vt);
                if (vc == null) throw new Exception(value + " has no " + valueComponent);
                refObj = vc;
            }

            var so = new SerializedObject(comp);
            var prop = so.FindProperty(field);
            if (prop == null) throw new Exception(hostComponent + " has no serialized field '" + field + "'");
            prop.objectReferenceValue = refObj;
            so.ApplyModifiedProperties();

            EditorUtility.SetDirty(comp);
            EditorSceneManager.MarkSceneDirty(hostGo.scene);
            EditorSceneManager.SaveScene(hostGo.scene);
            return new { ok = true, host, hostComponent, field, value, valueComponent, wired = refObj.GetType().Name };
        }

        private static Type ResolveType(string name)
        {
            var t = Type.GetType(name);
            if (t != null) return t;
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                t = asm.GetType(name);
                if (t != null) return t;
            }
            return null;
        }
    }
}
