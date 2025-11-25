namespace Celeste.Mod.ChatInputBox;

public sealed class ChatMessageListView
{
    private readonly ITextRenderer textRenderer;

    public ChatMessageListView(ITextRenderer textRenderer)
    {
        this.textRenderer = textRenderer;
    }

    public void Update()
    {

    }

    public void Render<TChatMessage>(IReadOnlyList<TChatMessage> messages)
        where TChatMessage : IChatMessage
    {
        const float Margin = 16f;
        const float Padding = 8f;

        Vector2 baseLoc = new Vector2(Margin, Engine.Height - Margin - 64f);

        float totalHeight = messages.Count * textRenderer.LineHeight;
        float paddedHeight = totalHeight + 2 * Padding;

        Draw.Rect(
            baseLoc - Vector2.UnitY * paddedHeight,
            Engine.Width / 3f * 2f + 2 * Padding,
            paddedHeight,
            Color.Black with { A = 100 }
        );

        float curY = baseLoc.Y - Padding;
        foreach (var msg in messages.Reverse())
        {
            float curX = baseLoc.X + Padding;
            textRenderer.Draw(
                msg.Sender,
                new Vector2(curX, curY),
                new Vector2(0f, 1f),
                msg.SenderColor
            );
            curX += textRenderer.Measure(msg.Sender).X;
            const string Separator = ": ";
            textRenderer.Draw(
                Separator,
                new Vector2(curX, curY),
                new Vector2(0f, 1f),
                Color.White
            );
            curX += textRenderer.Measure(Separator).X;
            textRenderer.Draw(
                msg.Text,
                new Vector2(curX, curY),
                new Vector2(0f, 1f),
                msg.Color
            );

            curY -= textRenderer.LineHeight;
        }
    }
}