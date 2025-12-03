namespace Celeste.Mod.ChatInputBox;

// TODO support scrolling
public sealed class ChatMessageListView<TChatMessage> where TChatMessage : IChatMessage
{
    private record struct ChatItem(TChatMessage Message, float ShowTimer, float FadeOut = 1f);
    private readonly List<ChatItem> chatLog;
    private readonly ITextRenderer textRenderer;

    public int MaxCount { get; set; } = 12;

    public float ShowDuration { get; set; } = 2f;

    public bool AlwaysShow { get; set; }

    public ChatMessageListView(ITextRenderer textRenderer)
    {
        this.textRenderer = textRenderer;
        chatLog = new();
    }

    public void AddChatMessage(TChatMessage chatMessage)
    {
        chatLog.Add(new(chatMessage, ShowDuration));
    }

    public void CleanUp()
    {
        chatLog.Clear();
    }

    public void Update()
    {
        for (int i = chatLog.Count - 1; i >= 0; i--)
        {
            var item = chatLog[i];
            if (item.ShowTimer <= 0f)
            {
                if (item.FadeOut <= 0f)
                {
                    break;
                }
                else
                {
                    const float DisappearDuration = 0.25f;
                    item.FadeOut -= (1f / DisappearDuration) * Engine.RawDeltaTime;
                    if (item.FadeOut < 0f)
                        item.FadeOut = 0f;
                }
            }
            else
            {
                item.ShowTimer -= Engine.RawDeltaTime;
            }
            chatLog[i] = item;
        }
    }

    public void Render()
    {
        //const float LinePadding = 2f;
        const float Margin = 16f;
        const float Padding = 8f;

        Vector2 baseLoc = new Vector2(Margin, Engine.Height - Margin - 64f);

        float curY = baseLoc.Y - Padding;
        foreach (var (msg, _, msgFade) in Enumerable.Reverse(chatLog).Take(MaxCount))
        {
            float fade = msgFade;
            if (AlwaysShow)
            {
                fade = 1f;
            }
            else if (fade <= 0f)
                break;

            float height = textRenderer.LineHeight;
            float lineWidth = Engine.Width / 3f * 2f;
            SnappedRect(
                baseLoc.X - Padding,
                curY - height,
                lineWidth + 2 * Padding,
                height,
                ColorWithFade(Color.Black, fade * 0.5f)
            );

            float curX = baseLoc.X + Padding;
            if (msg.Sender is not null)
            {
                textRenderer.Draw(
                    msg.Sender,
                    new Vector2(curX, curY),
                    new Vector2(0f, 1f),
                    ColorWithFade(msg.SenderColor, fade)
                );
                curX += textRenderer.Measure(msg.Sender).X;
                const string Separator = ": ";
                textRenderer.Draw(
                    Separator,
                    new Vector2(curX, curY),
                    new Vector2(0f, 1f),
                    ColorWithFade(Color.White, fade)
                );
                curX += textRenderer.Measure(Separator).X;
            }
            textRenderer.Draw(
                msg.Content,
                new Vector2(curX, curY),
                new Vector2(0f, 1f),
                ColorWithFade(msg.Color, fade)
            );

            curY -= textRenderer.LineHeight;
        }

        static Color ColorWithFade(Color color, float fade)
            => color with { A = (byte)(color.A * fade) };

        static void SnappedRect(float x, float y, float width, float height, Color color)
        {
            float xi = MathF.Floor(x);
            float yi = MathF.Floor(y);
            float wi = MathF.Floor(x + width) - xi;
            float hi = MathF.Floor(y + height) - yi;

            Draw.Rect(xi, yi, wi, hi, color);
        }
    }
}