using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Crossroads.Engine;
using Crossroads.UI;

namespace Crossroads.Game.Lighthouse
{
    // Lighthouse's own card-choice selection effect: as a side is dragged the lighthouse "beam" sweeps warm
    // light across the chosen plaque - it brightens from the cool resting hint toward the warm glow, swells,
    // and gains a growing warm halo, while the other side dims into the dark. This is a game-authored look;
    // it replaces the engine's DefaultCardChoiceFeedback purely by living on the Card GameObject (CardView
    // discovers it via GetComponent). No engine code is touched - all colors come from the resolved theme
    // tokens (ChoiceHintColor = cool resting, ChoiceGlowColor = warm beam, ChoiceTextColor = lit label).
    public sealed class LightSweepFeedback : CardChoiceFeedback
    {
        // The chosen side eases in (light "rushes" past the midpoint) so the sweep reads as a beam catching
        // the plaque rather than a linear fade.
        private const float MaxSwell = 0.11f;   // extra scale on the lit plaque at full drag
        private const float MaxHalo = 10f;      // outline spread (px) of the warm halo at full drag

        public override void ApplyDrag(CardView card, ChoiceSide side, float fraction)
        {
            float f = Mathf.Clamp01(fraction);
            float lit = f * f * (3f - 2f * f);   // smoothstep: the beam sweeps in, not a flat ramp
            bool leftActive = side == ChoiceSide.Left;

            Color hint = card.ChoiceHintColor;
            Color glow = card.ChoiceGlowColor;
            Color textLit = card.ChoiceTextColor;

            TMP_Text active = leftActive ? card.LeftChoiceLabel : card.RightChoiceLabel;
            TMP_Text other = leftActive ? card.RightChoiceLabel : card.LeftChoiceLabel;
            if (active != null) active.color = Color.Lerp(hint, textLit, lit);
            if (other != null) other.color = hint;

            SweepPlaque(card.LeftPlaque, card.LeftChoiceLabel, glow, leftActive ? lit : 0f, !leftActive && f > 0.001f);
            SweepPlaque(card.RightPlaque, card.RightChoiceLabel, glow, leftActive ? 0f : lit, leftActive && f > 0.001f);
        }

        public override void Reset(CardView card)
        {
            Color hint = card.ChoiceHintColor;
            Color glow = card.ChoiceGlowColor;
            if (card.LeftChoiceLabel != null) card.LeftChoiceLabel.color = hint;
            if (card.RightChoiceLabel != null) card.RightChoiceLabel.color = hint;
            SweepPlaque(card.LeftPlaque, card.LeftChoiceLabel, glow, 0f, false);
            SweepPlaque(card.RightPlaque, card.RightChoiceLabel, glow, 0f, false);
        }

        // Lights one plaque by its (already eased) drag amount: swell, warm the fill toward the beam glow, and
        // grow a warm halo (an Outline drawn in the glow color). A dimmed (non-active during a drag) plaque
        // recedes toward the dark sea so the chosen side clearly stands out.
        private static void SweepPlaque(Image bg, TMP_Text label, Color glow, float lit, bool dimmed)
        {
            float s = 1f + MaxSwell * lit;
            if (bg != null)
            {
                // Warm the plaque toward the beam color as the light sweeps in; a dimmed plaque sinks dark.
                Color warm = new Color(glow.r, glow.g, glow.b, 1f);
                bg.color = dimmed ? new Color(0.42f, 0.44f, 0.48f, 1f) : Color.Lerp(Color.white, warm, 0.45f * lit);
                bg.rectTransform.localScale = new Vector3(s, s, 1f);

                var halo = bg.GetComponent<Outline>();
                if (lit > 0.001f)
                {
                    if (halo == null) halo = bg.gameObject.AddComponent<Outline>();
                    halo.effectColor = new Color(glow.r, glow.g, glow.b, 0.9f * lit);
                    float d = MaxHalo * lit;
                    halo.effectDistance = new Vector2(d, -d);
                    halo.enabled = true;
                }
                else if (halo != null) halo.enabled = false;
            }
            if (label != null) label.rectTransform.localScale = new Vector3(s, s, 1f);
        }
    }
}
