namespace Celeste.Mod.ChatInputBox;

public sealed class InputBox
{
    public const float CaretBlinkInterval = 0.5f;

    private readonly ITextRenderer textRenderer;
    private readonly TextBuffer buffer;
    private bool showCaret = true;
    private float caretTimer = CaretBlinkInterval;

    public string Text => buffer.Text;

    public InputBox(ITextRenderer textRenderer)
    {
        buffer = new();
        this.textRenderer = textRenderer;
    }

    public void Active()
        => TextInput.OnInput += OnCharInput;

    public void CleanUp()
    {
        TextInput.OnInput -= OnCharInput;
        buffer.Clear();
    }

    public void Update()
    {
        if (Input.MenuRight.Pressed)
        {
            Input.MenuRight.ConsumePress();
            if (buffer.ForwardCaret())
                SetAlwaysShowCaretTimer();
        }
        else if (Input.MenuLeft.Pressed)
        {
            Input.MenuLeft.ConsumePress();
            if (buffer.BackwardCaret())
                SetAlwaysShowCaretTimer();
        }

        if (caretTimer > 0f)
        {
            caretTimer -= Engine.RawDeltaTime;
        }
        else
        {
            caretTimer = CaretBlinkInterval;
            showCaret = !showCaret;
        }
    }

    private void OnCharInput(char chr)
    {
        bool operated = false;
        if (char.IsControl(chr))
        {
            switch (chr)
            {
            case (char)8: operated = buffer.Backspace(); break; // backspace
            case (char)2: operated = buffer.BackwardToHomeCaret(); break; // home
            case (char)3: operated = buffer.ForwardToEndCaret(); break; // end
            case (char)127: operated = buffer.Delete(); break; // delete
            }

        }
        else
        {
            // TODO need we support surrogate pair?

            if (textRenderer.CanRender(chr))
            {
                buffer.InputChar(chr);
                operated = true;
            }
        }
        if (operated)
            SetAlwaysShowCaretTimer();
    }

    private void SetAlwaysShowCaretTimer()
    {
        showCaret = true;
        caretTimer = CaretBlinkInterval;
    }

    public void Render()
    {
        const float Margin = 16f;
        const float Padding = 8f;

        Vector2 baseLoc = new Vector2(Margin, Engine.Height - Margin);
        Vector2 textBaseLoc = baseLoc + new Vector2(Padding, -Padding);

        Vector2 textSize = textRenderer.Measure(buffer.Text);

        Draw.Rect(
            position: baseLoc,
            width: Math.Max(textSize.X, Engine.Width / 3f * 2f) + 2 * Padding,
            height: -(textRenderer.LineHeight + 2 * Padding),
            color: Color.Black with { A = 100 }
        );

        textRenderer.Draw(
            buffer.Text,
            textBaseLoc,
            justify: new Vector2(0f, 1f),
            color: Color.White
        );

        if (showCaret)
        {
            Vector2 beforeCaret = textRenderer.Measure(buffer.Text.Substring(0, buffer.CaretPosition));
            float width = beforeCaret.X;

            Vector2 fromLoc = textBaseLoc + new Vector2(width, 0);
            Vector2 toLoc = fromLoc - new Vector2(0f, textRenderer.LineHeight);

            Draw.Line(fromLoc, toLoc, Color.White, 2f);
        }
    }
}