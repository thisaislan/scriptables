<p align="center">
  <a href="https://github.com/thisaislan/janus-scenes">
    <img src="https://github.com/thisaislan/just-images/raw/main/images/scriptables/scriptables_banner.png">
  </a>
</p>


<h1 align="center" style="text-align:center;">
  Scriptables
</h1>

A powerful Unity package that enhances ScriptableObjects with runtime data management, editor debugging capabilities, and intuitive data separation between editor and runtime environments.

<p align="center">
    <a href="https://unity3d.com/get-unity/download">
        <img src="https://img.shields.io/badge/unity-tools-blue" alt="Unity Download Link"></a>
    <a href="https://github.com/thisaislan/scriptables/blob/main/LICENSE.md">
        <img src="https://img.shields.io/badge/License-MIT-brightgreen.svg" alt="License MIT"></a>
    <a href="https://chat.deepseek.com">
        <img src="https://img.shields.io/badge/%F0%9F%92%AC-DeepSeek%20AI-blue" alt="License MIT"></a>
    <a href="https://chatgpt.com/">
        <img src="https://img.shields.io/badge/%F0%9F%92%AC-Chat%20GPT%20AI-black" alt="License MIT"></a>
</p>

</br>
</br>

### Table of Contents
- [Features](#✨-features)
- [Architecture Overview](#🏗️-architecture-overview)
- [Quick Start](#🚀-quick-start)
- [Install](#📦-install)
- [Support](#🤝-support)
- [Thanks](#💖-thanks)
- [License](#📄-license)

</br>

## ✨ Features

- **Dual Data System**: Separate editor data from runtime data with automatic reset functionality
- **Runtime ScriptableObjects**: Data that automatically resets when exiting play mode
- **Settings ScriptableObjects**: Preserve editor data while allowing runtime modifications
- **Advanced Editor Integration**: Custom inspector with real-time data visualization
- **Type-Safe Data Management**: Generic implementation supporting any serializable data type
- **Reflection-Based UI**: Automatically generates editor UI for your data structures
- **Reset Management**: Automatic data cleanup when entering/exiting play mode

</br>

## 🏗️ Architecture Overview

The package follows a clean ScriptableObject architecture by dividing them into two specialized types:

### ⚙️ Settings ScriptableObjects
- **Fixed data** configured to be used when the game runs
- Can be updated via remote config, repositories, or any external means
- Persistent data that maintains editor-configured values
- Ideal for game configuration, constants, and setup parameters

### 🔄 Runtime ScriptableObjects  
- **Temporary data** shared across different systems in the project
- Volatile data that resets automatically
- Perfect for game state, temporary variables, and system communication
- No persistence - reset to defaults on every run

## 🚀 Quick Start

### Creating Settings ScriptableObjects

```csharp
[CreateAssetMenu(fileName = "RateScriptableRuntime", menuName = "Exploding Ghosts/Scriptable/Runtime/Rate")]
public class RateScriptableRuntime : ScriptableRuntime<RateScriptableRuntime.RateData>
{
    [Serializable]
    public class RateData: Data
    {
        public int UpLeftSpawnerRate;
        public int UpRightSpawnerRate;
        public int DownLeftSpawnerRate;
        public int DownRightSpawnerRate;
    }
}
```

```csharp
[CreateAssetMenu(fileName = "BoundariesScriptableSettings", menuName = "Exploding Ghosts/Scriptable/Settings/Boundaries")]
public class BoundariesScriptableSettings : ScriptableSettings<BoundariesScriptableSettings.BoundariesData>
{
    [Serializable]
    public class BoundariesData : Data
    {
        public float MinXPosition;
        public float MaxXPosition;
        public float MinZPosition;
        public float MaxZPosition;
    }
}
```

### 👀 Visual Examples

When you create instances of these ScriptableObjects in the Inspector, they appear with a clean, organized interface:

<div align="center" style="text-align:center;">

  <img src="https://github.com/thisaislan/just-images/raw/main/images/scriptables/runtime.png"  width="500" > 

  <br>

  <img src="https://github.com/thisaislan/just-images/raw/main/images/scriptables/settings.png" width="500" > 
  
</div>

The editor provides:

    Clear separation between Settings and Runtime data types

    Intuitive interface for configuring default values

    Visual indicators for data type and purpose

### 🙊 Runtime View

During gameplay, the data management becomes interactive:

<div align="center" style="text-align:center;">

  <img src="https://github.com/thisaislan/just-images/raw/main/images/scriptables/runtime_play_mode.png" width="500" > 

  <br>

  <img src="https://github.com/thisaislan/just-images/raw/main/images/scriptables/settings_play_mode.png" width="500" > 
  
</div>

Runtime features include:

    Real-time data modification capabilities

    Visual feedback for data changes

    Reset functionality for testing

    Clean display of current runtime values

🔄 Runtime Behavior

The runtime data can be changed freely during gameplay without any persistence concerns. The system automatically handles:

    Automatic Reset: Runtime data resets to default values on every run in the Unity Editor

    Zero Persistence: Changes made during runtime are never saved automatically

    Isolated Modifications: Runtime changes don't affect the original editor-configured values

    Clean State: Every play session starts with fresh, default data

</br>

## 📦 Install

1. Copying git url https://github.com/thisaislan/scriptables.git

2. Click on `Window/Package Manager` in Unity Editor

3. Click on add package button `Add package button`

4. Select `Add package from git URL...`

5. Past the url

6. Press `Enter` or clink on the `Add` button

7. Enjoy :satisfied:

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

Enjoy! :heart:
</h4>
<br>
