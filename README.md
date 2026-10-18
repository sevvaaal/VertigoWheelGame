# Vertigo Wheel: Game Developer Demo

A wheel of fortune risk game inspired by Critical Strike's card game, made with **Unity 6000.3.23f1**, C#, DOTween and TextMeshPro. Landscape, for Android.

**APK:** see [Releases](https://github.com/sevvaaal/VertigoWheelGame/releases) · **Video:** [`Media/vertigo_gameplay.mp4`](Media/vertigo_gameplay.mp4) (recorded from the Android build)

| 20:9 (Android build) | 16:9 | 4:3 |
|---|---|---|
| ![20:9](Media/screenshot_20x9.png) | ![16:9](Media/screenshot_16x9.png) | ![4:3](Media/screenshot_4x3.png) |

## How it plays
- Every zone you spin a wheel with 8 slices: rewards and, on normal zones, one **bomb**.
- A reward is added to the collected rewards panel and you move to the next zone. Rewards **grow every zone** (+10% per zone, set in `ZoneConfig`).
- **Every 5th zone** is a **safe zone**: a silver wheel without a bomb. **Every 30th zone** is a **super zone**: a golden wheel without a bomb and with special rewards.
- **Bomb:** everything collected is lost, and the run restarts from zone 1.
- **Leave:** walk away with everything collected so far, whenever the wheel is not spinning.

## How to run
- **Editor:** open `Assets/_Project/Scenes/GameScene` and press Play. The game is designed for landscape 1920x1080 and scales to any landscape ratio.
- **Android:** install the APK from the Release (ARM64, Android 7.1+).
- **Tests:** Window > General > Test Runner > EditMode > Run All.
- **Testing tip:** `GameController > Start Zone` in the Inspector starts a run at any zone (for example 29, to reach the super zone in one spin).

## Project structure
```
Assets/_Project/
├── Art/            UI sprites (from the demo content) and UIAtlas (Sprite Atlas V2)
├── Data/           ScriptableObject assets: reward types, Bronze/Silver/Golden wheels, ZoneConfig
├── Prefabs/        ui_slice, ui_reward_row, ui_image_reward_fly
│   └── UI/         ui_popup_bomb, ui_popup_collect, ui_panel_zone_bar, ui_panel_rewards
├── Scenes/         GameScene
├── Scripts/        Vertigo.Runtime assembly
│   ├── Data/       RewardTypeSO, Reward, SliceData, WheelSO, ZoneConfigSO
│   ├── Gameplay/   GameController, Inventory, ZoneRules, ZoneType
│   └── UI/         WheelSpinner, WheelView, SliceView, ZoneBarView, RewardListView, RewardRowView,
│                   RewardFlyEffect, PopupView, BombPopup, CollectPopup, ButtonView, UIChildFinder
└── Tests/EditMode/ Vertigo.Tests.EditMode assembly: ZoneRulesTests, InventoryTests
```

## Architecture
- **Data (ScriptableObjects).** Every wheel's slices, the reward types and the zone settings are assets, editable in the Inspector without code. `ZoneConfig` decides which wheel each zone type uses and how zones are spaced.
- **Rules (plain C#).** `ZoneRules` (zone type, reward scaling, next safe or super zone, when leaving is allowed) and `Inventory` (collected rewards) have no Unity scene dependencies, so they are unit tested.
- **Views (MonoBehaviours).** `WheelView`, `ZoneBarView`, `RewardListView`, the popups and the buttons only show state and report input. They never decide game rules.
- **Referee.** `GameController` connects everything: it asks the spinner to spin, decides what a result means, plays the result sequence and controls when Spin and Leave are available.

The wheel **decides the winning slice first**, then DOTween rotates the wheel so that slice stops under the pointer.

### SOLID and patterns
- **Single responsibility:** spinning (`WheelSpinner`), drawing the wheel (`WheelView`), rules (`ZoneRules`), totals (`Inventory`) and flow (`GameController`) are separate classes.
- **Open/closed:** a new wheel, reward or zone spacing is a new or edited asset, not a code change. A new popup inherits `PopupView`.
- **Observer:** C# events (`SpinStarted`, `SpinCompleted`, `AmountChanged`, `Cleared`, `Clicked`, `RestartClicked`, `PlayAgainClicked`). The rewards panel updates itself from the inventory's events.
- **Inheritance:** `PopupView` holds the shared show, hide and pop-in animation. `BombPopup` and `CollectPopup` only add their own content.
- **Object reuse:** the zone bar reuses the same 9 cells and slides them, instead of creating new objects every zone.

## Checklist
| Requirement | How it is handled |
|---|---|
| Slice contents changeable from the editor | `WheelSO` assets (Bronze, Silver, Golden) list their slices; `ZoneConfig` picks the wheel per zone type |
| Canvas mode Expand | Canvas Scaler: Scale With Screen Size, 1920x1080, Expand. Anchors keep each panel on its edge |
| 20:9, 16:9, 4:3 | Screenshots above. The 20:9 one comes from the Android build |
| TextMeshPro | All texts |
| Naming general → specific, `_value` for changing elements | e.g. `ui_image_wheel`, `ui_text_next_safe_zone_value`, `ui_text_reward_amount_value` |
| No Raycast Target / Maskable on unnecessary images | Raycast only on button graphics and popup blockers; Maskable only inside the zone bar's `RectMask2D` |
| UI animators not on the root | Animations run on dedicated children: the popup panels (not the popup roots), the zone strip, the glow and flash images, the wheel inside `ui_wheel_group` |
| Button references set from OnValidate | `ButtonView`, `BombPopup` and `CollectPopup` find their buttons in `OnValidate` (`GetComponent` / `UIChildFinder` by name) |
| No OnClick from the editor | Every click is connected in code with `AddListener` |
| Sliced sprites, no stretched images | Buttons and panels use sliced sprites; other images use Preserve Aspect |
| ScriptableObjects, DOTween, Sprite Atlas | Data assets above; DOTween for the spin, pop-ins, zone slide, flying icons, count-up, shake and glow; `UIAtlas` packs all UI art |
| Prefab and folder structure | Reusable UI prefabs, everything under `Assets/_Project`, code in Data / Gameplay / UI folders, with separate runtime and test assemblies |
| Testable codebase | 17 EditMode test runs for `ZoneRules` and `Inventory` |

## Rule interpretations
- **Leaving.** The case says *"Player can choose to leave when wheel is not spinning and when the zone is safe or at the super zone"* and also *"Before spinning the wheel, the player has a chance to walk away and collect all the rewards they received until that point"*. I chose the second reading because that is how the wheel works in Critical Strike itself: Leave is available on any zone, but never while the wheel is spinning or a result is still playing. Changing it back is one line in `ZoneRules.CanLeave`.

## Android build notes
- IL2CPP, ARM64, landscape only (both landscape sides).
- Graphics APIs: **OpenGL ES 3 first, Vulkan second.** The Android emulator on macOS showed a blank screen with Vulkan; OpenGL ES 3 works on the emulator and on every phone.

## Known limitations and next steps
- **Event unsubscription:** subscriptions are never removed in `OnDestroy`. It's harmless with a single scene, but needed if scenes start reloading.
- **Pooling:** wheel slices, reward rows and flying icons are created and destroyed; pooling them would avoid garbage on low-end phones.
- **Deterministic tests:** the winning slice uses `UnityEngine.Random` directly; injecting a random source would allow tests for the spin result.
- **Bonus:** continue-with-currency after a bomb.
