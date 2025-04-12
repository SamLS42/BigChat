namespace BigChat.Infrastructure.Settings;


public class MissingSettingsException : Exception
{
    public string? SettingName { get; set; }

    public MissingSettingsException() : base()
    {
    }

    public MissingSettingsException(string? message) : base(message)
    {
    }

    public MissingSettingsException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
