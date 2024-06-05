# Minecraft Realms Emulator

This is a custom implementation of a Minecraft Realms server for Java Edition,
written in C#.

# Requirements

- .NET SDK (version 8.0 or higher)
- Minecraft Java Edition
- PostgreSQL (for database support)
- Docker (for `REALMS` mode to work)

# Installation

Clone the repository:

```sh
git clone https://github.com/CyberL1/Minecraft-Realms-Emulator.git 
cd Minecraft-Realms-Emulator/Minecraft-Realms-Emulator
```

Configure the server:

Create a `.env` file:

```
CONNECTION_STRING="User Id=postgres;Password=password;Server=db;Port=5432;Database=Minecraft-Realms-Emulator;"
ADMIN_KEY="[RANDOMLY GENERATED KEY]"
```

Build the server:

```sh
dotnet build
```

Run the server:

```sh
dotnet run
```
