# CipherSolution

CipherSolution is an educational and practical .NET application for text encryption. It demonstrates classical polyalphabetic ciphers and software design patterns, while also offering a practical AES-GCM mode for real text/file workflows.

## Overview

The project supports classical algorithms:

| Cipher | Description |
| --- | --- |
| Vigenere | Polyalphabetic cipher with a repeating keyword |
| Beaufort | Reciprocal Vigenere-style cipher |
| AutoKey | Cipher that extends the key with plaintext |
| Running Key | Cipher that requires a key at least as long as the text |

The desktop app also includes:

- Practical mode with automatic alphabet detection
- AES-GCM text encryption with PBKDF2-SHA256 password-based key derivation
- English, Russian, number, symbol, and mixed alphabet variants
- Text file import and result export
- Operation history with round-trip verification
- Clipboard actions: copy, paste, swap, repeat
- Built-in cipher and pattern reference tab

Classical ciphers are useful for learning and experimentation. AES-GCM is the practical mode for authenticated text encryption.

## Tech Stack

- .NET 10
- C#
- WPF desktop UI
- Console UI
- NUnit 4
- JSON session history storage
- AES-GCM and PBKDF2 from `System.Security.Cryptography`

## Quick Start

Run the desktop application:

```bat
start.bat
```

Run the console application:

```bat
start.bat --console
```

Run tests during startup:

```bat
start.bat --test
```

Stop a running application instance:

```bat
stop.bat
```

## Environment Variables

No environment variables or secrets are required.

## Architecture

```text
CipherSolution/
|-- ApplicationL/        Console UI entry point
|-- CipherDesktop/       WPF desktop UI entry point
|-- CipherLib/           Cipher algorithms, workflows, and pattern implementations
|-- Logging/             Singleton loggers
|-- CipherTests/         NUnit test suite
|-- start.bat            Setup and launch script
|-- stop.bat             Shutdown script
`-- CipherSolution.sln
```

## Design Patterns

| Pattern | Location |
| --- | --- |
| Abstract Factory | `CipherLib/AbstractFactory/` |
| Builder | `CipherLib/Builder/` |
| Factory Method | `CipherLib/Factory/` |
| Singleton | `Logging/` |
| Prototype | `CipherLib/Prototype/` |
| Strategy | `CipherLib/Practical/` |
| Command | `CipherLib/Practical/CipherOperationCommand.cs` |
| Facade | `CipherLib/Practical/PracticalCipherFacade.cs` |

## Running Tests Manually

```bat
dotnet test CipherSolution.sln --configuration Release
```
