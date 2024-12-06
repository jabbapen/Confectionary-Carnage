# Overview
Unity uses Scenes, which are similar to pages within a website. Since they are
implemented within the Unity editor, they're not covered by DocFX. Here's a
brief summary of our scenes:

## Main Menu:
A simple main menu to connect the game, level editor, and settings.
Uses Unity's built-in UI components to render buttons, behavior is handled by our [Menu](/api/Global.Menu.html) script.

## Editor:
Allows users to build levels, placing tiles and enemies. They can then upload
these levels to our backend, where they can be played by other players.

## Game:
The core gameplay loop, where players fight enemies and progress.