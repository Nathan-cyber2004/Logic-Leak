# Logic Leak: A Python Learning Adventure

> A retro cyberpunk terminal puzzle game that teaches K-12 students the building blocks of Python.

**Play it:** https://nathan-cyber2004.itch.io/logic-leak | **Platforms:** [WebGL / Android / iOS]

---

## About

Logic Leak is a 2D educational game built in Unity. Instead of typing code, students **drag and drop code blocks** into a terminal-style editor to fix the "leaks" in a broken system. Each level teaches one core Python concept, with hints that explain *why* an answer is wrong.

There are no character controllers or physics. The whole game runs on Unity's UI Canvas system.

## Levels

| # | Level | Python Concept | Gameplay | Learning Objective |
|---|-------|----------------|----------|--------------------|
| 1 | The Power Grid | Variables & Data Types | Drag values into variable slots to power a door | Tell `int`, `string`, and `boolean` apart |
| 2 | The Security Gate | If / Elif / Else | Build conditions to decide what a robot does | Conditional branching and comparison (`==`, `<`) |
| 3 | The Supply Chain | Lists | Sort items into a list, append, index, and use `len()` | List indexing (starts at 0) |
| 4 | The Data Drill | For Loops | Use `range()` and a nested `if` to break a wall | Loops automate repetition |
| 5 | The System Core | Debugging | Patch 6 syntax bugs in a pre-written script | Spot colons, typos, quotes, and brackets |

## How the Game Works

The game does **not** include a real Python interpreter. Instead it uses a **pseudo-interpreter**: each blank slot has a list of accepted answers, and the validation engine compares the blocks the player placed with those answers. Wrong answers show a hint written for that specific mistake (for example, *"A single = assigns. To compare, use =="*).

## Tech Stack

| | |
|---|---|
| Engine | Unity 2022.3+ (LTS) |
| Language | C# |
| UI | Unity UI Canvas + TextMeshPro |
| Rendering | Universal Render Pipeline (URP) with Bloom for the neon glow |
| Reference resolution | 1920 x 1080 (Canvas Scaler: Scale With Screen Size) |
| Monetization | Google AdMob interstitial ads |
| Platforms | [WebGL / Android / iOS] |

## Repository Structure

```
/Assets
  /Scripts      C# logic: LevelManager, DraggableBlock, CodeSlot, AdManager, GameSession
  /Data         LevelData ScriptableObjects (one per level)
  /Prefabs      Reusable UI: CodeBlock, CodeSlot, CodeLine, Panel, Button
  /Scenes       MainMenu, LevelSelect, Level (shared by all levels), EndScreen
  /Fonts        TextMeshPro font assets
  /Audio        Retro sound effects
/Builds         Exported builds (ignored by git, see .gitignore)
/Docs           Screenshots and design documents
```

## Getting Started

### Requirements
- Unity Hub
- Unity **2022.3 LTS** (or newer 2022.3.x) with the build modules you need (WebGL, Android, iOS)
- Git

### Setup
1. Clone the repository:
   ```bash
   git clone [YOUR REPO URL]
   ```
2. Open **Unity Hub**, click **Add > Add project from disk**, and select the cloned folder.
3. Open it with Unity 2022.3 LTS. The first import may take a few minutes.
4. Open `Assets/Scenes/MainMenu` and press **Play**.

## Adding or Editing a Level

Levels are data, not code. Each one is a ScriptableObject, so you can add a level without writing a new script.

1. In the Project window, right-click in `Assets/Data` and choose **Create > LogicLeak > Level**.
2. Fill in the fields:
   - **Level Title** and **Story**: what the student reads at the top.
   - **Code Lines**: one entry per line of code. Write `{0}`, `{1}`, ... where a blank slot should appear. Example: `if status {0} "Friend" {1}`
   - **Slots**: one entry per blank, in order. For each, set the **Accepted Answers**, a **Default Hint**, and optional **Specific Hints** for common wrong answers.
   - **Blocks**: every block the player can drag, including distractors.
   - **Win Message** and **Show Ad After**.
3. Add the new asset to the level list in `[LevelSelect / GameSession / wherever you register levels]`.
4. Press Play and test it.

> Tip: Always include a few wrong "distractor" blocks and a specific hint for each. This is where most of the teaching happens.

## Monetization (AdMob)

The game shows an interstitial ad after Level 2 and Level 4, controlled by the **Show Ad After** checkbox on each LevelData asset.

1. Import the **Google Mobile Ads Unity plugin** ([developers.google.com/admob/unity](https://developers.google.com/admob/unity/quick-start)).
2. Set your App ID under **Assets > Google Mobile Ads > Settings**.
3. Set your ad unit IDs in `AdManager.cs`.

**Use Google's test IDs during development.** Real ad units can get an AdMob account flagged if you tap your own ads. Current test IDs are listed in the AdMob documentation.

## Building

| Platform | Steps |
|----------|-------|
| **WebGL** | File > Build Settings > WebGL > Build. Upload the output folder to a web host (e.g. itch.io, GitHub Pages). |
| **Android** | Switch platform to Android, set a package name, then build an APK or AAB. |
| **iOS** | Switch platform to iOS, build the Xcode project, then archive and sign it in Xcode (requires a Mac and Apple Developer account). |

**Before each build, check that:**
- The Canvas Scaler uses **1920 x 1080** and the UI looks right at other aspect ratios.
- Text is readable at small screen sizes.
- Test ad IDs have been replaced with live IDs **only** for the final release build.

## Design Notes

- **Art style:** retro cyberpunk terminal with neon green and pink on a dark background (`#07090D`), monospaced fonts, and Bloom for the glow.
- **Accessibility:** large text, simple language, and a hint on every wrong answer.
- **Difficulty:** Level 1 introduces values, and each later level adds a new idea: branching, lists, loops, then debugging everything together.

## Asset Credits

All third-party assets are listed with their licenses in [`ATTRIBUTION.md`](ATTRIBUTION.md).

| Asset | Author | License |
|-------|--------|---------|
| [Font name] | [Author] | [e.g. SIL Open Font License] |
| [Sound pack] | [Author] | [e.g. CC0] |

## Roadmap

- [x] Terminal UI and main menu
- [x] Drag-and-drop system
- [x] Validation engine
- [x] Levels 1 to 5
- [ ] AdMob integration and sound effects
- [ ] WebGL, Android, and iOS builds
- [ ] Final testing and documentation

## Contributing

1. Fork the repo and create a branch: `git checkout -b feature/my-feature`
2. Commit your changes with clear messages.
3. Open a pull request describing what you changed and why.

Please comment your code so that the next contributor can follow it.

## Contact

[Nathan Pereira] | [nathanpereira204@gmail.com]
