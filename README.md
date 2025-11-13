# BigChat

BigChat is a WinUI 3 chat application that provides a modern desktop interface for interacting with large language models. It is designed for experimentation, developer workflows, and local/cloud model usage on Windows.

![Screenshot](https://github.com/user-attachments/assets/4db0be8a-5220-45f0-96aa-264aec1851f3)

## Highlights

- Native WinUI 3 desktop UI with a responsive chat experience.
- Flexible provider support: local Ollama models and OpenAI-compatible services.
- Conversation management with persistent, on-device storage (SQLite + EF Core).
- Fine-grained model controls: temperature, top-p, max output tokens, frequency/presence penalties, and restore defaults.
- Streaming responses and custom Markdown rendering for readable replies.
- Open-source and extensible — aimed at developers and advanced users.

## Features

* Modern WinUI 3 chat interface for creating and managing conversations.
* Multiple AI provider support:
  - Ollama (local model server via OllamaSharp)
  - OpenAI-compatible endpoints (use your API key)
* Local data store using Entity Framework Core with SQLite – conversations and messages are stored on-device.
* Settings page to configure provider, endpoints, API keys, and model parameters.
* Custom Markdown rendering using a modified `MarkdownTextBlock` from the Community Toolkit.

## Prerequisites

* Windows 11 or later.
* .NET 10 SDK.
* Visual Studio (recommended) with WinUI 3 workload to build from source.
* For Ollama: install and run a local Ollama server before using Ollama models.
* For cloud providers: valid API key(s) and correct endpoint configuration in Settings.

## Getting Started

1. Clone the repository.
2. Restore dependencies and build in Visual Studio or via the .NET CLI.
3. Run the app (`BigChat.WinUI`).
4. Open Settings and choose your AI provider. Configure endpoint, model ID, and API key if required.
5. Start a new conversation and type your message in the input box. Responses stream back as they are generated.

## Configuring AI Providers

Open Settings to select the active provider and set provider-specific values:

* `Ollama`: set the local endpoint (default `http://localhost:11434`) and choose a local model.
* `OpenAI-compatible`: set endpoint, API key, and model ID.

## Privacy & Security

* Conversation history is stored locally in a SQLite database by default.
* Model inference is performed by the configured provider. If you use a remote provider (OpenAI or Azure), your data is sent to that service according to its policy. Review provider privacy terms before connecting.

## Limitations & Notes

* Experimental: the app relies on preview and alpha libraries and is intended for learning and experimentation.

## License

Licensed under the MIT License. See `LICENSE.txt` for details.

## Acknowledgments

* Built with WinUI 3, .NET 10, Entity Framework Core, and CommunityToolkit components.

---

Efficient, modern, and open.
