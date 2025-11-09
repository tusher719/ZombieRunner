# 🧟 Dead Protocol - Zombie Survival Game

<div align="center">

![Unity](https://img.shields.io/badge/Unity-2021.3+-black?style=for-the-badge&logo=unity)
![Platform](https://img.shields.io/badge/Platform-PC%20%7C%20Android-green?style=for-the-badge)
![Version](https://img.shields.io/badge/Version-v2.4.1.2-blue?style=for-the-badge)
![Status](https://img.shields.io/badge/Status-In%20Development-yellow?style=for-the-badge)

A hyper-casual first-person zombie survival game built with Unity, featuring intuitive controls for both PC and mobile platforms.

[Features](#-features) • [Installation](#-installation) • [Controls](#-controls) • [Development](#-development)

</div>

---

## 📋 Table of Contents

- [About](#-about)
- [Features](#-features)
- [Tech Stack](#-tech-stack)
- [Installation](#-installation)
- [Controls](#-controls)
- [Game Mechanics](#-game-mechanics)
- [Project Structure](#-project-structure)
- [Development Setup](#-development-setup)
- [Roadmap](#-roadmap)
- [Version History](#-version-history)
- [License](#-license)

---

## 🎮 About

**Dead Protocol** is a hyper-casual FPS zombie survival game designed for quick, intense gameplay sessions. Battle through waves of zombies with an arsenal of weapons across multiple levels. Built with Unity and optimized for both PC and mobile platforms.

**Project Type:** University Pre-Defence Project  
**Development Time:** 1 month  
**Team Role:** Programming & Level Design  
**Unity Version:** 2021.3 LTS (or higher recommended)

---

## ✨ Features

### Core Gameplay
- ✅ **First-Person Shooter Mechanics** - Smooth camera controls and weapon handling
- ✅ **3 Unique Weapons** - Assault Rifle, Shotgun, and SMG with individual characteristics
- ✅ **Zombie AI System** - NavMesh-based pathfinding with chase and attack behaviors
- ✅ **Health System** - Dynamic health bar with visual feedback
- ✅ **Score Tracking** - Point-based system with kill counting

### Platform Support
- ✅ **PC Controls** - Keyboard (WASD) + Mouse for movement and aiming
- ✅ **Mobile Controls** - Touch joystick + gesture-based camera control
- ✅ **Multi-touch Support** - Simultaneous movement and aiming on mobile
- ✅ **Responsive UI** - Adapts to different screen sizes and aspect ratios

### UI/UX
- ✅ **Splash Screen** - Game intro with touch/click to continue
- ✅ **Main Menu** - Clean interface with settings panel
- ✅ **In-Game HUD** - Health bar, ammo counter, kill tracker
- ✅ **Pause System** - Resume or return to main menu
- ✅ **Game Over Screen** - Restart or exit options

### Additional Systems
- ✅ **Weapon Switching** - Quick weapon selection (PC: Mouse wheel, Mobile: UI button)
- ✅ **Zoom System** - Scope zoom for compatible weapons
- ✅ **Level Progression** - Door-based level transitions after completing objectives
- ✅ **Auto Zoom-Out** - Smart zoom reset when switching weapons

---

## 🛠 Tech Stack

| Technology | Purpose |
|-----------|---------|
| **Unity 2021.3 LTS** | Game Engine |
| **C#** | Primary Programming Language |
| **NavMesh** | Enemy AI Pathfinding |
| **Unity UI** | User Interface System |
| **TextMeshPro** | Advanced Text Rendering |
| **Standard Assets** | First-Person Controller |
| **Unity Input System** | Cross-platform Input Handling |

---

## 💾 Installation

### Prerequisites
- **Unity Hub** (Latest Version)
- **Unity 2021.3 LTS** or newer
- **Git** for version control

### Clone the Repository
```bash
# Clone the repository
git clone https://github.com/tusher719/ZombieRunner.git

# Navigate to project directory
cd ZombieRunner
```

### Open in Unity

1. Open **Unity Hub**
2. Click **"Add"** → **"Add project from disk"**
3. Select the cloned `ZombieRunner` folder
4. Unity will import and configure the project automatically
5. Wait for asset import to complete (may take 5-10 minutes on first load)

### Build Settings

#### For PC (Windows):
1. **File → Build Settings**
2. Platform: **PC, Mac & Linux Standalone**
3. Target Platform: **Windows**
4. Architecture: **x86_64**
5. Click **Build** or **Build and Run**

#### For Android:
1. **File → Build Settings**
2. Platform: **Android**
3. **Switch Platform** (if not already selected)
4. **Player Settings:**
   - Minimum API Level: **Android 7.0 (API 24)**
   - Target API Level: **Android 13 (API 33)**
   - Scripting Backend: **IL2CPP**
   - Target Architectures: **ARM64** ✓
5. Click **Build** or **Build and Run**

---

## 🎮 Controls

### PC Controls

| Action | Key/Button |
|--------|-----------|
| **Move Forward** | W |
| **Move Backward** | S |
| **Move Left** | A |
| **Move Right** | D |
| **Look Around** | Mouse Movement |
| **Shoot** | Left Mouse Button |
| **Zoom** | Right Mouse Button |
| **Switch Weapon** | Mouse Wheel / Number Keys (1-3) |
| **Jump** | Space (Planned) |
| **Pause** | ESC |

### Mobile Controls

| Action | Control |
|--------|---------|
| **Move** | Left Joystick |
| **Look Around** | Swipe on Right Screen |
| **Shoot** | Tap Shoot Button |
| **Zoom** | Tap Zoom Button |
| **Switch Weapon** | Tap Switch Button |
| **Jump** | Tap Jump Button (Planned) |
| **Pause** | Tap Pause Button |

---

## 🎯 Game Mechanics

### Objective
Eliminate 10 zombies per level to unlock the exit door and progress to the next area.

### Weapons

| Weapon | Damage | Fire Rate | Ammo Type | Zoom |
|--------|--------|-----------|-----------|------|
| **Assault Rifle** | 30 | Medium | Bullets | ❌ |
| **Shotgun** | 30 | Slow | Shells | ✅ |
| **SMG** | 30 | Fast | Bullets | ✅ |

### Zombie AI Behavior
- **Idle State:** Patrols or stands guard
- **Detection:** Spots player within 5-unit radius
- **Chase State:** Pursues player using NavMesh pathfinding
- **Attack State:** Deals damage when in melee range
- **Damage Response:** Becomes aggressive when shot

### Scoring System
- **10 points** per zombie eliminated
- **Total score** tracked across all levels
- **Kill counter** displays progress (e.g., 5/10)

---

## 📁 Project Structure
```
ZombieRunner/
├── Assets/
│   ├── Scenes/
│   │   ├── Splash.unity          # Game intro screen
│   │   ├── Main.unity            # Main menu
│   │   └── Sandbox.unity         # Gameplay scene (Level 1)
│   ├── Scripts/
│   │   ├── Player/
│   │   │   ├── PlayerMovement.cs
│   │   │   ├── PlayerHealth.cs
│   │   │   └── MobileCameraLook.cs
│   │   ├── Weapons/
│   │   │   ├── Weapon.cs
│   │   │   ├── WeaponeSwitcher.cs
│   │   │   ├── WeaponZoom.cs
│   │   │   └── MobileWeaponZoom.cs
│   │   ├── Enemy/
│   │   │   ├── EnemyAI.cs
│   │   │   ├── EnemyHealth.cs
│   │   │   └── EnemyAttack.cs
│   │   ├── Managers/
│   │   │   ├── GameManager.cs
│   │   │   ├── SplashManager.cs
│   │   │   ├── MainMenuManager.cs
│   │   │   └── DeathHandler.cs
│   │   ├── Mobile/
│   │   │   ├── MobileJumpButton.cs
│   │   │   ├── MobileShootButton.cs
│   │   │   └── MobileWeaponSwitcher.cs
│   │   └── UI/
│   │       ├── HealthDisplay.cs
│   │       └── BlinkText.cs
│   ├── Prefabs/
│   │   ├── Player.prefab
│   │   ├── Zombie.prefab
│   │   └── Weapons/
│   └── Materials/
├── ProjectSettings/
└── Packages/
```

---

## 🔧 Development Setup

### Required Packages
- **TextMesh Pro** (Essential)
- **Standard Assets** (First Person Controller)
- **Input System** (Optional, for new input handling)

### Import TextMesh Pro
1. **Window → TextMeshPro → Import TMP Essential Resources**
2. Click **Import** in the popup window

### Scene Setup Order
1. Open **Splash** scene first
2. Add **Main** and **Sandbox** scenes to Build Settings
3. Ensure scene order: `0. Splash → 1. Main → 2. Sandbox`

### Testing in Editor
- Use **Free Aspect** or **16:9** resolution for testing
- Test mobile controls with Unity Remote (optional)
- Use **Stats** window to monitor performance

---

## 🗺 Roadmap

### Current Version: v2.4.1.2

#### ✅ Completed
- [x] Core FPS mechanics
- [x] 3 functional weapons
- [x] Zombie AI with NavMesh
- [x] Health system with UI
- [x] PC and Mobile controls
- [x] Multi-touch support (both controllers work simultaneously)
- [x] Scene flow (Splash → Main → Gameplay)
- [x] Score and kill tracking
- [x] Weapon switching
- [x] Zoom system (platform-specific, no auto-zoom bug)
- [x] Game over and restart functionality
- [x] Main menu with settings panel
- [x] Responsive UI for multiple screen sizes

#### 🚧 In Progress
- [ ] Map/Level Selector scene
- [ ] Level unlock system
- [ ] 5 complete levels

#### 📋 Planned (v2.5.0+)
- [ ] Jump button implementation
- [ ] Audio system (music + SFX)
- [ ] Settings functionality (audio toggles)
- [ ] Wave-based spawning
- [ ] Additional zombie types
- [ ] Health/Ammo pickups
- [ ] Boss zombie
- [ ] Polish and visual effects

#### 💡 Future Considerations
- [ ] Multiplayer mode
- [ ] Achievement system
- [ ] Leaderboards
- [ ] More weapons and maps

---

## 📊 Version History

### v2.4.1.2 (Current - 2024)
**Added:**
- Splash screen with auto-load and manual skip
- Main menu UI with settings panel
- Settings panel show/hide functionality

**Fixed:**
- Auto-zoom bug on Android completely resolved
- Both controllers (joystick + camera) work simultaneously without conflicts
- Platform-specific zoom controls (PC: Right-click, Mobile: Button only)
- Multi-touch handling improved

**Improved:**
- Camera control code optimized (simpler, more responsive)
- UI scaling for various Android screen sizes
- Code cleanup and organization

### v2.4.0.3 (2024)
**Fixed:**
- Initial auto-zoom issue investigation
- Platform-specific zoom controls first implementation

### v2.4.0.2 (2024)
**Fixed:**
- UI scaling issues on mobile devices
- Canvas scaler optimization for multiple aspect ratios

### v2.4.0.1 (2024)
**Added:**
- GameManager foundation
- Kill tracking and score system
- Level progression door system

### v2.3.2.5 (2024)
**Improved:**
- Complete mobile controls
- Health display system
- Clean code (removed debug logs)

### v2.3.2.0 (2024)
**Added:**
- Health bar with dynamic scaling
- Mobile UI buttons (shoot, zoom, weapon switch)

## Version 2.4.2.1 - Enemy Health System Implementation

### Added Features
- **Enemy Health Bar System**: Implemented visual health indicator for enemies
  - Dynamic health bar that displays current enemy health status
  - Smooth fill animation using `Mathf.MoveTowards` for gradual health reduction
  - Billboard effect to keep healthbar always facing the camera
  
- **Health Bar UI Components**:
  - Background and Foreground image-based health display
  - Canvas-based rendering with proper layering
  - Configurable reduction speed for health bar animation

### Fixed
- **Health Value Validation**: 
  - Prevented negative health values using `Mathf.Max(hitPoints, 0)`
  - Health bar clamped between 0-1 range using `Mathf.Clamp01()`
  - Ensured accurate health percentage calculation
  
- **Enemy Death State Handling**:
  - Health bar automatically hides when enemy health reaches zero
  - Canvas disabled upon enemy death for cleaner visuals
  - Proper integration with existing death animation system

### Technical Improvements
- Auto-assignment of health bar sprite if not manually configured
- Error handling for missing UI components
- Professional code documentation with clear inline comments
- Proper component reference management in Start() method

### Scripts Modified
- `EnemyHealth.cs`: Added health bar integration and validation
- `EnemyHealthbar.cs`: Complete implementation with billboard and hide functionality

---

---

## 🤝 Contributing

This is a university project currently not accepting external contributions. However, feedback and suggestions are welcome!

---

## 👨‍💻 Developer

**Tusher**
- Role: Programmer & Level Designer
- GitHub: [@tusher719](https://github.com/tusher719)
- Project Duration: 1 month
- Focus: Cross-platform game development (PC + Android)

---

## 📄 License

This project is developed for educational purposes as part of a university pre-defence project.

---

## 🙏 Acknowledgments

- Unity Technologies for the game engine
- Standard Assets for the First Person Controller
- TextMesh Pro for advanced text rendering
- Unity community for tutorials and support

---

## 📞 Support

For issues or questions:
- Open an issue on GitHub
- Contact: hasan40-074@diu.edu.bd

---

<div align="center">

**Made with ❤️ for University Pre-Defence Project**

⭐ Star this repo if you find it useful!

</div>
