namespace Celeste.Mod.ChatInputBox;

// TODO support scrolling
public sealed class ChatMessageListView
{
    private record struct ChatItem(ChatText Message, float ShowTimer, float FadeOut = 1f);
    private readonly List<ChatItem> chatLog;
    private readonly ITextRenderer textRenderer;

    public int MaxCount { get; set; } = 12;

    public float ShowDuration { get; set; } = 8f;

    public bool AlwaysShow { get; set; }

    public ChatMessageListView(ITextRenderer textRenderer)
    {
        this.textRenderer = textRenderer;
        chatLog = new();
    }

    public void AddChatMessage(ChatText chatMessage)
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
        const float Margin = 16f;
        const float Padding = 8f;

        Vector2 baseLoc = new Vector2(Margin, Engine.Height - Margin - textRenderer.LineHeight * 1.5f);

        float curY = baseLoc.Y - Padding;
        foreach (var (msg, _, msgFade) in Enumerable.Reverse(chatLog).Take(MaxCount))
        {
            float fade = msgFade;
            if (AlwaysShow)
                fade = 1f;
            else if (fade <= 0f)
                break;

            float lineHeight = textRenderer.LineHeight;
            float lineWidth = Engine.Width / 5f * 4f;
            DrawSnappedRect(
                baseLoc.X,
                curY - lineHeight,
                lineWidth + 2 * Padding,
                lineHeight,
                ColorWithFade(Color.Black, fade * 0.5f)
            );

            float curX = baseLoc.X + Padding;
            foreach (var seg in msg.Segments)
            {
                Vector2 size = textRenderer.Measure(seg.Text);

                if (!seg.Style.HasFlag(ChatTextStyle.Outline))
                {
                    textRenderer.Draw(
                        seg.Text,
                        new Vector2(curX, curY),
                        new Vector2(0f, 1f),
                        ColorWithFade(seg.Color, fade)
                    );
                }
                else
                {
                    textRenderer.DrawOutline(
                        seg.Text,
                        new Vector2(curX, curY),
                        new Vector2(0f, 1f),
                        ColorWithFade(seg.Color, fade)
                    );
                }

                if (seg.Style.HasFlag(ChatTextStyle.Underscore))
                {
                    Draw.Line(
                        new Vector2(curX, curY),
                        new Vector2(curX + size.X, curY),
                        seg.Color
                    );
                }

                if (seg.Style.HasFlag(ChatTextStyle.Strikethrough))
                {
                    Draw.Line(
                        new Vector2(curX, curY - lineHeight / 2f),
                        new Vector2(curX + size.X, curY - lineHeight / 2),
                        seg.Color
                    );
                }

                curX += size.X;
            }

            curY -= textRenderer.LineHeight;
        }

        static Color ColorWithFade(Color color, float fade)
            => color with { A = (byte)(color.A * fade) };

        static void DrawSnappedRect(float x, float y, float width, float height, Color color)
        {
            float xi = MathF.Floor(x);
            float yi = MathF.Floor(y);
            float wi = MathF.Floor(x + width) - xi;
            float hi = MathF.Floor(y + height) - yi;

            Draw.Rect(xi, yi, wi, hi, color);
        }
    }
}