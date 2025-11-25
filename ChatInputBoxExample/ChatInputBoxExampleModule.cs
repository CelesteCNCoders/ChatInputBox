using Microsoft.Xna.Framework.Input;

namespace Celeste.Mod.ChatInputBoxExample;

public sealed class ChatInputBoxExampleModule : EverestModule
{
    private static List<ChatMessage> msgs =
    [
        new("sap", "Hello!"),
        new("voidsd", "I'm yoidsd"),
        new("bot", "Ha! Welcome to MiaoNet!")
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