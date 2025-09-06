<p align="center">
  <a href="https://github.com/thisaislan/janus-scenes">
    <img src="https://github.com/thisaislan/just-images/raw/main/images/scriptables/scriptables_logo.png">
  </a>
</p>

<h1 style="font-size: 50px" align ="center">
  Scriptables  
  <h4  align ="center">
    A powerful Unity package that enhances ScriptableObjects with runtime data management, editor debugging capabilities, and intuitive data separation between editor and runtime environments.
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
- [Features](#✨-features)
- [Overview](#🏗️-overview)
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
- **Reactive ScriptableObjects**: Preserve editor data while allowing other classes to add observer to value changes
- **Advanced Editor Integration**: Custom inspector with real-time data visualization
- **Type-Safe Data Management**: Generic implementation supporting any serializable data type
- **Reflection-Based UI**: Automatically generates editor UI for the data structures (Editor mode)
- **Reset Management**: Automatic data cleanup when entering/exiting play mode
- **Scriptable Panel**: Panel to help organize all scriptableObjects in the project (Tools/Scriptable/Panel)

</br>

## 🏗️ Overview

The package follows a clean ScriptableObject architecture by dividing them into three specialized types:

### ⚙️ Scriptable Settings
- **Fixed data** configured to be used when the game runs
- Can be updated via remote config, repositories, or any external means
- Persistent data that maintains editor-configured values
- Ideal for game configuration, constants, and setup parameters

<h5 style="font-size: 1px" align ="center">
    <br>
</h5>


### 🔄 Scriptable Runtime
- **Temporary data** shared across different systems in the project
- Volatile data that resets automatically
- Perfect for game state, temporary variables, and system communication
- No persistence - reset to defaults on every run

<h5 style="font-size: 1px" align ="center">
    <br>
</h5>


### 🔥Scriptable Reactive
- **Observable data** that notifies listeners automatically when values change
- Editor/Runtime separation maintains different values for edit mode vs. play mode
- Event-driven architecture allows decoupled communication between systems
- Automatic persistence preserves editor-configured values while allowing runtime modifications

</br>

## 🚀 Quick Start 
<h5 style="font-size: 1px" align ="center">
    <br>
</h5>


### Creating Scriptable Settings

```csharp
[CreateAssetMenu(fileName = "BarScriptableSettings", menuName = "Bar/Settings")]
public class BarScriptableSettings : ScriptableSettings<BarScriptableSettings.BarData>
{
    [Serializable]
    public class BarData : Data
    {
        public string Foo;
    }
}
```

<h5 style="font-size: 1px" align ="center">
    <br>
</h5>

When a new instance is created of these ScriptableObjects in the Inspector, they appear with a clean, organized interface:

<h5 style="font-size: 1px" align ="center">
    <br>
</h5>


<div align="center" style="text-align:center;">
  <img src="https://github.com/thisaislan/just-images/raw/main/images/scriptables/settings.png"  width="500" > 
  <br>
</div>

<h5 style="font-size: 1px" align ="center">
    <br>
</h5>

And in play mode:

<h5 style="font-size: 1px" align ="center">
    <br>
</h5>

<div align="center" style="text-align:center;">
  <img src="https://github.com/thisaislan/just-images/raw/main/images/scriptables/settings_play_mode.png"  width="500" > 
  <br>
</div>

<h5 style="font-size: 1px" align ="center">
    <br>
</h5>

The editor provides:

    Clear separation between Settings and Runtime data

    Intuitive interface for configuring default values

    Visual indicators for data type and purpose

    Method to initialize data when necessary

    Method to print data (Editor only)

<h5 style="font-size: 1px" align ="center">
    <br>
</h5>

### Creating Scriptable Runtime

```csharp
[CreateAssetMenu(fileName = "FooScriptableRuntime", menuName = "Foo/Runtime")]
public class FooScriptableRuntime : ScriptableRuntime<FooScriptableRuntime.FooData>
{
    [Serializable]
    public class FooData : Data
    {
        public int Bar;
    }
}
```

<h5 style="font-size: 1px" align ="center">
    <br>
</h5>


Instance:
<h5 style="font-size: 1px" align ="center">
    <br>
</h5>


<div align="center" style="text-align:center;">
  <img src="https://github.com/thisaislan/just-images/raw/main/images/scriptables/runtime.png"  width="500" > 
  <br>

</div>

<h5 style="font-size: 1px" align ="center">
    <br>
</h5>


Play mode:

<h5 style="font-size: 1px" align ="center">
    <br>
</h5>


<div align="center" style="text-align:center;">
  <img src="https://github.com/thisaislan/just-images/raw/main/images/scriptables/runtime_play_mode.png"  width="500" > 
  <br>
</div>

<h5 style="font-size: 1px" align ="center">
    <br>
</h5>


As ScriptableSettings the runtime data can be changed freely during gameplay without any persistence concerns. The system automatically handles:

    Automatic Reset: Runtime data resets to default values on every run in the Unity Editor

    Zero Persistence: Changes made during runtime are never saved automatically

    Isolated Modifications: Runtime changes don't affect the original editor-configured values

    Clean State: Every play session starts with fresh, default data

<h5 style="font-size: 1px" align ="center">
    <br>
</h5>

Runtime features include:

    Real-time data modification capabilities

    Visual feedback for data changes

    Reset functionality for testing

    Clean display of current runtime values

    Method to reset data when necessary
    
    Method to print data (Editor only)

<h5 style="font-size: 1px" align ="center">
    <br>
</h5>

### Creating Scriptable Reactive

Scriptable Reactives are observable data containers that automatically notify listeners when their values change, enabling reactive programming patterns throughout on the project.


It is possible to create Scriptable Reactives using the asset menu by right-clicking and selecting:
Create > Scriptables > Reactives

<h5 style="font-size: 1px" align ="center">
    <br>
</h5>

The following pre-made types are available:

  - BooleanScriptableReactive
  - ColorScriptableReactive
  - DoubleScriptableReactive
  - FloatScriptableReactive
  - GameObjectScriptableReactive
  - IntScriptableReactive
  - NoParametersScriptableReactive
  - QuaternionScriptableReactive
  - StringScriptableReactive
  - TransformScriptableReactive
  - Vector2ScriptableReactive
  - Vector3ScriptableReactive

<h5 style="font-size: 1px" align ="center">
    <br>
</h5>

When a instances of these ScriptableObjects is created in the Inspector (Vector2 type):

<h5 style="font-size: 1px" align ="center">
    <br>
</h5>


<div align="center" style="text-align:center;">
  <img src="https://github.com/thisaislan/just-images/raw/main/images/scriptables/reactive.png"  width="500" > 
  <br>
</div>

<h5 style="font-size: 1px" align ="center">
    <br>
</h5>

In play mode:

<h5 style="font-size: 1px" align ="center">
    <br>
</h5>

<div align="center" style="text-align:center;">
  <img src="https://github.com/thisaislan/just-images/raw/main/images/scriptables/reactive_play_mode.png"  width="500" > 
  <br>
</div>

<h5 style="font-size: 1px" align ="center">
    <br>
</h5>

  It is possible to extend the ScriptableReactive class to create custom reactive types:

```csharp
[CreateAssetMenu(fileName = "FooBarScriptableReactive", menuName = "FooBar/Reactive")]
public class FooBarScriptableReactive : ScriptableReactive<FooBarScriptableReactive>
{
    // Methods here if needed
}
```

> Note: Not all types are serializable by default or will be visible in the editor. Ensure the custom type is marked as [System.Serializable] if it's a custom class.

<h5 style="font-size: 1px" align ="center">
    <br>
</h5>

Like ScriptableSettings, Scriptable Reactives handle runtime data intelligently:

    Automatic Reset: Runtime values reset to editor-configured defaults on every play session

    Zero Persistence: Runtime modifications are never automatically saved

    Isolated Modifications: Editor values remain untouched during gameplay

    Clean State: Each play session starts with fresh, predictable data

<h5 style="font-size: 1px" align ="center">
    <br>
</h5>


Features include:

    Method to reset data (When possible)
    
    Method to print data (Editor only)

    Methods to ensure observability of the data

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
  <a href="https://github.com/thisaislan/scriptables">
    <img src="https://github.com/thisaislan/just-images/raw/main/images/scriptables/scriptables_all.png" style="width: 100px">    
  </a>

  Enjoy! ♥️
</h4>
<br>
