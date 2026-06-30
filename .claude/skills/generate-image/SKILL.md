---
name: generate-image
description: Generate an image by driving the user's logged-in ChatGPT Pro web session (no OpenAI API key, no extra cost). Runs the chatgpt-image-cli tool (gpt.py), waits for the image, and displays the saved PNG. Triggers on requests to generate/create/draw an image - 'generate an image of...', 'make me an image', 'create a picture of...', 'תייצר תמונה של...', 'תצייר לי...', 'צור לי תמונה'.
---

# Generate Image (via ChatGPT web session)

Generate an image from a text prompt by driving the user's logged-in ChatGPT
web session through `chatgpt-image-cli`. This uses the Pro plan the user already
pays for - **no OpenAI API key and no per-image API billing**.

## The tool

It lives at a fixed absolute path (this is a global skill, so never assume cwd):

```
C:\Users\orben\OneDrive\DEV\Projects\chatgpt-image-cli\gpt.py
```

It drives `chatgpt.com` with Playwright using a persistent profile under
`~/.chatgpt-cli` (login survives between runs).

## How to run it

1. **Decide the output path.** Default to the current working directory with a
   short slug of the prompt plus a timestamp, e.g. `./<slug>-<YYYYMMDD-HHMMSS>.png`.
   If the user named a file or folder, honor that instead.

2. **Collapse the prompt to a single line.** The tool types the prompt and then
   presses Enter; a literal newline mid-prompt can submit it early. Replace all
   newlines with spaces before passing it. Meaning is preserved.

3. **Run it** (from the project dir, so `gpt.py` resolves):

   ```bash
   cd "/c/Users/orben/OneDrive/DEV/Projects/chatgpt-image-cli"
   PROMPT='<single-line prompt>'
   python gpt.py image "$PROMPT" --out "<output-path>"
   ```

   - A real (headful) browser window opens - this is intentional, not a bug.
   - By default the chat runs inside the ChatGPT project `agent-cli-project`
     (auto-created) so it does not clutter the main chat list. Override with
     `--project <name>`, or `--no-project` to run in the main list. Usually leave
     the default.
   - It can take a few minutes. Use a long Bash timeout (e.g. 540000 ms).

4. **Parse the result.** The final stdout line is `SAVED <path>`. Take that path.

5. **Display it.** Read the saved PNG so the image renders in chat, and tell the
   user where it was saved.

## When it fails

- **`Could not find the ChatGPT prompt box` / login errors** -> the session
  expired. Run `python gpt.py login` (opens a browser; user logs in once, presses
  Enter in the terminal), then retry.
- **Timed out waiting for an image / wrong element grabbed** -> the chatgpt.com
  DOM likely changed and the `SELECTORS` dict at the top of `gpt.py` is stale.
  Use the **chrome-devtools MCP** to open the live page, inspect the real
  selectors (composer, generated image, assistant message), and update `SELECTORS`
  in `gpt.py`. Do not guess - read the live DOM, fix, retry.

## Notes

- Quiet personal use only - keep a human pace, do not hammer it.
- If the user wants a specific aspect ratio (e.g. 9:16), put it in the prompt
  text itself; the tool does not expose a size flag.
