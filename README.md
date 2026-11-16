# Sky Meadow

Sky Meadow is a 3D platforming obstacle-course game made in Unity. The player
must cross floating platforms, avoid falling, pass through doors, and collect
the golden coins placed throughout the level.

## Game Goal

Complete the `Section1` course while collecting all five golden coins. The HUD
shows progress as `Coins: collected/5`.


## Rules and Gameplay

1. Stay on the platforms and avoid falling below the level.
2. Use jumps to cross gaps and reach higher platforms.
3. Moving platforms travel from side to side. Wait for a safe landing before
   jumping off.
4. Walk into a golden coin to collect it. Each coin can only be collected once.
5. Some doors open automatically when the player approaches. Another door is
   configured to break when touched.
6. Falling displays **YOU FAILED** and stops player control.
7. Select **PLAY AGAIN** after failure to reload the level.

## Controls

| Action                   | Keyboard / mouse           |
| ------------------------ | -------------------------- |
| Move forward             | `W` or `Up Arrow`          |
| Move backward            | `S` or `Down Arrow`        |
| Move left                | `A` or `Left Arrow`        |
| Move right               | `D` or `Right Arrow`       |
| Look and turn            | Move the mouse             |
| Jump                     | `Space`                    |
| Release the mouse cursor | `Escape`                   |
| Capture the cursor again | Click inside the Game view |

Movement follows the direction the player is facing. The `E` key is not
required for the doors configured in the current playable scene.

## Screenshots

### Start Screen

![Sky Meadow start screen](Assets/Project/view/start.png)

### Platform Course

![Sky Meadow platform course](Assets/Project/view/latform.png)

## Unity Version and Technologies

- **Unity:** `6000.3.8f1`
- **Render pipeline:** Universal Render Pipeline
- **Input package:** Unity Input System `1.20.0`
- **Playable scene:** `Assets/Project/Scenes/Section1.unity`

## Setup and Running

### Requirements

- Unity Hub
- Unity `6000.3.8f1`
- A computer capable of running Unity

### Run in the Unity Editor

1. Clone or download this repository.
2. Open the project folder in Unity Hub.
3. Select Unity `6000.3.8f1` if Unity asks for an editor version.
4. Open `Assets/Project/Scenes/Section1.unity`.
5. Press the **Play** button.
6. Click **START**, then use the controls above.

## Project Structure

| Path                                   | Purpose                                              |
| -------------------------------------- | ---------------------------------------------------- |
| `Assets/Project/Scenes/Section1.unity` | Main playable level                                  |
| `Assets/Scripts/`                      | Player, coin, platform, camera, and gameplay scripts |
| `Assets/Sat Productions/`              | Easy Door System assets and scripts                  |
| `Assets/kenney_platformer-kit/`        | Platformer environment assets                        |
| `ProjectSettings/`                     | Unity project and build configuration                |
| `GAME_INSTRUCTIONS.md`                 | Detailed player instructions                         |


## Team Contributions

The current Git history shows the following contributions:

| Member     | Contribution                                                      |
| ---------- | ----------------------------------------------------------------- |
| Devitvun85 | Scene setup, documentation, set coin doors and screenshots |
| John       | Barrel-roll mechanic                                              |

## Submission Links

| Submission item | Link |
|---|---|
| Source repository | [Sky Meadow on GitHub](https://github.com/Devitvun85/Sky-Meadow-2026-10-18_01-37-41) |
| Gameplay video | [YouTube Video](https://youtu.be/QaZv5KhQVns) |
| Course submission form | [Microsoft Form](https://forms.cloud.microsoft/Pages/ResponsePage.aspx?id=7GGUHmJTKUOuRmH6PpHG0krixqZ-5JBLgDU3_E9cNpdUQ0ZCU0hTSzJQVlNUSkwyREZYRktDTE5SSy4u)|

## Credits

- **Unity Technologies** - Unity Editor, runtime, Universal Render Pipeline,
  Input System, and TextMesh Pro.
- **Kenney** - platformer environment assets in
  `Assets/kenney_platformer-kit/`. See
  [`License.txt`](Assets/kenney_platformer-kit/License.txt).
- **Sat Productions** - Easy Door System assets in
  `Assets/Sat Productions/E-01/Easy Door System/`.
