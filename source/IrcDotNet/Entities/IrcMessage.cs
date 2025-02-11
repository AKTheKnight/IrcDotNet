using System.Collections.Generic;
using System.Diagnostics;
using IrcDotNet.Interfaces;

namespace IrcDotNet.Entities;

/// <summary>
    ///     Represents a raw IRC message that is sent/received by <see cref="IrcClient" />.
    ///     A message contains a prefix (representing the source), a command name (a word or three-digit number),
    ///     and any number of parameters (up to a maximum of 15).
    /// </summary>
    /// <seealso cref="IrcClient" />
    [DebuggerDisplay("{ToString(), nq}")]
    public struct IrcMessage
    {
        /// <summary>
        ///     The source of the message, which is the object represented by the value of <see cref="Prefix" />.
        /// </summary>
        public IIrcMessageSource Source;

        /// <summary>
        ///     The message's tags. Null if tags aren't enabled for this message.
        /// </summary>
        public IDictionary<string, string> Tags;

        /// <summary>
        ///     The message prefix.
        /// </summary>
        public string Prefix;

        /// <summary>
        ///     The name of the command.
        /// </summary>
        public string Command;

        /// <summary>
        ///     A list of the parameters to the message.
        /// </summary>
        public IList<string> Parameters;

        /// <summary>
        ///     Initializes a new instance of the <see cref="IrcMessage" /> structure.
        /// </summary>
        /// <param name="client">A client object that has sent/will receive the message.</param>
        /// <param name="prefix">The message prefix that represents the source of the message.</param>
        /// <param name="command">The command name; either an alphabetic word or 3-digit number.</param>
        /// <param name="tags">(optional) The message's tags.</param>
        /// <param name="parameters">
        ///     A list of the parameters to the message. Can contain a maximum of 15 items.
        /// </param>
        public IrcMessage(IrcClient client, string prefix, string command, IList<string> parameters,
            IDictionary<string, string> tags = null)
        {
            Prefix = prefix;
            Command = command.ToUpper();
            Parameters = parameters;
            Tags = tags;

            Source = client.GetSourceFromPrefix(prefix);
        }

        /// <summary>
        ///     Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string that represents this instance.</returns>
        public override string ToString()
        {
            return string.Format("{0} ({1} parameters)", Command, Parameters.Count);
        }
    }
