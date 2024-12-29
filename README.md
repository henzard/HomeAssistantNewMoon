# New Moon Notifier

This project is a template for deploying a binary package to NetDaemon. It includes a NewMoonNotifier app that sends notifications about the next new moon using Home Assistant and NetDaemon.

For more details, see the [NewMoonNotifier.cs](HomeAssistant/apps/NewMoonNotifier/NewMoonNotifier.cs) file.

## Features

- Sends WhatsApp messages when the next new moon is within 72 hours.
- Updates Home Assistant sensors with the time remaining until the next new moon.
- Sends persistent notifications with information about the next new moon.

## Getting Started

Please see [netdaemon.xyz](https://netdaemon.xyz/docs) for more information about getting started with developing apps for Home Assistant using NetDaemon.

### Prerequisites

- .NET 9 SDK
- Docker
- Home Assistant
- NetDaemon

### Setup

1. Clone the repository:

    ```sh
    git clone <repository-url>
    cd HomeAssistant
    ```

2. Restore the .NET dependencies:

    ```sh
    dotnet restore
    ```

3. Build the project:

    ```sh
    dotnet build
    ```

4. Publish the project:

    ```sh
    dotnet publish -c Release -o ./publish
    ```

5. Build the Docker image:

    ```sh
    docker build -t newmoonnotifier:latest .
    ```

6. Run the Docker container:

    ```sh
    docker run -d --name newmoonnotifier newmoonnotifier:latest
    ```

### Using the Code Generator

Please add code generation features in 

program.cs

 when using code generation features by removing comments.

See [NetDaemon Code Generator](https://netdaemon.xyz/docs/hass_model/hass_model_codegen) for more details.

## Issues

- If you have issues or suggestions for improvements to this template, please [add an issue](https://github.com/net-daemon/netdaemon-app-template).
- If you have issues or suggestions for improvements to NetDaemon, please [add an issue](https://github.com/net-daemon/netdaemon/issues).

## Discuss the NetDaemon

### Community and Support

Join the conversation and get support from the NetDaemon community. Share your experiences, ask questions, and help others by participating in discussions.

Please [join the Discord server](https://discord.gg/K3xwfcX) to get support or if you want to contribute and help others.

## Code Overview

### [program.cs](HomeAssistant/program.cs)

The entry point of the application. It sets up the NetDaemon host and registers the [NewMoonNotifier.cs](HomeAssistant/apps/NewMoonNotifier/NewMoonNotifier.cs) app.

### [NewMoonNotifier.cs](HomeAssistant/apps/NewMoonNotifier/NewMoonNotifier.cs)

Contains the [NewMoonNotifier.cs](HomeAssistant/apps/NewMoonNotifier/NewMoonNotifier.cs) class, which is responsible for:

- Sending WhatsApp messages when the next new moon is within 72 hours.
- Updating Home Assistant sensors with the time remaining until the next new moon.
- Sending persistent notifications with information about the next new moon.

### [build.ps1](HomeAssistant/build.ps1)

A PowerShell script to automate the build and deployment process. It:

1. Navigates to the project directory.
2. Runs `dotnet build`.
3. Runs `dotnet publish`.
4. Builds the Docker image.
5. Stops and removes any existing Docker container.
6. Runs a new Docker container.

### [Dockerfile](HomeAssistant/Dockerfile)

Defines the Docker image for the project. It:

1. Uses the .NET 9 SDK to build the project.
2. Publishes the project to a folder.
3. Uses the .NET 9 ASP.NET runtime to run the published project.

## License

This project is licensed under the MIT License. See the LICENSE file for details.
