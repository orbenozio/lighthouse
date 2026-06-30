using UnityEngine;
using Crossroads.Engine;
using Crossroads.UI;

namespace Crossroads.Game.Lighthouse
{
    // Declarative wiring only (spec 12.2): fill the shell Config from the scene refs + content and run it.
    // All flow - menu / pause / Settings (music + sound) / save & continue / music / end - lives in
    // Crossroads.UI.GameShell, shared by every game. Cloning = copy the folder + swap Content/Theme (M7).
    public sealed class GameBootstrap : MonoBehaviour
    {
        [Header("Content (per-game)")]
        [SerializeField] private TextAsset storyJson;
        [SerializeField] private ResourceSet resources;
        [SerializeField] private Theme theme;
        [SerializeField] private int seed = 12345;

        [Header("UI (from Crossroads.UI)")]
        [SerializeField] private CardView cardView;
        [SerializeField] private ResourceBarView resourceBar;
        [SerializeField] private SwipeInput swipeInput;
        [SerializeField] private EndScreen endScreen;
        [SerializeField] private MessageOverlay messageOverlay;
        [SerializeField] private MenuOverlay menu;
        [SerializeField] private PauseButton pauseButton;
        [SerializeField] private AudioDirector audioDirector;

        [Header("Title / menu text")]
        [SerializeField] private string title = "Crossroads";
        [SerializeField] [TextArea] private string intro = "A short story of choices. Swipe left or right to decide.";

        private readonly GameShell _shell = new GameShell();

        private void Start()
        {
            _shell.Run(new GameShell.Config
            {
                format = GameShell.Format.Reigns,
                storyJson = storyJson, resources = resources, theme = theme, seed = seed,
                title = title, intro = intro,
                cardView = cardView, resourceBar = resourceBar, swipeInput = swipeInput,
                endScreen = endScreen, messageOverlay = messageOverlay, menu = menu,
                pauseButton = pauseButton, audioDirector = audioDirector,
            });
        }
    }
}
