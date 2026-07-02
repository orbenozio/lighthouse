using UnityEditor;
using UnityAgentBridge.Editor;

namespace UnityAgentBridge.Editor.CustomTools
{
    // Sets the WebGL default canvas size (PlayerSettings.defaultWebScreenWidth/Height) so the built
    // index.html canvas renders at the right aspect - portrait for this game. Run before build_webgl.
    public static class set_web_resolution
    {
        [McpTool("set_web_resolution", "Set the WebGL default canvas size (width, height) - portrait 540x960 for Lighthouse")]
        public static object Invoke(int width = 540, int height = 960)
        {
            PlayerSettings.defaultWebScreenWidth = width;
            PlayerSettings.defaultWebScreenHeight = height;
            AssetDatabase.SaveAssets();
            return new { ok = true, width = PlayerSettings.defaultWebScreenWidth, height = PlayerSettings.defaultWebScreenHeight };
        }
    }
}
