using Celeste.Mod.ChatInputBox;
using Microsoft.Xna.Framework.Input;
namespace Celeste.Mod.ChatInputBoxExample;

[Tracked]
public sealed class ChatInputBoxEntity : Entity
{
    private readonly InputBox inputBox;
    private readonly ChatMessageListView msgListView;

    public ChatInputBoxEntity(List<ChatText> messages)
    {
        Tag |= Tags.HUD | Tags.PauseUpdate | Tags.FrozenUpdate | Tags.TransitionUpdate;
        Language lang = Dialog.Languages["schinese"];
        TextRenderer r = new(lang)
        {
            Scale = 2f / 3f
        };
        inputBox = new(r);
        msgListView = new(r);
        foreach (var item in messages)
            msgListView.AddChatMessage(item);
    }

    public override void Added(Scene scene)
    {
        base.Added(scene);
        inputBox.Active();
        scene.Paused = true;
    }

    public override void Removed(Scene scene)
    {
        base.Removed(scene);
        inputBox.Deactive();
        scene.Paused = false;
    }

    public override void SceneEnd(Scene scene)
    {
        base.SceneEnd(scene);
        Removed(scene);
    }

    public override void Update()
    {
        inputBox.Update();
        msgListView.Update();
        if (MInput.Keyboard.Pressed(Keys.Enter))
            RemoveSelf();
    }

    public override void Render()
    {
        inputBox.Render();
        msgListView.Render();
    }
}
