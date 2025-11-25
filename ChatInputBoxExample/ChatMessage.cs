using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Celeste.Mod.ChatInputBox;

namespace Celeste.Mod.ChatInputBoxExample;

public sealed record ChatMessage(string Sender, Color SenderColor, string Text, Color Color) : IChatMessage
{
    public ChatMessage(string sender, string text)
        : this(sender, Color.Yellow, text, Color.White)
	{
	}
}