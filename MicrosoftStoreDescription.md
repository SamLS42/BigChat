A native WinUI 3 chat client to experiment with LLMs: local Ollama or OpenAI-compatible endpoints with local history.

Full description (for Microsoft Store listing)
BigChat is a lightweight, native WinUI 3 desktop chat client for experimenting with large language models on Windows. Built for developers and advanced users, BigChat offers a simple, responsive chat UI with persistent local history and configurable provider settings.

Key features
- Native WinUI 3 interface optimized for desktops.
- Provider support: local Ollama server and OpenAI-compatible endpoints (configure endpoint, API key, and model in Settings).
- Persistent, on-device conversation history (SQLite + EF Core).
- Fine-grained model controls: model ID, temperature, top-p, max output tokens, frequency and presence penalties, and restore defaults.
- Streaming responses and Markdown-rendered replies for clear, readable output.
- Open-source and extensible; intended for learning, prototyping, and local model experimentation.

Requirements & important notes
- Windows 11 or later.
- For local Ollama models: install and run a local Ollama server before using Ollama models.
- For OpenAI-compatible providers: provide a valid endpoint and API key via Settings.
- Conversations and API keys are stored locally on your device. If you connect to a remote provider, your messages will be transmitted to that provider and handled according to their policies. Review provider privacy terms before connecting.
- Experimental: uses preview/alpha libraries and intended for developer experimentation.

Get started
Open Settings, choose a provider, enter endpoint/API key/model, tune parameters, and start a new conversation. Responses stream back as they are generated.

Privacy
All conversation data is saved locally by default. For a full privacy policy, visit the provided Privacy Policy URL in this listing.

Support
Report issues or request help via the project repository linked on this listing.