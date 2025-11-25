namespace Celeste.Mod.ChatInputBox;

public interface IChatMessage
{
    public string Sender { get; }

    public Color SenderColor { get; }

    public string Text { get; }

    public Color Color { get; }
}