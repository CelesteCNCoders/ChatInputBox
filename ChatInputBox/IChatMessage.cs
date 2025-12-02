namespace Celeste.Mod.ChatInputBox;

public interface IChatMessage
{
    public string? Sender { get; }

    public Color SenderColor { get; }

    public string Content { get; }

    public Color Color { get; }
}