# Journal
### 22/04/26 - 22/06/26
###### Commit Name
<hr>

#### 22/04/26:
###### Initial check-in; Initial #2
Created the Project and Unity Project.

#### 23/04/26:
###### Add assets
Added a Cave Tileset and Player Character Tileset to the project assets.[^1]

#### 27/04/26:
###### Further Collection of sprites
Created design of Player Character with Sword in Piskel. \
Added Minotaur assets. [^2]

#### 06/05/26:
###### Added README containing first part of documentation and partly done index
Started on documentation in a README.md[^3] file. Started working on defining classes and basic features for these classes.

#### 08/05/26:
###### Finished Design of Characters and Environment section
Completed characters and environment sections in documentation. Added a few potential methods.

#### 11/05/26:
###### Case Study of Hunt the Wumpus including Genesis of Wumpus and translation of Wumpusworld to OOP paradigm. Started Procedural programming vs Object Oriented programming comparison.
Renamed README.md file to [Identifying and Defining](1.%20Identifying%20and%20Defining.md) and organised a documentation folder in project folder. \
Completed case study of Hunt the Wumpus including the Genesis of Wumpus[^4]. Translated aspects of Hunt the Wumpus into OOP paradigm design. \
Started to do comparison of Structural Programming and OOP Programming.

#### 14/05/26:
###### Created a new section called Proposed Solution where it explains the game idea. Created comparison table between Pascal language and OOP paradigm. Stated some success criteria.
Created a new file [Proposed Solution](0.%20Proposed%20Solution.md) to explain the game idea and inspiration. \
Finished comparison of Structural Programming and OOP Programming. \
Defined a few Success Criteria of the game.

#### 15/05/26:
###### Organised into files with suitable names, added diagrams such as lvl 0 and 1 dfd. Created structure for data dictionary.
Changed the structure of the project more such that the documentation file names are more intuitive. Added [Research and Planning](2.%20Research%20and%20Planning.md). \
Finished Success Criteria. \
Organised the case study into Research and Planning. \
Added Planning Diagrams including Lvl 0 and Lvl 1 DFDs. \
Layouted format for my Data Dictionary.

#### 17/05/26:
###### Edited sprites to have more accurate names. Added textmeshpro modules. Started on random tilemap generation from tutorial.
Set up the added Tileset into usable tiles. Started a tutorial[^5] for random map generation. \
Added TextMeshPro modules to be later used.

#### 18/05/26:
###### Continued in the tutorial. Did some random room generation and binary space partitioning.
Continued in the tutorial where it went through random room generation and Binary Space Partitioning. Scripts relating to changing map and generating walls[^6], and randomly generating rooms.

#### 20/05/26:
###### Finished procedural generation. Need to configure walls, but other stuff is done.
Finished with the tutorial but some things I need to configure because of asset differences in tutorial. Finished wall generation.

#### 22/05/26:
###### Finished tweaking tilebases such that the walls line up now. Started making background overlay.
Finished with wall configuration. Started working on overlaying the background with plain tiles.

#### 24/05/26:
###### Created new GridFirstDungeonGenerator which is modelled off the other dungeon generators. However, still some errors including forever loop.
Decided that the tutorial scripts for map generation did not fit with my game idea. However, I started a new script GridFirstDungeonGenerator[^7] that is somewhat modelled off of the tutorial scripts. \
Code in GridFirstDungeonGenerator still does not fully work as there is an infinitely running loop most likely in while loops.

#### 26/05/26:
###### Finished autogeneration. Also made spawn and boss room position feature.
Finished autogeneration but there is a random while statement error that only sometimes occurs. I added presets to player spawn and boss spawn rooms calculated from the size of the dungeon.

#### 28/05/26:
###### Finished the generation such that the map is a mathematically correct tree graph.
Finally finished the map generation such that the map does not have multiple paths from one to another room. This is defined as a tree graph in Mathematics. \
Took very long to debug it because of forever loops. \
Added creating new corridors based on joining rooms code, however it still does not fully work and further effort needs to be commit to it.

#### 29/05/26
###### Autogeneration++;.
Cleaned up some code in GridFirstDungeonGenerator. Broke down code into better systems such that it is more flexible with possible further code.

#### 3/06/26
###### Finished animation for now and fixed dungeon generation (it's been 3 weeks); Reorganising files; Finally finished dungeon generation TEMPLATE code.
Finally finished essential parts of GridFirstDungeonGenerator code. \
Organised map generation scripts into a folder 'Map Generation'. \
Started making animations for the player character without a sword. \
Organised assets into individual assets folder so that it is neater.

#### 5/06/26:
###### Coin and pedestal game art
Created coin and pedestal game assets in Piskel. Added both coin fragments and a full coin, both glowing and not glowing. Pedestal design also looks cool.

#### 11/06/26:
###### (Unfinished) Most of the code for calculating where the coin fragments would go. Too much logic for a few days.
+47,504 lines, -7,290 lines 😭. \
Added minor pedestals to coin and pedestal asset image. \
Added a coin controller script that controls where the coin fragments would spawn by calculating combinations of spawn rooms with the least amount of intersections. \
Added a coin fragment script. \
Added more to the Game Manager script to send messages to coin controller script. Because some code would run before other code across other scripts when the scene is played, I separated the game manager stuff into 3 stages. \
Decided to add a limit to the amount of iterations a while loop could do in GridFirstDungeonGenerator script. This limit solved the issue of an infinitely running loop but would still create errors because null references. \
Changed stuff in TilemapVisualiser script to make the generation of floor tiles to randomised between three tiles, with weighted chances. \
Added a pedestal script.

#### 13/06/26:
###### Found problem with unity freezing when playing. Started a little bit on coin fragments.
Major bug where Unity freezes, due to loops running for too long. \
Made coin fragments into prefabs. \
Updated CoinController script because I found out that it would generate lists that were too long for the system to handle in a short amount of time. Could not fix this issue yet. \
Changed coin fragment spawning rules such that it works on a score-based system to spawn the coins. This is calculated based on distance from the corners and distance from each other. The highest scoring combination of spawn rooms wins. \
Added more to CoinController script and GameManager script so that they work together. \
Cleaned up a bit of code in GridFirstDungeonGenerator script. \
Created an abstract class MapItem script that acts as a base for coin fragments, and possible sword in the future. \
Added inventory variable to PlayerController script.

#### 14/06/26:
###### Big progress, finished coin spawning, pedestal spawning, sword spawning. Partly done on UI, need to add animation.
+18,710 lines, -20,495 lines 😭v.2. \
Added UI elements such as health. \
Added visuals on Pedestal spawning coin fragments. \
Changed the GetCombinations() method in CoinController such that it now works better and produces way less lists. \
Changed a little bit to the coin fragment spawning such that now it uses standard deviation of distances, which helps produce more desirable results. \
When a coin fragment now touches the player, the coin fragment is added to the player's inventory and the GameObject destroyed. \
Game Manager script now controls both the player and the SwordController scripts. \
Added AddToInventory method to MapItem script. \
Added i-frames to PlayerController script. \
Added trigger to Sword script. \
Sword controller script now creates the Sword prefab into a calculated location. \
Added a UIHandler script that handles the player UI of the game. It is callable from everywhere and can set VisualElements visible or invisible. \
Added several .uxml scripts that reflect the UI of the game. This is part of Unity's UI Toolkit module and is very handy for non-interactive UI I think.

#### 17/06/26:
###### Finished new interaction animations. Some code on minotaur charging too.
Created Minotaur animations. \
Created particle systems for particle effects for Minotaur. \
Amended some small issues in CoinController script. \
Created EnemyAwarenessOfPlayer script that controls whether the entity is aware of the player position. \
Created the abstract class Entity script such that it can be applied to both the player character and the Minotaur. \
Created Minotaur script that controls how the minotaur moves, how the minotaur acts, where it spawns, and the particle system effects. \
Created MoveToPedestal script to control visuals for pedestal creating coin fragment and coin. \
Added code to Pedestal script to do as it should and create a coin for the player. \
Added interactions to the player so the player can interact with map stuff. \
Added more to UIHandler script so that it can now control inventory. UIHandler also creates fade visuals for interactions. \
Updated .uxml scripts to better fit the game.

#### 19/06/26:
###### Final touches on code, finished minotaur code and did start screen. need to make how to play and end screens.
Start of the final stretch of this project: \
Further animations to minotaur. \
Trimmed some greyed-out code that was not used. \
Added a Menu scene and corresponding MenuHandler that controls the menu and what happens when the user clicks buttons. \
Added AssembledCoinController script that is a basis for how the map would show the path to the Minotaur. \
Finished pedestal assembling coin visuals. \
Added player interaction with pedestal and stabbing the minotaur. \
Added GameObjects that resemble an endscreen. Added some control over it by UIHandler.

#### 20/06/26:
###### Finished game to basic extent. sorry. Really bad bug in which awake stopped running, code was skipped, for and foreach loop was skipped. apparently fixed by adding Debug.Log() :| 🪦; Update 0. Proposed Solution.md
Game is now finished: \
Minotaur animations are more refined now. \
Finished AssembledCoinController script and linked it to GameManager which would then change the map based on calculations. \
GameManager now controls whether moving GameObjects move by enabling or disabling scripts. \
For some reason, there was this really weird bug where the code would refuse to run and would just skip over loops. Even using debug mode and 'step through code' just skips the code. Eventually I found a fix by adding a **Debug.Log()** statement. \
Added game end text to EndScreen, including a bit of flavour text and lore.

#### 21/06/26:
###### Final final touches i think. it should work with camera sizes, stuff, etc.
After some simple alpha testing, I realised that resolution would change on different platforms and devices, so I found a fix to that and made UI conform to the resolution and size of the screen. \
Small alterations to other code.

#### 21/06/26 Part 2:
###### Trimming excess code, update to documentation.; Added extra parts. Finished class diagrams and key and put data dictionary in.
Final day for project: \
Trimmed some extra code. \
Added a lot to documentation in [Testing and Evaluating](3.%20Testing%20and%20Evaluating.md): \
Finished explanations of testing methodologies and provided some examples. Added review from Testers. \
Finished a structure chart of the game and put into [Research and Planning](2.%20Research%20and%20Planning.md) \
Exported Data Dictionary and added a link to it in Research and Planning. \
Completed Analysis and Evaluation of my game and compared to my success criteria defined earlier in the project. \
Created Class diagrams for the whole project, including for Characters, Map Generation, and Item Handling. Also added a Key for reference.

#### 22/06/26:
Added bibliography and completed journal in [Testing and Evaluating](3.%20Testing%20and%20Evaluating.md) and [Journal](Journal.md).




[^1]: See [Testing and Evaluating](3.%20Testing%20and%20Evaluating.md): Bibliography for Link.
[^2]: ibid.
[^3]: Since deprecated and deleted.
[^4]: See [^1]
[^5]: ibid.
[^6]: TilemapVisualiser and WallGenerator scripts.
[^7]: GridFirstDungeonGenerator script.