using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityAgentBridge.Editor;
using Crossroads.Engine;
using Crossroads.UI;

namespace UnityAgentBridge.Editor.CustomTools
{
    // Bind a SPECIFIC story node to the CardView for layout preview (the wire tool only shows the first
    // node). Useful for tuning long speaker cards. Edit-mode only.
    public static class preview_card
    {
        [McpTool("preview_card", "Bind a specific story node to the CardView for layout preview (nodeId)")]
        public static object Invoke(string nodeId = "smuggler",
            string storyPath = "Assets/Game/Content/story.json",
            string resourcesPath = "Assets/Game/Content/resources.asset",
            string themePath = "Assets/Game/Content/theme.asset")
        {
            var story = AssetDatabase.LoadAssetAtPath<TextAsset>(storyPath);
            var resources = AssetDatabase.LoadAssetAtPath<ResourceSet>(resourcesPath);
            var theme = AssetDatabase.LoadAssetAtPath<Theme>(themePath);
            if (story == null || resources == null) throw new Exception("story/resources not found");

            var card = GameObject.Find("Canvas/Card");
            var canvas = GameObject.Find("Canvas");
            if (card == null || canvas == null) throw new Exception("Canvas/Card not found (wire the scene first)");
            var cardView = card.GetComponent<CardView>();
            var bar = canvas.GetComponent<ResourceBarView>();

            var storyData = StoryLoader.Parse(story.text);
            var engine = new EventEngine(storyData, resources, new Deck(storyData), 12345);
            engine.EnterNode(nodeId);
            if (engine.Current == null) throw new Exception("node not found: " + nodeId);

            cardView.Bind(ViewMapper.BuildNodeView(engine.Current), theme);
            if (bar != null) { bar.SetTheme(theme); bar.Bind(ViewMapper.BuildResourceViews(engine.State, resources, theme)); }

            if (!Application.isPlaying)
            {
                EditorUtility.SetDirty(cardView);
                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }
            return new { ok = true, node = engine.Current.Id, speaker = engine.Current.Speaker, body = engine.Current.Body };
        }
    }
}
