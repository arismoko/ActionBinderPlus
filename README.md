# Action Binder Plus

## Overview

**Action Binder Plus** is a versatile tool designed to simplify the management of input actions for interactable items, making it ideal for handling complex inventory systems and various interactive elements in your game.

## Features

- **Flexible Input Binding**: Easily bind input actions to various interactable items.
- **Inspector Customization**: Enhanced editor inspector for intuitive input action management.
- **JSON Import/Export**: Save and load your input action bindings to/from JSON files.
- **Dynamic Action Handling**: Automatically handle action execution and cancellation.

## Installation

1. Clone or download the repository.
2. Place the `ActionBinderPlus` folder into your Unity project's `Assets` directory.

## Usage

### Setting Up an Interactable Item

1. Create a new GameObject in your scene and add the `InteractableItem` component.
2. Configure the `InteractableItem` component:
   - Assign a target component.
   - Set additional options such as `deactivateOnInteract` and `destroyOnInteract`.

### Binding Actions

1. Select your `InteractableItem` in the Unity Editor.
2. In the Inspector, you will see the "Input Action Bindings" section.
3. Use the provided interface to add new bindings:
   - **Unique Binding ID**: A unique identifier for the binding.
   - **Action Name**: The name of the input action.
   - **Method Name**: The method to be called on action trigger.
   - Additional options based on whether the action should be called immediately, or on started or canceled states.

### Exporting and Importing Bindings

- **Export to JSON**: Save your current bindings to a JSON file.
- **Import from JSON**: Load bindings from a previously saved JSON file.

### Examples

A simple example is provided in the `Samples` folder. You must have TMP installed. The sample was made using the URP pipeline. More examples will be added in the future to demonstrate various use cases and features of the package.

## Contributing

1. Fork the repository.
2. Create a new branch for your feature or bugfix.
3. Commit your changes.
4. Push your branch and create a pull request.

## License

This project is licensed under the MIT License.
