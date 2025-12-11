# Responsibility Game

A 2D game built with Unity that teaches responsibility through engaging story-driven gameplay, featuring interactive dialogue, challenging minigames, and immersive level exploration.

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Game Architecture](#game-architecture)
- [Game Mechanics](#game-mechanics)
- [Technical Stack](#technical-stack)
- [Project Structure](#project-structure)
- [Development Setup](#development-setup)
- [Building the Game](#building-the-game)
- [Contributing](#contributing)

## Overview

Responsibility Game is an educational 2D pixel art adventure that teaches players about responsibility through interactive storytelling and gameplay challenges. Built with Unity 6000.0.58f2, the game combines narrative-driven dialogue powered by the Ink system with engaging minigames that reinforce learning objectives.

Players navigate through various environments, interact with NPCs, make story choices, and complete minigames that test their understanding of responsibility concepts. The game features a cohesive narrative structure with branching dialogue paths and multiple levels.

## Features

### Core Gameplay
- **Interactive Dialogue System**: Branching narratives with character portraits, typing animations, and voice-like audio effects
- **Story-Driven Progression**: Navigate through different environments (home, school, office) with contextual interactions
- **Educational Minigames**: Three distinct minigames that reinforce responsibility concepts
- **Save/Load System**: Platform-specific persistence (WebGL and standalone builds)
- **Responsive Controls**: Modern input system with keyboard and mouse support

### Minigames
1. **Catch Virus**: Reflexes and attention minigame where players catch falling viruses
2. **Multiplication Practice**: Math-based puzzle game teaching multiplication
3. **Pipe Connector**: Logic puzzle requiring players to connect pipes correctly

### Visual & Audio
- Pixel art aesthetic with animated character sprites
- Character-specific audio profiles with pitch variation
- Dynamic camera system powered by Cinemachine
- Smooth scene transitions with fade effects

## Game Architecture

### Technology Stack

The game is built on a robust, maintainable architecture using modern Unity development practices:

- **Unity 6000.0.58f2**: Game engine
- **Universal Render Pipeline (URP)**: 2D rendering pipeline
- **VContainer**: Dependency injection framework for clean architecture
- **Ink Unity Integration**: Narrative dialogue system
- **Unity Input System**: Modern input handling
- **Cinemachine**: Advanced camera control
- **TextMesh Pro**: High-quality text rendering

### Dependency Injection

The project uses **VContainer** for dependency injection with a two-tier lifetime scope architecture:

#### Global Lifetime Scope
Located at `Assets/Scripts/Core/DI/GlobalLifetimeScope.cs`

- Persists across all scenes (DontDestroyOnLoad)
- Registers core game managers as interfaces
- Handles platform-specific registration (WebGL vs Standalone)
- Manages singleton services:
  - `IPlayerManager`: Player spawning and lifecycle
  - `ILevelManager`: Scene transitions and level state
  - `IDialogueManager`: Ink-based dialogue with animations
  - `IMenuManager`: Main menu navigation
  - `IPauseMenuManager`: Pause functionality
  - `IInputManager`: Input event handling
  - `ISaveLoadManager`: Platform-specific save/load

#### Scene Lifetime Scope
Located at `Assets/Scripts/Core/DI/SceneLifetimeScope.cs`

- Scene-specific dependency scope
- Auto-discovers MonoBehaviours marked with `[InjectableMonoBehaviour]`
- Child scope of GlobalLifetimeScope
- Enables clean separation of scene-specific and global concerns

### Event System

Global event bus via static `GameEvents` class provides decoupled communication:

```csharp
GameEvents.Player.OnPlayerSpawned += HandlePlayerSpawn;
GameEvents.Dialogue.OnDialogueStarted += HandleDialogueStart;
GameEvents.Dialogue.OnDialogueEnded += HandleDialogueEnd;
```

Events are organized by domain (Player, Dialogue, etc.) and should be subscribed in `OnEnable` and unsubscribed in `OnDisable` to prevent memory leaks.

### State Management

**GameState** ScriptableObject (`Systems/Game/GameState.cs`) maintains runtime state across scenes:

- `isDialoguePlaying`: Tracks active dialogue sessions
- `playerSpawnLocation`: Remembers spawn points for scene transitions
- `activePlayer`: Reference to the current player GameObject

This enables seamless scene transitions while preserving game context.

## Game Mechanics

### Dialogue System

The dialogue system is powered by **Ink** and provides rich narrative experiences:

#### Features
- **Character-per-character typing animation**: Configurable typing speed
- **Character portraits**: Dynamic sprite changes via animation states
- **Audio feedback**: Per-character audio clips with pitch variation
- **Layout flexibility**: Support for different dialogue box positions
- **Branching narratives**: Player choices affect story progression

#### Ink Tag Support
```ink
speaker:John
portrait:Happy
layout:Bottom
audio:Friendly
Hello! How are you today? #speaker:John #portrait:Happy
```

Supported tags:
- `speaker:Name` - Set character name display
- `portrait:StateName` - Change portrait animation state
- `layout:Position` - Adjust dialogue box layout
- `audio:ID` - Switch audio profile for character voice

#### Dialogue Audio
Each character can have a unique audio profile defined in `DialogueAudioInfoSO`:
- Base audio clip for typing sounds
- Pitch variation for natural feel
- Frame skipping for performance optimization

### Scene Transitions

Scene transitions maintain game state continuity:

1. **LevelTransitionTrigger**: Collision-based triggers for automatic transitions
2. **SceneTransitionTrigger**: Base class with coroutine-based fade transitions
3. **LevelManager**: Coordinates transitions via `OnLevelExit(sceneName, spawnPointName)`

The system preserves `playerSpawnLocation` in GameState, ensuring players spawn at the correct location after transitioning.

### Input System

Built on Unity's new Input System:

- Input actions defined in `Controls.inputactions` asset
- `InputManager` converts callbacks to boolean flags
- Flags are consumed on read (single-frame events)
- Actions: Interact, Submit, Pause, Movement

Example usage:
```csharp
if (_inputManager.GetInteractPressed())
{
    // Handle interaction
}
```

### Minigame Architecture

All minigames follow a consistent pattern:

- **Manager class**: Core game logic and state management
- **Generator class**: Spawns and manages game elements
- **Cell/Element classes**: Individual interactive game objects
- **UI integration**: Score tracking, timers, and completion detection

Minigames are launched via `AppScript` double-click interactions in the overworld.

### Platform Support

The game supports multiple platforms with platform-specific implementations:

#### WebGL
- Uses `WebSaveLoadManager` for browser-compatible save/load
- Optimized for web performance
- Browser storage APIs for persistence

#### Standalone (Windows, Mac, Linux)
- Uses `SaveLoadManager` with file system access
- Full feature support
- Local file persistence

Platform selection is handled automatically at build time via preprocessor directives.

## Technical Stack

### Dependencies

- **VContainer** (1.17.0): Dependency injection framework
- **Unity Input System** (1.14.2): Modern input handling
- **Ink Unity Integration**: Dialogue scripting integration
- **Cinemachine** (2.10.4): Virtual camera system
- **Universal Render Pipeline** (17.0.4): 2D rendering pipeline
- **TextMesh Pro**: Advanced text rendering

### Requirements

- Unity 6000.0.58f2 or higher
- .NET Standard 2.1
- Supported platforms: Windows, macOS, Linux, WebGL

## Project Structure

```
Assets/
├── Scripts/
│   ├── Core/                    # Core systems and abstractions
│   │   ├── Abstractions/        # Interface definitions for managers
│   │   ├── DI/                  # VContainer lifetime scopes
│   │   └── Events/              # Global event system
│   ├── Features/                # Game features and mechanics
│   │   ├── Bootstrap/           # Game initialization
│   │   ├── Camera/              # Camera control
│   │   ├── Characters/          # Player and NPC controllers
│   │   ├── Dialogue/            # Ink-based dialogue system
│   │   ├── Levels/              # Level management
│   │   ├── Menu/                # Menu systems
│   │   └── MiniGames/           # Minigame implementations
│   │       ├── Base/            # Base minigame classes
│   │       ├── CatchVirus/      # Virus-catching game
│   │       ├── Multiplying/     # Math multiplication game
│   │       └── Pipes/           # Pipe-connecting puzzle
│   └── Systems/                 # Core game systems
│       ├── Audio/               # Audio configuration
│       ├── Game/                # GameSettings, GameState, PlayerStats
│       ├── Input/               # Input management
│       └── SaveLoad/            # Save/load managers
├── Data/                        # ScriptableObject assets
│   ├── Config/                  # Game configuration
│   ├── Dialogue/                # Ink scripts and compiled JSON
│   └── Localization/            # Text localization
├── Scenes/                      # Unity scenes
│   ├── Core/                    # Bootstrap and core scenes
│   ├── Levels/                  # Game levels
│   └── MiniGames/               # Minigame scenes
├── Prefabs/                     # Reusable game objects
├── Art/                         # Sprites and animations
└── Audio/                       # Sound effects and music
```

## Development Setup

### Installation

1. Clone the repository:
```bash
git clone <repository-url>
cd responsibility-game
```

2. Open the project in Unity Hub:
   - Click "Add" → Select project folder
   - Ensure Unity 6000.0.58f2 is installed
   - Open the project

3. Let Unity import all assets (first import may take several minutes)

4. Open the Bootstrap scene: `Assets/Scenes/Core/Bootstrap.unity`

### Running the Game

1. Open Bootstrap scene
2. Press Play in Unity Editor
3. The game will initialize core systems and load the main menu

### Adding New Features

#### Creating a New Manager

1. Define interface in `Core/Abstractions/`:
```csharp
public interface IMyManager
{
    void DoSomething();
}
```

2. Implement in appropriate folder:
```csharp
public class MyManager : IMyManager
{
    public void DoSomething() { }
}
```

3. Register in `GlobalLifetimeScope.Configure()`:
```csharp
builder.Register<IMyManager, MyManager>(Lifetime.Singleton);
```

#### Creating Injectable MonoBehaviours

Mark scene-specific MonoBehaviours for automatic injection:

```csharp
[InjectableMonoBehaviour]
public class MyComponent : MonoBehaviour
{
    [Inject] private IDialogueManager _dialogueManager;

    private void Start()
    {
        // _dialogueManager is automatically injected
    }
}
```

For runtime-added components, extend `InjectableDynamicMonoBehaviour`.

#### Adding Dialogue

1. Create `.ink` file in `Assets/Data/Dialogue/`
2. Write dialogue with Ink syntax
3. Add `DialogueTrigger` component to GameObject
4. Assign compiled TextAsset (Ink JSON) to trigger
5. Configure trigger type (OnInteract, OnCollision, etc.)

### Testing

Tests are run through Unity Test Runner:

1. Open Test Runner: `Window → General → Test Runner`
2. Select PlayMode or EditMode
3. Click "Run All" to execute tests

## Building the Game

### Build Process

1. Open Build Settings: `File → Build Settings`
2. Select target platform (Windows, macOS, Linux, WebGL)
3. Click "Build" or "Build and Run"

### Platform-Specific Builds

#### WebGL Build
- Automatic use of `WebSaveLoadManager`
- Optimized for browser performance
- Test in supported browsers (Chrome, Firefox, Edge)

#### Standalone Build
- Uses standard `SaveLoadManager`
- Full feature support
- Platform-specific optimizations applied automatically

### Build Optimization

- Enable IL2CPP for better performance (non-WebGL)
- Use Medium or High compression for WebGL
- Strip unused engine code in Player Settings
- Optimize sprite atlases and texture compression

## Contributing

### Development Guidelines

1. **Follow the architecture**: Use dependency injection, implement interfaces
2. **Event-driven communication**: Use `GameEvents` for decoupled systems
3. **Injectable components**: Mark MonoBehaviours with `[InjectableMonoBehaviour]`
4. **No FindObjectOfType**: Always inject dependencies
5. **Memory management**: Subscribe/unsubscribe events properly
6. **Platform awareness**: Test platform-specific code on target platforms

### Code Style

- Use C# naming conventions
- Keep classes focused (Single Responsibility Principle)
- Document public APIs with XML comments
- Use meaningful variable names
- Prefer composition over inheritance

### Commit Guidelines

- Write clear, descriptive commit messages
- Use conventional commits format:
  - `feat:` New features
  - `fix:` Bug fixes
  - `chore:` Maintenance tasks
  - `docs:` Documentation updates
