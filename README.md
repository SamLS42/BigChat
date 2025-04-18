# BigChat

BigChat is a WinUI 3 chat application built using a vertical slice architecture. It provides a modern desktop interface for chatting with large language models. You can choose between using the Ollama inference engine or Azure AI Inference to drive the chat functionality.

![image](https://github.com/user-attachments/assets/21d88ffa-0792-443d-849c-f02f16e6e0b1)

## Features

- **Chat UI:** A responsive interface where conversations are listed and managed.
- **Multiple AI Providers:** Support for both Ollama and Azure AI Inference enabling flexible chat experiences.
- **Local Data Store:** Entity Framework Core with SQLite is used for data persistence to store conversations and messages.
- **Settings Management:** Easily switch between AI providers and customize chat parameters (such as temperature, token limits, and penalties).

## Prerequisites

- **Windows 10** or later.
- **Visual Studio 2022** with WinUI 3 and .NET 9 development workload installed.
- **.NET 9 SDK**
- **[CommunityToolkit.Labs packages source](https://github.com/CommunityToolkit/Labs-Windows?tab=readme-ov-file#getting-started)**

## Getting Started

1. **Clone the Repository**

2. **Restore Dependencies and Build**

   Open the solution in Visual Studio 2022. Restore NuGet packages, then build the solution.

3. **Run the Application**

   Set **BigChat.WinUI** as the startup project and run the application. The main window will load the chat interface, where you can start a new chat session with your preferred AI provider.

## Project Structure

- **BigChat.WinUI:** Contains the WinUI 3 UI implementation and application startup code.
- **BigChat.AppCore:** Houses the application logic including view models, messaging, and navigation.
- **BigChat.Infrastructure:** Implements the data access layer with EF Core (SQLite) and integrates AI provider services.

## Configuring AI Providers

In the **Settings** page of the application, you can configure the endpoints and credentials for:
- **Ollama Chat:** Set the model ID, temperature, and other chat parameters.
- **Azure AI Inference:** Provide the API key, model ID, and adjust parameters such as temperature and token limits.

Adjust these settings to tailor the chat experience to your needs.

## Disclaimer

**This project is not intended for production use.**  
It leverages several **alpha, preview, and experimental libraries**. These libraries may have unstable APIs and limited support, so use this project only for experimental purposes or as a learning tool.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE.txt) file for details.

## Acknowledgments

- The project leverages the latest [WinUI](https://github.com/microsoft/microsoft-ui-xaml) framework.
- Built using technologies such as .NET 9, Entity Framework Core, and CommunityToolkit libraries.

---

Happy coding and enjoy your chat experience with BigChat!

