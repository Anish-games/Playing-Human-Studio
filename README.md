Main Menu

<img width="309" height="667" alt="image" src="https://github.com/user-attachments/assets/aa258d5a-ee31-493f-a6e1-1280cb6f279f" />

Test 1

<img width="311" height="670" alt="image" src="https://github.com/user-attachments/assets/7a30762b-235a-49b3-a717-5e02d0c6d24e" />

Test 2

<img width="309" height="672" alt="image" src="https://github.com/user-attachments/assets/dfa7164a-d68d-4555-90ea-e1eac16efaa0" />




Project Overview

This Unity project contains two separate 2D scenes created as part of the Unity Game Developer test. Each scene focuses on a different core skill: UI logic and UI layout responsiveness.

Test 1 – Batting Order UI (Functionality)

The first scene implements a batting order management screen.

When the scene loads, the user sees a vertical list of 11 players. Each player row contains Up and Down buttons that allow reordering of the list.

Players can move up or down in the list using the buttons.

The first player cannot move up, and the last player cannot move down, preventing invalid actions.

Any change made updates the UI immediately.

The screen includes the following buttons:

Auto Populate
Automatically generates or shuffles the player order randomly.

Continue
Saves the current batting order locally using persistent storage.
After saving, a confirmation popup appears.
The game then returns to the main screen.
When the user revisits Test 1, the saved order is restored.

Back
Discards all changes made during the current visit.
The order is reset to the previously saved or default state.
A reset confirmation popup is shown.

This test focuses on UI interaction, list reordering logic, edge-case handling, and local data persistence.

Test 2 – Match UI Screen (UI & Responsiveness)

The second scene recreates the provided match UI reference.

The UI layout is built using proper hierarchy and anchors.

No gameplay or button functionality is implemented.

The screen scales correctly across different Android screen sizes using appropriate Canvas Scaler settings.

All UI elements maintain their layout and proportions on different resolutions.

This test focuses on UI layout accuracy, responsiveness, and visual polish.
