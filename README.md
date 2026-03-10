# Project Architecture & Onboarding Documentation

---

### Main Systems & Communication

- **Player System**: `PlayerController` is the central hub. It receives input via the new Input System, manages movement (`PlayerMovement`), health (`HealthModule`), stats (`PlayerStats`), inventory (`PlayerInventory`), and animations (`PlayerAnimations`). It also holds a reference to `PlayerInteraction` for handling interactable objects.
- **Enemy System**: `MeleeEnemy` controls enemy behavior, using `EnemyMovement` for movement, `HealthModule` for health, and `MeleeEnemyAnimations` for animation. It subscribes to its own health events to trigger death and damage reactions.
- **Room System**: `RoomController` detects player entry/exit via `PlayerTrigger2D`. Each room type (combat, safe, portal) inherits from `RoomBehaviour` and implements specific logic (spawning enemies, healing, etc.). Rooms communicate with doors and spawners.
- **Interaction System**: Objects implementing `IInteractable` (Chest, Engine, LockedDoor) define interaction logic. `PlayerInteraction` detects the nearest interactable, shows a prompt via `InteractablePrompt`, and invokes `Interact()` when the player presses the interaction key.
- **Item & Stats System**: Items are `ScriptableObject` definitions (`BaseItem`). Stat items apply `StatModifier`s to `PlayerStats`. Quest items are tracked in `PlayerInventory` and can be consumed. The `IStatProvider` interface allows any object (player, enemy) to provide stats; `PlayerStats` and `EnemyStats` implement it.
- **UI System**: `TooltipUI` is a singleton for displaying item tooltips on hover. HUD item controllers (`HudQuestItemsController`, `HudUpgradeItemsController`) listen to inventory updates and refresh the display. Health bars (`PlayerHPBar`, `EnemyHPBar`) listen to health events.

### Main Game Flow

1. **Game Start**: `MainMenuController` loads the dungeon scene (`DungeonPrototype`).
2. **Scene Load**: `PlayerController` is instantiated (placed in scene). It sets up input, finds the main camera, and assigns follow target.
3. **Room Entry**: `RoomController` detects player via `PlayerTrigger2D` and invokes `OnPlayerEnterRoom`. The active `RoomBehaviour` starts its logic.
4. **Combat**: Player shoots projectiles (`Projectile`) that damage enemies. Enemies attack the player. Health changes trigger UI updates and death events.
5. **Interaction**: When near an interactable, `PlayerInteraction` shows a prompt; pressing 'E' calls `Interact()`. Chests grant items; doors unlock; engines accept fuses.
6. **Room Completion**: After all enemies are defeated, doors open. If it's a portal room, destroying the portal stops spawns and opens doors.
7. **Progression**: Player collects items (stat upgrades, quest items) that affect stats. Quest items are used to unlock doors/engines.
8. **Game End**: Yet there is no win/lose condition.

### Plugins & Packages Used

For now there is no package added, but the one that come with Unity.

---

## Technical Onboarding

### How to Compile and Use the Project

- Open the project in Unity (version 6000.3.8f1).
- Open the scene `Assets/Scenes/MainMenu.unity` and press Play.

### Where the Game Begins

The entry point is `MainMenuController` in the MainMenu scene. It loads the gameplay scene (`DungeonPrototype`) on start button click. The gameplay scene contains the player prefab, rooms, and initial setup.

### Implementing a New Feature

- **New Enemy Type**: Create a new prefab with components like `HealthModule`, `EnemyMovement`, and a custom behavior script. Implement its attack logic. Add to spawn points in rooms.
- **New Item**: Create a new `ScriptableObject` (right-click > Items > Stat Item or Quest Item). Set name, description, icon, and modifiers. Add to chest loot lists.
- **New Room Behaviour**: Derive from `RoomBehaviour`, implement `SubscribeToEvents` and `UnsubscribeFromEvents`. Attach to a room GameObject with a `RoomController`. Use `UnityEvent` hooks if needed.

### Running Tests

Currently, there are no automated tests. To manually test:
- Play the game and verify functionality.
- Use Unity's Test Framework to add unit tests (e.g., for stat calculations, inventory logic) if needed in the future.

### How to Build

- Open **File > Build Settings**.
- Add the scenes (MainMenu, DungeonPrototype) to the build list.
- Select target platform and click **Build**.

---

## Practical Workflows

### For Designers / Artists

#### Creating a New Enemy

1. **Prefab**: Duplicate an existing enemy prefab (e.g., `MeleeEnemy`). Place in `Assets/Prefabs/Enemies/`.
2. **Components**: Adjust `MeleeEnemy` parameters (attack range, damage, cooldown). Replace sprite and animations via the `Animator`.
3. **Add to Spawners**: In a room (e.g., `CombatRoomBehaviour`), drag the new enemy prefab into the `enemyPrefab` field of the spawn points.

#### Where to Place Models / Sprites

- Sprites: `Assets/Sprites/`
- Animations: `Assets/Animations/`
- Prefabs: `Assets/Prefabs/`

#### Git Branch Structure

- `main`: stable, production-ready code.
- `development`: integration branch for features.
- Commits should be atomic and use descriptive messages.

#### Creating a New Arena (Room)

1. **Layout**: Build the room in the scene using tilemaps or sprites.
2. **Add RoomController**: Attach `RoomController` and assign a `PlayerTrigger2D` (child trigger collider).
3. **Add RoomBehaviour**: Choose `CombatRoomBehaviour`, `SafeRoomBehaviour`, or create a custom one. Configure spawn points, doors, and events.
4. **Connect Doors**: Assign door GameObjects to the behaviour; they will be opened/closed automatically.

### Balancing (Data-Driven)

- **Enemy Stats**: Exposed in `EnemyStats` component (MaxHealth). Attack values are in `MeleeEnemy` (attackDamage, knockback).
- **Player Stats**: Base values in `PlayerStats` component (startMaxHealth, startMoveSpeed, etc.). Items apply modifiers via `StatModifierData` (ScriptableObject).
- **Item Definitions**: All items are ScriptableObjects; designers can tweak name, description, icon, and stat modifiers without touching code.
- **Room Parameters**: Spawn rates, heal per second, etc., are public fields in room behaviours.

---

## Coding and Naming Conventions

- **Classes**: `PascalCase` (e.g., `PlayerController`).
- **Public fields**: `camelCase` (e.g., `followLerp`, `attackRange`).
- **Private fields**: `camelCase`.
- **Properties**: `PascalCase`.
- **Methods**: `PascalCase` - For listener methods, use the prefix `On` before the name of the event e.g `OnRoomEnter` - 
- **Interfaces**: `I` prefix (e.g., `IInteractable`).
- **Enums**: `PascalCase` (e.g., `StatType`, `ModifierType`).
- **Events**: always public, using the class `Action`, `PascalCase` e.g `RoomEnter` (not `OnRoomEnter`)
- **UnityEvents**: `PascalCase` always private with the `SerializeField` attribute (e.g `OnRoomEnter`)

### Project Organization

- Scripts are placed in `Assets/Scripts/` with subfolders by feature:
  - `Player/`
  - `Enemy/`
  - `Room/`
  - `UI/`
  - `Items/`
  - `Interaction/`
  - `Stats/`
  - `Utilities/`

All scripts should be inside their namespaces that follow the folder structure (e.g `namespace AncientDescent.Player`)

### Design Patterns Used

- **Observer**: Events (`Damaged`, `Died`, inventory update actions) decouple systems (e.g., UI listens to health changes).
- **Component**: Separation of concerns (movement, health, stats as separate components).
- **Strategy**: Different room behaviours encapsulate room logic.
- **Interface**: `IInteractable` allows any object to be interacted with.
- **ScriptableObject**: Used for item definitions to enable data-driven design.

### ScriptableObject Usage

- `BaseItem` and its derivatives (`StatItemDefinition`, `QuestItemDefinition`) are ScriptableObjects. They define immutable item properties.
