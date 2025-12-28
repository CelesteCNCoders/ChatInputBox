using Celeste.Mod.ChatInputBox;
using Microsoft.Xna.Framework.Input;

namespace Celeste.Mod.ChatInputBoxExample;

public sealed class ChatInputBoxExampleModule : EverestModule
{
    private static readonly List<ChatText> msgs =
    [
        ChatText.Parse(@"\esap\r: \uHello!", Color.White),
        ChatText.Parse(@"\evoidsd\r: \sI'm yoidsd", Color.White),
        ChatText.Parse(@"\ebot:\r \bHa! Welcome to MiaoNet!", Color.White),
        ChatText.Parse(@"\etest:\r \o\0Outlined\r\uUnderscored\sAlsoStrikeThrough\r\#badff4Color of #badff4\\Escape", Color.White),
    ];

    public static ChatInputBoxExampleModule Instance { get; private set; }

    public override void Load()
    {
        Instance = this;
        Everest.Events.Level.OnAfterUpdate += Level_OnAfterUpdate;
    }

    public override void Unload()
    {
        Everest.Events.Level.OnAfterUpdate -= Level_OnAfterUpdate;
    }

    private static void Level_OnAfterUpdate(Level level)
    {
        if (MInput.Keyboard.Pressed(Keys.T))
        {
            var entity = level.Tracker.GetEntity<ChatInputBoxEntity>();
            if (entity is null)
                level.Add(new ChatInputBoxEntity(msgs));
        }
        else if (MInput.Keyboard.Pressed(Keys.Escape))
        {
            // TODO any better way?
            MInput.VirtualInputs.ForEach(i => (i as VirtualButton)?.ConsumePress());
            level.Tracker.GetEntity<ChatInputBoxEntity>()?.RemoveSelf();
        }
    }
}