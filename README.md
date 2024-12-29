# New Moon Notifier

*Ever wondered when to celebrate the new moon, just like in the Bible? Well, you're in luck! The New Moon Notifier has got you covered!*

## Overview

This project is a sleek template for deploying a binary package to NetDaemon. But wait—it’s not just *any* template. This one sends you WhatsApp notifications about upcoming new moons! Because nothing says "I'm organized" like a celestial calendar at your fingertips.

## Features

- *Cosmic WhatsApp*: Sends a message to remind you of the new moon within 72 hours.
- *Galactic Sensors*: Updates Home Assistant with countdowns for the next new moon.
- *Persistent Magic*: Pops up notifications so you never miss the lunar action.

## Prerequisites

Before diving into the lunar vibes, you’ll need:

- .NET 9 SDK (*because 8 is sooo last lunar cycle*)
- Docker (because containers are cool, like spaceships)
- Home Assistant (to make your smart home extra cosmic)
- NetDaemon (the secret sauce of automation wizards)

## Setup

Ready to launch your moon tracker? Follow these steps:

1. Clone this interstellar repository:

    ```sh
    git clone <repository-url>
    cd HomeAssistant
    ```

2. Restore .NET dependencies:

    ```sh
    dotnet restore
    ```

3. Build your project (*because building is cooler than buying*):

    ```sh
    dotnet build
    ```

4. Publish it (*not to the moon, just yet*):

    ```sh
    dotnet publish -c Release -o ./publish
    ```

5. Build your Docker spaceship:

    ```sh
    docker build -t newmoonnotifier:latest .
    ```

6. Launch the Docker container (*Houston, we have lift-off!*):

    ```sh
    docker run -d --name newmoonnotifier newmoonnotifier:latest
    ```

## Configuration

Add some magic (a.k.a. your Home Assistant details) to a file named `appsettings.Development.json`:

```json
{
    "HomeAssistant": {
        "Host": "homeassistant.local",
        "Port": 8123,
        "Ssl": false,
        "Token": "your_home_assistant_long_lived_access_token"
    }
}
```

Remember: Replace "your_home_assistant_long_lived_access_token" with your actual token. No, "abracadabra" won’t work.

## Issues?

- If you’re stuck, you can [add an issue](https://github.com/net-daemon/netdaemon-app-template) and summon the GitHub wizards.
- For NetDaemon-related woes, [add an issue here](https://github.com/net-daemon/netdaemon/issues).

## Community & Support

Feeling lonely on your lunar journey? Join the cosmic chatter:

- [Discord](https://discord.gg/K3xwfcX) (*where all the cool lunar geeks hang out*)

## License

This project is licensed under the MIT License. Translation? Do cool stuff with it, but don't sue us if your lunar obsession gets out of hand.

---

*May your new moons be timely and your automations bug-free!*
