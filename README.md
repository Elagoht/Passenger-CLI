# Passenger CLI

![Codacy Badge](https://app.codacy.com/project/badge/Grade/3ffd2277d4154220bc7477096f390f67)
![C# .Net](https://img.shields.io/badge/C%23-.Net_8.0-purple)
![GitHub Repo stars](https://img.shields.io/github/stars/Elagoht/Passenger-cli?style=flat)
![GitHub Issues or Pull Requests](https://img.shields.io/github/issues/Elagoht/Passenger-cli)
![GitHub Issues or Pull Requests](https://img.shields.io/github/issues-pr/Elagoht/Passenger-cli)
![GitHub License](https://img.shields.io/github/license/Elagoht/Passenger-cli)

Passenger is a sophisticated passphrase manager designed to securely store your passphrases. This project encompasses the backend and database components, leveraging AES GCM encryption alongside a user-written custom and unique symmetric encoding-decoding algorithm to ensure maximum security. The AES GCM secret key is required as an environment variable to enhance security.

## Features

- **Database Operations**: The [`Database`](DataBase.cs) class facilitates CRUD operations for the database.
- **File System Interactions**: The [`FileSystem`](FileSystem.cs) class handles reading and writing data to the file system.
- **Authorization**: Managed in [`Authorization.cs`](Authorization.cs).
- **Command Line Interactions**: Implemented in [`CommandLine.cs`](CommandLine.cs).
- **Encoding and Decoding**: Provided by [`EnDeCoder.cs`](EnDeCoder.cs).
- **Global Variables and Constants**: Defined in [`Globals.cs`](Globals.cs).
- **Worker Operations**: Executed in [`Worker.cs`](Worker.cs).

## Building the Project

To build this project, .NET 8.0 or later is required.

Generate a binary build using the following command:

```sh
dotnet publish
```

All necessary arguments are defined in the project settings.

## Running the Project

After building the project, execute the binary located at `./bin/Release/net8.0/osx-arm64/publish/Passenger`. This is a standalone executable with no dependencies.

## Usage

For help and manual pages, use the following commands:

```sh
./Passenger --help
```

```sh
./Passenger man
```

## Contributing

We welcome contributions! Please submit a pull request or create an issue to discuss any changes you would like to make.

## License

This project is licensed under the GNU General Public License v3.0. For more details, see the [LICENSE](LICENSE) file in the project root.

---

Feel free to reach out if you have any questions or need further assistance. Thank you for using Passenger!
