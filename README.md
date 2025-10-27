# BigChat

BigChat is a WinUI 3 chat application. It provides a modern desktop interface for chatting with large language models.

## Features

* **Chat UI:** A responsive interface for managing conversations.
* **Ollama Support:** Currently supports **only Ollama**, integrated via **OllamaSharp** and **Microsoft.Extensions.AI**.
* **Local Data Store:** Uses **Entity Framework Core with SQLite** for persisting conversations and messages.
* **Settings Management:** Configure chat parameters such as temperature, token limits, and penalties.
* **Custom Markdown Rendering:** Uses a modified version of `MarkdownTextBlock` from the Community Toolkit.

## Prerequisites

* **Windows 11** or later
* **Visual Studio 2026** with WinUI 3 and .NET 10 workloads
* **.NET 10 SDK**
* **[Modified CommunityToolkit.Labs packages source](https://github.com/SamLS42/Labs-Windows?tab=readme-ov-file#getting-started)**

## Getting Started

1. **Clone the Repository**
2. **Restore Dependencies and Build**
   Open the solution in Visual Studio, restore NuGet packages, and build the project.
3. **Run the Application**
   Set **BigChat.WinUI** as the startup project and run. The main window will load the chat interface.

## Project Structure

* **BigChat.WinUI:** UI layer built with WinUI 3.
* **BigChat.AppCore:** Application logic, view models, and navigation.
* **BigChat.Infrastructure:** Data access with EF Core and Ollama service integration.

## Configuring AI Providers

Currently, only **Ollama** is supported.
In the **Settings** page, set:

* **Model ID**
* **Temperature**
* **Token limit** and related chat parameters

Additional providers (e.g., Azure AI Inference) will be supported later.

## Disclaimer

**Experimental use only.**
This project relies on **alpha and preview libraries**, which may change or break in future updates. Use for learning or experimentation.

## License

Licensed under the **MIT License**. See [LICENSE](LICENSE.txt) for details.

## Acknowledgments

* Built on [WinUI](https://github.com/microsoft/microsoft-ui-xaml)
* Uses **.NET 10**, **Entity Framework Core**, and **CommunityToolkit** components

---

Efficient, modern, and open.
