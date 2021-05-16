# 3D Unity Platformer
### Status: Work in Progress

A 3D Parkour/Shooter hybrid prototype developed using the <a href="https://unity.com/">Unity</a> game engine with two others.

![Demo of Game](https://thumbs.gfycat.com/IdealisticWateryCaecilian-size_restricted.gif)

## About The Game
A first-person 3D game developed using C# and the Unity game engine. It is a hybrid of a parkour/platforming game and a first-person shooter game. It is currently unfinished; so far having the Tutorial level has been constructed. The objective of the game is to traverse through different levels to try to reach the end. There are many obstacles and challenges to overcome throughout each level. 
<br>
<br>
Some features:
* First-person camera for more immersive parkour experience
* Movement options; movement and physics made using Unity's <a href="https://docs.unity3d.com/Manual/class-CharacterController.html">CharacterController</a>, most of the logic you can find in the [Scripts/PlayerController.cs](Scripts/PlayerController.cs) file
  * Standard walking/jumping/sprinting as in most 3D Games
  * A powerful knockback pistol that propels the player in the opposite direction that it was fired
  * Wall-running and wall-jumping
  * Sliding down slopes
* Standard hitscan rifle
* Pressure pads and shootable switches that alter the level environment
* Customizable keybinds, mouse sensitivity, and video settings in the in-game menu

## How To Play
To try out a demo of the game (currently has a Tutorial level), download the [Demo.zip](Demo.zip) file, extract its contents, then run the Game Project.exe file located in the extracted folder. 

Controls (default):

* Movement: 
  * <kbd>W</kbd> <kbd>A</kbd> <kbd>S</kbd> <kbd>D</kbd>: move forward, left, backwards, and right respectively
  * <kbd>space</kbd> Jump/wall-jump
  * <kbd>Left Shift</kbd> (hold): sprint
  * <kbd>Left Control</kbd>: slide down slope
* Camera:
  * Move mouse to look around
* Weapons:
  * <kbd>Left click</kbd>: fire selected weapon
  * <kbd>Right click</kbd>: aim-down sights with rifle
  * <kbd>1</kbd> <kbd>2</kbd>: switch to rifle (1) or knockback pistol (2)
  * <kbd>R</kbd>: reload selected weapon
* Other:
  * <kbd>Esc</kbd>: open in-game menu


  
  
