# **Daedalus' Labyrinth** (name pending)
#### *Assessment Two: IT and Software Object Oriented Programming Report*
---
## Index
1. [Design of Characters and Environment](#design-of-characters-and-environment) \
	1.1. [Characters](#characters) \
	1.2. [Environment](#environment) \
	1.3. [Objects and Classes](#objects-and-classes) \
	1.4. [Characters](#characters-1) \
	1.5. [Environment](#environment-1)
2. 

---

## 1. Design of Characters and Environment
### *Characters*:
1. Player Character:
	- Controlled by Player
	- 32x32 Pixel design
	- 8 Walking frames
	- Player Move/Attack Set
2. Minotaur:
	- Non-Player Enemy
	- 64x64 Pixel design
	- 3 Walking frames
	- Minotaur Move/Attack Set
3. Other small creatures:
	- Spiders (so generic ik)

### *Environment*:
1. Map:
	- 2.5D Style
	- Undergound Cave-like Labyrinth
	- Corridors and bigger rooms
2. Coin/Round Tablet and Monolith:
	- Coin/Tablet is split into 4 parts
	- Interact with each other
3. Sword:
	- Placed in random location
	- Deals damage toward enemy characters

### Objects and Classes:
#### *Characters*:
──► Character Class \
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; ──► Player Character \
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; ──► Enemy Class \
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; ──► Minotaur \
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; ──► Other Small Creatures

***Character Class Features*** \
\- General Movement \
\- Health \
\- Decrease Health \
\- Deal Damage \
\- Animation \
\- Sound

***Enemy Class Features*** \
\- AI Movement and Behaviour \
\- Health \
\- Decrease Health \
\- Deal Damage \
\- Animation \
\- Sound

|              Player              |              Minotaur             |          Other Enemy NPC         |
| -------------------------------- | --------------------------------- | -------------------------------- |
| Movement control                 |  AI Movement and Unique Behaviour | AI Movement and Unique Behaviour |
| Player Health and Decrease Heath | Enemy Health and Decrease Health  | Enemy Health and Decrease Health |
| Deal Damage                      | Deal Damage                       | Deal Damage                      |
| Animation and Sound              | Animation and Sound               | Animation and Sound              |

#### *Environment*:
──► Item Class \
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; ──► Old Coin/Tablet and Monolith \
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; ──► Sword

***Item Class*** \
\- Obtainable and Symbol in GUI \
\- Spawn at location \
\- Animation \
\- Sound

|              Old Coin/Tablet              |           Monolith           |                        Sword                       |
| ----------------------------------------- | ---------------------------- | -------------------------------------------------- |
| Obtainable								| Unobtainable                 | Obtainable                                         |
| Spawn at Determined Location              | Spawn at Determined Location | Spawn at Random Location                           |
| Passive Animation before and after Pickup | Passive animation            | Passive Animation and Swing Animation after Pickup |
| Ambient Sound before Pickup               | Ambient Sound                | Ambient Sound before Pickup                        |