using System.Linq;
using System.Text;
using NUnit.Framework;
using UnityEditor;
using Crossroads.Engine;
using Crossroads.UI;

namespace Crossroads.Game.Lighthouse.Tests
{
    // Validation of the real Lighthouse content: loads story.json + resources.asset from disk and runs them
    // through the same StoryValidator the game uses at load time. Paths are local to this game project.
    public sealed class LighthouseContentTests
    {
        private const string StoryPath = "Assets/Game/Content/story.json";
        private const string ResPath = "Assets/Game/Content/resources.asset";

        [Test]
        public void Lighthouse_Story_Validates_NoErrors()
        {
            var storyAsset = AssetDatabase.LoadAssetAtPath<UnityEngine.TextAsset>(StoryPath);
            var resources = AssetDatabase.LoadAssetAtPath<ResourceSet>(ResPath);
            Assert.IsNotNull(storyAsset, "Lighthouse story.json missing at " + StoryPath);
            Assert.IsNotNull(resources, "Lighthouse resources.asset missing at " + ResPath);

            var story = StoryLoader.Parse(storyAsset.text);
            var issues = StoryValidator.Validate(story, resources);

            var errors = issues.Where(i => i.Severity == IssueSeverity.Error).ToList();
            var sb = new StringBuilder();
            foreach (var e in errors) sb.AppendLine(e.ToString());
            Assert.IsEmpty(errors, "Lighthouse content has validation errors:\n" + sb);
        }

        [Test]
        public void Lighthouse_Has_FourMeters_And_StartNode()
        {
            var resources = AssetDatabase.LoadAssetAtPath<ResourceSet>(ResPath);
            var storyAsset = AssetDatabase.LoadAssetAtPath<UnityEngine.TextAsset>(StoryPath);
            Assert.IsNotNull(resources);
            Assert.AreEqual(4, resources.resources.Length, "Lighthouse defines 4 meters");

            var story = StoryLoader.Parse(storyAsset.text);
            Assert.IsTrue(story.Nodes.Any(n => n.Id == story.StartNodeId), "startNodeId must resolve to a real node");
            Assert.GreaterOrEqual(story.Nodes.Count, 5, "Lighthouse has a real (if small) deck, not a stub");
        }
    }
}
