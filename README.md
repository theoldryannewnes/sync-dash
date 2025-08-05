# Sync Dash

### A Unity 6 Game to Simulate Network Lag

 \#\# 📝 Overview

Sync Dash is a Unity 6 game designed to provide a hands-on demonstration of how network latency affects gameplay. The game features two characters: a primary player character and a "ghost" character that mirrors its movements.

The core concept is to simulate a player's actions being sent over a network, with the ghost character representing how those actions are received and rendered by another player's client.

## 🕹️ How to Play

### Player Controls

  - **Movement:** The primary player controls the green cube on the right side of the screen.
  - **Jump:** Click or tap anywhere on the screen to make the player character jump.

### Network Simulation

  - **The Ghost Character:** The character on the left is a "ghost" that follows the same movements as the player. Its actions are delayed to simulate network lag.
  - **Delay Slider:** The slider at the top of the screen allows you to configure the amount of delay (in milliseconds) applied to the ghost character's movement.

Observe how increasing the delay affects the ghost's ability to react to obstacles and collect orbs, providing a clear visual representation of high-latency gameplay.

## 🚀 Getting Started

To run this project, you will need to have Unity 6 installed.

1.  **Clone the Repository:**
    ```
    git clone https://github.com/your-username/sync-dash.git
    ```
2.  **Open in Unity:**
      * Open the Unity Hub.
      * Click "Open" and navigate to the cloned `sync-dash` folder.
3.  **Run the Scene:**
      * Open the `Menu` scene from the project window.
      * Press the ▶️ button to start the game.

## ⚙️ Built With

  * [**Unity 6**](https://unity.com/) - The game engine used.
  * **C\#** - The primary scripting language.
  * **UI Toolkit & TextMeshPro** - For the user interface components.

## 🧑‍💻 Author

  * **[Ryan Newnes]** - *Initial work* - [GitHub Profile](https://www.google.com/search?q=https://github.com/theoldryannewnes)
