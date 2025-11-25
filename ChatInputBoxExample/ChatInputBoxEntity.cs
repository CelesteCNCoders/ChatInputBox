using Celeste.Mod.ChatInputBox;
using Microsoft.Xna.Framework.Input;
namespace Celeste.Mod.ChatInputBoxExample;

[Tracked]
public sealed class ChatInputBoxEntity : Entity
{
    private readonly InputBox inputBox;
    private readonly ChatMessageListView msgList;
    private readonly List<ChatMessage> messages;

    public ChatInputBoxEntity(List<ChatMessage> messages)
    {
        Tag |= Tags.HUD | Tags.PauseUpdate | Tags.FrozenUpdate | Tags.TransitionUpdate;
        Language lang = Dialog.Languages["schinese"];
        TextRenderer r = new(lang)
        {
            Scale = 2f / 3f
        };
        inputBox = new(r);
        msgList = new(r);
        this.messages = messages;
    }

    public override void Added(Scene scene)
    {
        base.Added(scene);
        inputBox.Active();
    }

    public override void Removed(Scene scene)
    {
        base.Removed(scene);
        inputBox.CleanUp();
    }

    public override void SceneEnd(Scene scene)
    {
        base.SceneEnd(scene);
        Removed(scene);
    }

    public override void Update()
    {
        inputBox.Update();
        if(MInput.Keyboard.Pressed(Keys.Enter))
        {
            messages.Add(new ChatMessage("sapcc", inputBox.Text));
            RemoveSelf();
        }
    }

    public override void Render()
    {
        inputBox.Render();
        msgList.Render(messages);
    }
}
