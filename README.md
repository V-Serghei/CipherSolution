# CipherSolution

A .NET console application implementing classical polyalphabetic ciphers with a focus on software design patterns.

## Overview

CipherSolution demonstrates four classical substitution ciphers:

| Cipher | Description |
|--------|-------------|
| **Vigenère** | Standard polyalphabetic cipher with a repeating keyword |
| **Beaufort** | Variant of Vigenère with reversed encryption/decryption |
| **AutoKey** | Self-keying cipher — key is extended with the plaintext |
| **Running Key** | Uses a long text (e.g. book passage) as the key |

Supported alphabets: Russian (Cyrillic), English, and an extended set that includes digits and symbols.

## Design Patterns

| Pattern | Location |
|---------|----------|
| Abstract Factory | `CipherLib/AbstractFactory/` |
| Builder | `CipherLib/Builder/` |
| Factory Method | `CipherLib/Factory/` |
| Singleton | `Logging/` |
| Prototype | `CipherLib/Prototype/` |

## Tech Stack

- **.NET 10** — runtime and SDK
- **C#** — language
- **NUnit 4** — unit tests
- **JSON** — session history storage (`CipherLib/Prototype/data/allSessions.json`)

## Quick Start

```bat
start.bat
```

This single command restores packages, builds the solution, runs tests, and launches the interactive menu.

To stop the application:

```bat
stop.bat
```

## Project Structure

```
CipherSolution/
├── ApplicationL/        # Interactive console menu (entry point)
├── CipherLib/           # Cipher algorithms and design pattern implementations
├── Logging/             # Singleton logger
├── CipherTests/         # NUnit test suite
├── start.bat            # Setup and launch script
├── stop.bat             # Shutdown script
└── CipherSolution.sln
```

## Running Tests Manually

```bat
dotnet test CipherSolution.sln --configuration Release
```
