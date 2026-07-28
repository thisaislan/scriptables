
<br>
<br>

<p align="center">
  <a href="https://github.com/thisaislan/scriptables">
    <img src="https://github.com/thisaislan/just-images/raw/main/images/scriptables/scriptables_logo.png">
  </a>
</p>

<h1 style="font-size: 50px" align ="center">
  Scriptables  
  <h4  align ="center">
    Extend ScriptableObjects with reactive events, runtime reference keeping, editor debugging tools, and clean data separation between edit-time and play-mode. Ships with an inspector panel and a creation wizard.
    <br>
  </h4>
</h1>

<p align="center">
    <a href="https://unity3d.com/get-unity/download">
        <img src="https://img.shields.io/badge/unity-tools-blue" alt="Unity Download Link"></a>
    <a href="https://github.com/thisaislan/scriptables/blob/main/LICENSE.md">
        <img src="https://img.shields.io/badge/License-MIT-brightgreen.svg" alt="License MIT"></a>
    <a href="https://chat.deepseek.com">
        <img src="https://img.shields.io/badge/%F0%9F%92%AC-DeepSeek%20AI-blue" alt="License MIT"></a>
</p>

<h1></h1>

</br>

### Table of Contents
- [Features](#-features)
- [Overview](#-overview)
- [Scriptables Panel](#-scriptables-panel)
- [Quick Start](#-quick-start)
  - [Wizard](#wizard)
  - [Settings](#custom-scriptable-settings)
  - [Runtime](#custom-scriptable-runtime)
  - [Reactive](#custom-scriptable-reactive)
  - [No-Parameter Reactive](#custom-no-parameter-reactive)
  - [Pre-made Types](#pre-made-types)
- [Pin Feature](#-pin-feature)
- [Interfaces](#-interfaces)
- [Install](#-install)
- [Support](#-support)
- [Thanks](#-thanks)
- [License](#-license)

</br>

## ✨ Features

- **Creation Wizard** — step-by-step UI to create new scriptable assets with data type configuration and auto-generated script files
- **Scriptables Panel** — centralised window to browse, search, filter, and inspect every scriptable asset in the project
- **Reactive Events** — observable data containers that notify subscribers when values change; emit with zero allocation
- **Pin System** — keep ScriptableObjects alive across domain reloads with `IPinnable` / `ReferenceKeeper`
- **Subscriber Tracking** — every reactive inspector includes a live observer list showing all current subscribers; no more guessing who is listening to your events
- **Description Field** — rich text descriptions on every asset, visible in both panel and inspector
- **Dual Data System** — separate editor-configured data from runtime data with automatic reset on play mode transitions
- **Runtime ScriptableObjects** — volatile data that resets on every play session; ideal for shared game state
- **Settings ScriptableObjects** — persistent data that survives play mode; ideal for configuration and constants
- **Standard Interfaces** — `ISubscribable<T>`, `ISettable<T>`, `IEmitable`, `IResettable`, `IInitializable<T>`, `IPinnable` — code against contracts, not concrete types
- **Custom Inspectors** — per-type editors with real-time data visualisation, reset buttons, and printer utilities
- **Type-Safe Generics** — no more `Data` base class constraint; freely generic on any serializable type
- **Zero per-frame allocations** — all editor UI uses cached `GUIContent`/`GUIStyle` statics

</br>

## 🏗️ Overview

The package organises ScriptableObjects into four specialised categories:

### ⚙️ Scriptable Settings
- **Persistent data** configured at edit time, available at runtime
- Survives play mode — changes in editor are serialized
- Can be updated via remote config, repositories, or any external means
- Ideal for game configuration, constants, balance parameters

<br>

### 🔄 Scriptable Runtime
- **Volatile data** shared across systems during a session
- Automatically resets when exiting play mode
- No persistence — every run starts fresh
- Perfect for game state, temporary variables, inter-system communication

<br>


### 🔥 Scriptable Reactive
- **Observable data** that notifies listeners automatically on change
- Play-mode data is isolated from edit-time defaults
- Event-driven architecture for decoupled communication
- Also available as **NoParamsReactive** (signal-only, no payload)

</br>

## 📋 Scriptables Panel


<div align="center" style="text-align:center;">
  <img src="https://github.com/thisaislan/just-images/raw/main/images/scriptables/panel.png"  width="800" > 
  <br>
</div>

Open via **Tools > Scriptables > Panel**. The panel provides:

- **Tab navigation**: Scriptables, Settings, Runtime, Reactive, ScriptableObject
- **Search / filter**: real-time filtering by name, type, or category
- **Pagination**: browse large collections efficiently
- **Details card**: inspect description, type, path, and metadata of the selected asset
- **Quick actions**: Open asset, Ping in Project window

</br>

## 🚀 Quick Start

### Wizard

The fastest way to create a new scriptable asset:

1. Open **Tools > Scriptables > Wizard** or **Create > Scriptables > Wizard**
2. Choose a category (Settings, Runtime, Reactive, No-Parameter Reactive)
3. Configure the data type and asset name
4. Click **Create** — the asset and script file are generated automatically

> New script files can also be generated directly inside the wizard by selecting the "New" option.

<div align="center" style="text-align:center;">
  <img src="https://github.com/thisaislan/just-images/raw/main/images/scriptables/wizard.png"  width="800" > 
  <br>
</div>

<br>

### Custom Scriptable Settings

```csharp
[CreateAssetMenu(fileName = nameof(GameConfigSettings), menuName = "MyGame/Settings/" + nameof(GameConfigSettings))]
public class GameConfigSettings : ScriptableSettings<GameConfigSettings.GameConfigData>
{
    [Serializable]
    public class GameConfigData
    {
        public float MusicVolume = 0.8f;
        public int MaxPlayers = 4;
    }
}
```
<div align="center" style="text-align:center;">
  <img src="https://github.com/thisaislan/just-images/raw/main/images/scriptables/settings.png"  width="500" > 
  <br>
</div>
<br>

```csharp
public class SettingsUser : MonoBehaviour
{
    [SerializeField]
    private GameConfigSettings gameConfigSettings;

    private void Start()
    {
        // Initialise with runtime values
        gameConfigSettings.Initialize(new GameConfigSettings.GameConfigData
        {
            MusicVolume = 0.5f,
            MaxPlayers = 2
        });
    }
}
```

<br>

### Custom Scriptable Runtime

```csharp
[CreateAssetMenu(fileName = nameof(PlayerStateRuntime), menuName = "MyGame/Runtime/" + nameof(PlayerStateRuntime))]
public class PlayerStateRuntime : ScriptableRuntime<PlayerStateRuntime.PlayerStateData>
{
    [Serializable]
    public class PlayerStateData
    {
        public int Health = 100;
        public Vector3 Position;
    }
}
```
<div align="center" style="text-align:center;">
  <img src="https://github.com/thisaislan/just-images/raw/main/images/scriptables/runtime.png"  width="500" > 
  <br>
</div>
<br>

```csharp
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private PlayerStateRuntime playerStateRuntime;

    private void Update()
    {
        PlayerStateRuntime.PlayerStateData playerStateData = playerStateRuntime.Data;
        playerStateData.Position = transform.position;
        playerStateRuntime.Set(playerStateData); // overwrites runtime data
    }
}
```

</br>

### Custom Scriptable Reactive

```csharp
[CreateAssetMenu(fileName = nameof(ScoreReactive), menuName = "MyGame/Reactives/" + nameof(ScoreReactive))]
public class ScoreReactive : ScriptableReactive<Score>
{
    // Score is an existing class
}
```

> Ensure the type is marked `[Serializable]` to appear in the inspector.

<div align="center" style="text-align:center;">
  <img src="https://github.com/thisaislan/just-images/raw/main/images/scriptables/reactive.png"  width="500" > 
  <br>
</div>
<br>

```csharp
public class ScoreController : MonoBehaviour
{
    [SerializeField]
    private ScoreReactive scoreReactive;

    private void OnEnable()
    {
        scoreReactive.Subscribe(OnScoreChange);
    }

    private void OnDisable()
    {
        scoreReactive.Unsubscribe(OnScoreChange);
    }

    private void OnScoreChange(Score newScore) { ... }
}
```

To set values and trigger events:

```csharp
scoreReactive.Set(new Score(...))        // Sets and notifies all subscribers
scoreReactive.SetSilently(new Score(...)) // Sets the value silently — no notification
scoreReactive.Emit();                     // Emits the current value without changing it
```

</br>

### Custom No-Parameter Reactive

A signal-only reactive that fires an event without passing data:

```csharp
[CreateAssetMenu(fileName = nameof(GameOverSignal), menuName = "MyGame/Signals/" + nameof(GameOverSignal))]
public class GameOverSignal : NoParamsReactive { }
```

```csharp
public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameOverSignal gameOverSignal;

    public void EndGame()
    {
        gameOverSignal.Emit();
    }
}
```

```csharp
public class UIManager : MonoBehaviour
{
    [SerializeField]
    private GameOverSignalReactive gameOverSignalReactive;

    private void OnEnable() 
    {
        gameOverSignalReactive.Subscribe(ShowGameOverScreen);
    }

    private void OnDisable()
    {
        gameOverSignalReactive.Unsubscribe(ShowGameOverScreen);
    }

    private void ShowGameOverScreen() { ... }
}
```
<div align="center" style="text-align:center;">
  <img src="https://github.com/thisaislan/just-images/raw/main/images/scriptables/no_parm_reactive.png"  width="500" > 
  <br>
</div>
<br>

</br>

### Pre-made Types

Ready-to-use reactive and runtime assets are available via **Create > Scriptables**:

| Reactive | Runtime | Settings |
|---|---|---|
| `BooleanReactive` | `BooleanRuntime` | `BooleanSettings` |
| `ColorReactive` | `ColorRuntime` | `ColorSettings` |
| `DoubleReactive` | `DoubleRuntime` | `DoubleSettings` |
| `FloatReactive` | `FloatRuntime` | `FloatSettings` |
| `GameObjectReactive` | `GameObjectRuntime` | `GameObjectSettings` |
| `IntReactive` | `IntRuntime` | `IntSettings` |
| `QuaternionReactive` | `QuaternionRuntime` | `QuaternionSettings` |
| `StringReactive` | `StringRuntime` | `StringSettings` |
| `TransformReactive` | `TransformRuntime` | `TransformSettings` |
| `Vector2Reactive` | `Vector2Runtime` | `Vector2Settings` |
| `Vector3Reactive` | `Vector3Runtime` | `Vector3Settings` |

</br>

## 📌 Pin Feature

ScriptableObjects are normally unloaded when no scene references point to them. The **pin system** prevents that:

```csharp
public class GameManager : MonoBehaviour
{
    [SerializeField]
    private PlayerStateRuntime playerStateRuntime;

    private void Awake() => playerStateRuntime.Pin();   // keep alive

    private void OnDestroy() => playerStateRuntime.Unpin(); // release
}
```

Pinned objects survive domain reloads and remain functional across play mode transitions. The `ReferenceKeeper` internally uses a `HashSet` and a hidden scene `ScriptableCache` MonoBehaviour.

<div align="center" style="text-align:center;">
  <img src="https://github.com/thisaislan/just-images/raw/main/images/scriptables/pinneds.png"  width="500" > 
  <br>
</div>
<br>

</br>

## 🧩 Interfaces

All base classes implement standard interfaces so you can code against contracts:

| Interface | Method | Applied To |
|---|---|---|
| `ISubscribable<T>` | `Subscribe(Action<T>)` / `Unsubscribe(Action<T>)` | `ScriptableReactive<T>` |
| `ISubscribable` | `Subscribe(Action)` / `Unsubscribe(Action)` | `NoParamsReactive` |
| `IEmitable` | `Emit()` | `ScriptableReactive<T>`, `NoParamsReactive` |
| `ISettable<T>` | `Set(T)` | `ScriptableReactive<T>`, `ScriptableRuntime<T>` |
| `ISilentSettable<T>` | `SetSilently(T)` | `ScriptableReactive<T>` |
| `IResettable` | `Reset()` | `ScriptableRuntime<T>`, `ScriptableReactive<T>` |
| `IInitializable<T>` | `Initialize(T)` | `ScriptableSettings<T>` |
| `IPinnable` | `Pin()` / `Unpin()` | All base classes |

</br>

## 📦 Install

1. Copy the git URL: `https://github.com/thisaislan/scriptables.git`

2. In Unity Editor, click **Window > Package Manager**

3. Click the **+** button and select **Add package from git URL...**

4. Paste the URL and click **Add**

</br>

## 🤝 Support
Please submit any queries, bugs or issues, to the [Issues](https://github.com/thisaislan/scriptabes/issues) page on this repository. All feedback is appreciated as it not just helps myself find problems I didn't otherwise see, but also helps improve the project.

</br>

## 💖 Thanks
My friends and family, and you for having come here!

</br>

## 📄 License
Copyright (c) 2024-present Aislan Tavares (@thisaislan) and Contributors. Scriptables is free and open-source software licensed under the [MIT License](https://github.com/thisaislan/scriptables/blob/main/LICENSE.md).


<!--
  ko-fi donation button 
 -->
<br>
<br>
<br>
<br>
<br>
<br>
<h4 align="center" style="text-align:center;">
  <a href="https://ko-fi.com/thisaislan">
    <img src="https://github.com/thisaislan/just-images/raw/main/images/ko-fi/ko-fi_donation_banner.gif" style="width: 460px">
  </a>
</h4>
<br>
<br>

<h4 align="center" style="text-align:center;">
  <a href="https://github.com/thisaislan/scriptables">
    <img src="https://github.com/thisaislan/just-images/raw/main/images/scriptables/scriptables_all.png" style="width: 100px">    
  </a>

  Enjoy! ♥️
</h4>
<br>
