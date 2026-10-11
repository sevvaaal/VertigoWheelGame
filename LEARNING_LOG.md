# Vertigo Learning Log

## Day 1 - Unity basics

### GameObjects and Components
- A GameObject is like an actor on a stage. By itself it does nothing.
- I give it behavior by attaching components (scripts, Image, Button...).
- My scripts become components by inheriting from MonoBehaviour.
- I never use `new` on a MonoBehaviour. Unity creates it when I attach it.

### Hierarchy vs Project window
- Project window = files on disk (scripts, images, scenes). The warehouse.
- Hierarchy = GameObjects in the currently open scene. The stage.
- GameObjects can be parent/child. A child moves, rotates, scales with its parent.
  (I accidentally made a child by dropping my script below an object instead of onto it.)

### Lifecycle (Unity calls these, not me)
- Awake: runs once when the object is created.
- Start: runs once, before the first frame.
- Update: runs every frame (~60 times per second).
- Unity runs ALL Awakes first, then ALL Starts, then Updates.
  - So in Awake: set up myself (e.g. find my own components).
  - In Start: talk to other objects, because they are all set up by then.

### Fields vs local variables
- A local variable (inside a method) is created fresh every call and thrown away after.
- A field (top of the class) lives as long as the object. It is the object's memory.
- That's why my update counter had to be a field.
- Update is already a loop (Unity calls it every frame), so I count frames with a field
  instead of using a for loop.

### Code style lessons
- Names must say what the variable holds: `updateCount`, not `hasLoggedUpdate` for a number.
- Avoid magic numbers: use `const int MaxUpdateLogs = 3;` instead of a bare 3.
- String interpolation: `$"{name}: Update {updateCount}"`.
- `[SerializeField] private` = private in code but editable in the Inspector (keeps encapsulation).

## Vertigo game design (I designed this)

| Thing | Knows | Does |
|---|---|---|
| RewardType (e.g. "Gold") | name, icon | nothing, pure data |
| Reward (e.g. "Gold x200") | a RewardType + amount | nothing, pure data |
| Slice | bomb OR a Reward | nothing, pure data |
| Wheel | its slices, its kind (bronze/silver/gold) | picks the winning slice |
| Zone | its number | calculates its type |
| Inventory | collected rewards | add (win), clear (bomb), keep (leave) |
| Game | current zone, is wheel spinning | calculates "can leave?", runs the flow |

### Design decisions and WHY
- A bomb is not a reward. "Bomb or not" belongs to the Slice, because the slice holds either one.
- RewardType vs Reward: "Gold" (name, icon) is the same everywhere, but the amount
  changes per slice and zone. So they are two different things.
- Rewards don't move themselves. The Inventory adds them (whoever acts owns the method).
- Spinning = 2 separate jobs:
  1. Logic decides the winning slice (instant).
  2. The animation rotates the wheel to land on that slice (just a show).
  Separating logic from visuals is the most important idea in both cases:
  logic can be tested without Unity, and visuals can change without breaking rules.
- Zone type is calculated from the number, not stored:
  - super: zoneNumber % 30 == 0, CHECKED FIRST (30 is also divisible by 5!)
  - safe: zoneNumber % 5 == 0
  - otherwise normal
- "Can leave" is also calculated: (zone is safe or super) AND wheel is not spinning.
  Rule: if I can calculate something, don't store it twice. Two copies get out of sync, which causes bugs.

### ScriptableObjects
- A ScriptableObject is a data card stored as a file in the Project window, not on stage.
- I edit it in the Inspector without code, so designers can add rewards without programming.
- Vertigo requires slice contents to be editable from the editor, so I use ScriptableObjects for that.

