BigChat Privacy Policy
Effective date: November 13, 2025

1. Introduction
BigChat is a desktop WinUI 3 application for Windows that provides a local UI for interacting with large language models. This Privacy Policy explains what information BigChat collects, how it is used and stored, and what choices you have. This policy applies only to BigChat (the application) and not to third‑party model providers you may configure.

2. Short summary
- BigChat does not collect or transmit telemetry, analytics, or crash reports to the app author by default.
- Conversations, settings and any API keys you enter are stored locally on your device.
- If you configure a remote AI provider (for example an OpenAI‑compatible endpoint), the content you send will be transmitted to that provider and handled under that provider’s privacy policy.

3. Information BigChat collects and stores
- Local conversation history: Conversations and message content you create in BigChat are stored locally in a SQLite database in application data. This enables history, rename/delete, and offline access.
- Local settings: Provider selection, endpoints, model IDs, and model parameters (temperature, top‑p, token limits, penalties) are stored locally in application settings.
- API keys and credentials: If you enter API keys for remote providers, those keys are stored locally in application settings. BigChat does not transmit your API keys to the developer or to Microsoft.
- No automatic telemetry: By default BigChat does not send usage analytics, crash reports, or diagnostics outside your device.

4. When BigChat transmits data to third parties
- Provider communication: BigChat acts as a client to the AI provider you select. When you use a remote provider (OpenAI‑compatible endpoint), messages and content you send are transmitted to that provider for processing.
- Local provider: If you use a local Ollama server, inference happens locally on your machine and data remains within your environment (subject to your Ollama configuration).
- Third‑party privacy: Review the privacy policies and data handling of any provider you configure (for example OpenAI, Microsoft/Azure, or other providers).

5. Purpose and lawful basis
- Primary purpose: to enable conversational interactions with the configured AI model and to persist conversation history locally.
- The app does not use your data for advertising, profiling, or marketing by the developer.

6. Retention and deletion
- Local data: Conversations and settings remain on your device until you delete them inside the app or uninstall the application. You can remove stored data by deleting conversations or clearing application data from your device.
- Third‑party retention: Data retention for remote providers is governed by those providers’ policies. Contact the provider for data removal requests related to their service.

7. Security
- Local storage: Data is stored in ApplicationData and a local SQLite database. Standard OS protections apply; secure your device and user account.
- API keys: Stored locally. Treat them like other sensitive credentials. Revoke and rotate keys at the provider if compromised.
- Network transmission: Communication with remote providers uses the transport and security mechanisms of the provider libraries (for example HTTPS/TLS). The app does not modify provider transport security.

8. Your choices and controls
- Use local-only providers: Run a local Ollama server to avoid sending data over the network.
- Do not enter sensitive personal data: Avoid sending highly sensitive personal, health, or financial information to remote providers.
- Delete data: Delete conversations inside the app or remove the app and its stored data from your device.
- Revoke API keys: If a key is compromised, revoke it with the provider and replace it in BigChat Settings.

9. Third‑party libraries and services
BigChat uses third‑party libraries and SDKs (.NET runtime, WinUI, EF Core, Microsoft.Extensions.AI, OllamaSharp, and optional provider SDKs). Those libraries may have their own data practices. The app itself does not automatically send data to those library authors. Any network transmission is from your device to the configured provider.

10. Changes to this policy
We may update this Privacy Policy. If material changes occur we will update the Effective Date above. Continued use of BigChat after changes indicates acceptance of the updated policy.

11. Contact
For questions about this Privacy Policy or to request guidance on removing local data, open an issue in the project repository or contact: bigchat.down305@passmail.net