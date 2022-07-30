# Multiplayer top-down shooter game 

For my thesis I wanted to try out myself at game developing, therefore I made
a multiplayer shooter game in **C#**, using **Unity**.

The program is built on a **model-view architecture**. The networking is **host-client based** as others can join your game with the proper *port-forwarding*
settings. I used Unity's "*Netcode for GameObjects*" library which helped me deal with low level networking.

*I mainly focused on solving the arising problems of a multiplayer videogame and
learning the possibilities of the game engine.*

## About the game
-Your goal is to go from room to room clearing them from enemies. After you finished clearing a room the next one unlocks. The furthest you go, the more score you achieve.
-You can equip weapons and armor on yourself. Items can be found by clearing enemies or opening chests.
-After all player died the game restarts. 

## Gifs
**_Join game_**
![joining_starting](https://user-images.githubusercontent.com/43759583/181918200-57960108-c656-4421-96c4-db94c564f083.gif)

**_Equip items_**
![equip_items_inventory](https://user-images.githubusercontent.com/43759583/181918210-21c5dbd3-5768-4709-89fc-39df14c359c9.gif)

**_Drop items_**
![give_items_drop](https://user-images.githubusercontent.com/43759583/181918232-3ec88487-329c-4c8e-bf9c-0ce31a341e93.gif)

**_Combat_**
![fight_as_two](https://user-images.githubusercontent.com/43759583/181918278-05d82339-9dd9-4166-aa89-e34bf3fbb124.gif)

**_Dying_**
![one_of_you_dies](https://user-images.githubusercontent.com/43759583/181918316-117f7b36-ea8d-40a1-8e79-857655d1382f.gif)
