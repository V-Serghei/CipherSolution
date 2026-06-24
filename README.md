# CipherSolution

CipherSolution is an educational .NET application that implements classical polyalphabetic ciphers and demonstrates several software design patterns.

## Overview

The project supports four cipher algorithms:

| Cipher | Description |
| --- | --- |
| Vigenere | Standard polyalphabetic cipher with a repeating keyword |
| Beaufort | Variant of Vigenere with reversed encryption and decryption logic |
| AutoKey | Self-keying cipher where the key is extended with plaintext |
| Running Key | Cipher that uses a long text as the key |

Supported alphabets include English, Russian, and extended variants with digits and symbols depending on configuration.

The desktop UI uses the same cipher creation paths as the console application and validates key/text alphabet compatibility before running an operation.

## Tech Stack

- .NET 10
- C#
- WPF desktop UI
- Console UI
- NUnit 4
- JSON session history storage

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
|-- CipherLib/           Cipher algorithms and design pattern implementations
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

## Running Tests Manually

```bat
dotnet test CipherSolution.sln --configuration Release
```
