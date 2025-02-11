using System;
using JetBrains.Annotations;

namespace IrcDotNet.Attributes;

// Indicates that method processes message for some protocol.
[MeansImplicitUse]
internal class MessageProcessorAttribute : Attribute
{
    public MessageProcessorAttribute(string commandName)
    {
        CommandName = commandName;
    }

    public string CommandName { get; private set; }
}
