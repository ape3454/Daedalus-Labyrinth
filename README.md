# **Daedalus' Labyrinth** (name pending)
#### *Assessment Two: IT and Software Object Oriented Programming Report*
---
## Index
1. [Design of Characters and Environment](#design-of-characters-and-environment) \
	1.1. [Characters](#characters) \
	1.2. [Environment](#environment) \
	1.3. [Objects and Classes](#objects-and-classes) \
	&nbsp;&nbsp;&nbsp;&nbsp;1.3.1. [Characters](#characters-1) \
	&nbsp;&nbsp;&nbsp;&nbsp;1.3.2. [Environment](#environment-1) \
	1.4. [Interations Between Objects](#interactions-between-objects) \
	&nbsp;&nbsp;&nbsp;&nbsp;1.4.1. [Player Methods](#player-methods) \
	&nbsp;&nbsp;&nbsp;&nbsp;1.4.2. [Minotaur Methods](#minotaur-methods) \
	&nbsp;&nbsp;&nbsp;&nbsp;1.4.3. [Other Small Creatures Methods](#other-small-creatures-methods)
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
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; ──► Player Character Object \
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; ──► Enemy Class \
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; ──► Minotaur Object \
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; ──► Other Small Creatures Objects

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

|              Player               |              Minotaur             |          Other Enemy NPC         |
| --------------------------------- | --------------------------------- | -------------------------------- |
| Movement control                  | AI Movement and Unique Behaviour  | AI Movement and Unique Behaviour |
| Player Health and Decrease Health | Enemy Health and Decrease Health  | Enemy Health and Decrease Health |
| Deal Damage                       | Deal Damage                       | Deal Damage                      |
| Animation and Sound               | Animation and Sound               | Animation and Sound              |

#### *Environment*:
──► Item Class \
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; ──► Old Coin/Tablet and Monolith Objects \
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; ──► Sword Object

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

### Interactions Between Objects:

#### *Player Methods*:
1. ChangeHealth(amount):
	- Change Player's health attribute by \<amount>
2. Attack():
	- Play attack animation
3. CollectItem(item):
	- Get name of \<item>
	- Destroy \<item> object
	- Add to GUI a symbol representing \<item>
4. InteractMonolith():
	- Play animation
	- Remove GUI symbol
	- Play monolith animation
	- Add GUI symbol


#### *Minotaur Methods*:
1. ChangeHealth(amount):
	- Change Minotaur's health attribute by \<amount>
2. Attack():
	- Play attack animation

#### *Other Small Creatures Methods*:
1. ChangeHealth(amount):
	- Change Creature's health attribute by \<amount>
2. Attack():
	- Play attack animation