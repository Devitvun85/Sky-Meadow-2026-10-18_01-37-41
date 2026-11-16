# Sky Meadow - Game Instructions

## What is Sky Meadow?

Sky Meadow is a short 3D platforming obstacle course. Guide the player across
the floating platforms, pass the doors, and collect the golden coins placed
around the level.

The current playable scene is `Assets/Project/Scenes/Section1.unity`.

## Main Objective

1. Start the level.
2. Cross the platform course without falling.
3. Collect all five golden coins. The counter at the top of the screen shows
   progress as `Coins: collected/5`.
4. Reach and explore the end of the course.

The current `Section1` scene does not contain a scripted finish trigger or
victory screen. Completing the course and collecting all five coins is
therefore the practical completion goal for this version.

## Controls

| Action | Keyboard / mouse |
|---|---|
| Move forward | `W` or `Up Arrow` |
| Move backward | `S` or `Down Arrow` |
| Move left | `A` or `Left Arrow` |
| Move right | `D` or `Right Arrow` |
| Look / turn | Move the mouse |
| Jump | `Space` |
| Release the mouse cursor | `Escape` |
| Capture the mouse cursor again | Click inside the Game view |

Movement follows the direction the player is facing. Mouse movement turns the
player horizontally and looks up or down.

## Starting the Game

When the level starts, the game is paused on the start screen.

1. Click **START**.
2. The mouse cursor is captured by the Game view.
3. Use `WASD` or the arrow keys to move and the mouse to look around.

If the mouse is released with `Escape`, click the Game view before trying to
move or look again.

## How to Play

### Crossing platforms

- Watch the gaps between platforms before moving.
- Use `Space` to jump across small gaps or onto higher platforms.
- Moving platforms travel from side to side. Step onto one and wait for a
  safe alignment before jumping off.
- Do not stand too close to an edge while looking around; the player moves
  relative to the direction they are facing.

### Collecting coins

Walk into a spinning golden coin to collect it. A collected coin disappears and
the counter increases by one. Each coin can only be collected once.

### Doors

Some doors open automatically when the player gets close. Another door is
configured to break when the player touches it. In the current `Section1`
scene, the `E` key is not required for the configured doors.

### Falling and failure

If the player falls below the level, the game shows **YOU FAILED** and stops
player control. Click **PLAY AGAIN** to reload the level from the beginning.

## HUD and Feedback

- **Coins:** Shows the number of collected coins out of five.
- **Start screen:** Appears before the level begins.
- **YOU FAILED:** Appears after falling from the course.
- **PLAY AGAIN:** Reloads the active level.

The project also contains timer code, but the current `Section1` scene does not
use a timer as a required objective.

## How to Run the Game in Unity

1. Open the `Sky Meadow` project in Unity.
2. Open `Assets/Project/Scenes/Section1.unity`.
3. Press the **Play** button in the Unity Editor.
4. Press **START** and play using the controls above.

`Section1.unity` is also the scene listed in the project's build settings. To
make a standalone version, use **File > Build Profiles** (or **File > Build
Settings**, depending on the Unity version), ensure `Section1` is included,
choose the target platform, and select **Build** or **Build and Run**.

## Tips

- Look ahead and plan your route before jumping.
- Use the moving platforms instead of trying to clear every gap directly.
- Collect coins while moving through the level, but prioritize landing safely.
- If the cursor seems stuck, press `Escape`, then click the Game view to
  continue.
