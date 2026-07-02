using System;
using UnityEngine;
using UnityEditor;
using UnityAgentBridge.Editor;
using Crossroads.UI;

namespace UnityAgentBridge.Editor.CustomTools
{
    // Show the EndScreen with a given ending text + image key for a press screenshot. Edit-mode only.
    public static class preview_ending
    {
        [McpTool("preview_ending", "Show the EndScreen for a screenshot (imageKey=survived|wreck|starved|fever|despair; text)")]
        public static object Invoke(
            string imageKey = "survived",
            string text = "The relief boat returns on the ninetieth day. The light never failed once. You did your duty.",
            string themePath = "Assets/Game/Content/theme.asset")
        {
            var canvas = GameObject.Find("Canvas");
            if (canvas == null) throw new Exception("Canvas not found (wire the scene first)");
            var end = canvas.GetComponent<EndScreen>();
            if (end == null) throw new Exception("no EndScreen on Canvas");
            var theme = AssetDatabase.LoadAssetAtPath<Theme>(themePath);
            end.SetTheme(theme);
            end.Show(text, imageKey, null, null);
            return new { ok = true, imageKey, text };
        }
    }
}
