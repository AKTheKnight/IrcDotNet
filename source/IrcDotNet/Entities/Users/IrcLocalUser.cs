﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using IrcDotNet.Entities.Channels;
using IrcDotNet.Entities.Servers;
using IrcDotNet.Interfaces;
using IrcDotNet.Utilities;

#if !NET9_0_OR_GREATER
    using IrcDotNet.Collections;
#else
#endif

    namespace IrcDotNet.Entities.Users;

    /// <summary>
    ///     Represents the local user of a specific <see cref="IrcClient" />.
    ///     The local user is the user as which the client has connected and registered, and may be either a normal user or
    ///     service.
    /// </summary>
    /// <threadsafety static="true" instance="false" />
    [DebuggerDisplay("{ToString(), nq} (local)")]
    public class IrcLocalUser : IrcUser, IIrcMessageSendHandler, IIrcMessageReceiveHandler, IIrcMessageReceiver
    {
        // True if local user is service; false, if local user is normal user.

        // Collection of current modes of user.
        private readonly HashSet<char> modes;

        internal IrcLocalUser(string nickName, string distribution, string description)
            : base(true, nickName, null, null)
        {
            IsService = true;
            modes = new HashSet<char>();
            Modes = new ReadOnlySet<char>(modes);
            ServiceDistribution = distribution;
            ServiceDescription = description;
        }

        internal IrcLocalUser(string nickName, string userName, string realName, IEnumerable<char> modes = null)
            : base(true, nickName, userName, realName)
        {
            IsService = false;
            this.modes = new HashSet<char>();
            Modes = new ReadOnlySet<char>(this.modes);
            if (modes is not null)
                this.modes.UnionWith(modes);
        }

        /// <summary>
        ///     Gets whether the local user is a service or normal user.
        /// </summary>
        /// <value>
        ///     <see langword="true" /> if the user is a service; <see langword="false" />, if the user is a normal
        ///     user.
        /// </value>
        public bool IsService { get; }

        /// <summary>
        ///     Gets a read-only collection of the modes the user currently has.
        /// </summary>
        /// <value>The current modes of the user.</value>
        public ReadOnlySet<char> Modes { get; }

        /// <summary>
        ///     Gets the distribution of the service, which determines its visibility to users on specific servers.
        /// </summary>
        /// <value>
        ///     A wildcard expression for matching against the names of servers on which the service should be
        ///     visible.
        /// </value>
        public string ServiceDistribution { get; }

        /// <summary>
        ///     Gets the distribution of the service, which determines its visibility to users on specific servers.
        /// </summary>
        /// <value>
        ///     A wildcard expression for matching against the names of servers on which the service should be
        ///     visible.
        /// </value>
        public string ServiceDescription { get; }

        /// <summary>
        ///     Occurs when the local user has received a message.
        /// </summary>
        public event EventHandler<IrcMessageEventArgs> MessageReceived;

        /// <summary>
        ///     Occurs when the local user has received a notice.
        /// </summary>
        public event EventHandler<IrcMessageEventArgs> NoticeReceived;

        /// <summary>
        ///     Occurs when the modes of the local user have changed.
        /// </summary>
        public event EventHandler<EventArgs> ModesChanged;

        /// <summary>
        ///     Occurs when the local user has joined a channel.
        /// </summary>
        public event EventHandler<IrcChannelEventArgs> JoinedChannel;

        /// <summary>
        ///     Occurs when the local user has left a channel.
        /// </summary>
        public event EventHandler<IrcChannelEventArgs> LeftChannel;

        /// <summary>
        ///     Occurs when the local user has sent a message.
        /// </summary>
        public event EventHandler<IrcMessageEventArgs> MessageSent;

        /// <summary>
        ///     Occurs when the local user has received a message, before the <see cref="MessageReceived" /> event.
        /// </summary>
        public event EventHandler<IrcPreviewMessageEventArgs> PreviewMessageReceived;

        /// <summary>
        ///     Occurs when the local user has sent a notice.
        /// </summary>
        public event EventHandler<IrcMessageEventArgs> NoticeSent;

        /// <summary>
        ///     Occurs when the local user has received a notice, before the <see cref="NoticeReceived" /> event.
        /// </summary>
        public event EventHandler<IrcPreviewMessageEventArgs> PreviewNoticeReceived;

        /// <inheritdoc cref="SendMessage(IEnumerable{IIrcMessageTarget}, string)" />
        /// <param name="target">The <see cref="IIrcMessageTarget" /> to which to send the message.</param>
        public void SendMessage(IIrcMessageTarget target, string text)
        {
            ArgumentNullException.ThrowIfNull(target);
            ArgumentNullException.ThrowIfNull(text);

            SendMessage(new[] {target}, text);
        }

        /// <inheritdoc cref="SendMessage(IEnumerable{string}, string, Encoding)" />
        /// <summary>
        ///     <inheritdoc cref="SendMessage(IEnumerable{string}, string, Encoding)" select="/summary/node()" />
        ///     A message target may be an <see cref="IrcUser" />, <see cref="IrcChannel" />, or <see cref="IrcTargetMask" />.
        /// </summary>
        /// <param name="targets">A collection of targets to which to send the message.</param>
        public void SendMessage(IEnumerable<IIrcMessageTarget> targets, string text)
        {
            ArgumentNullException.ThrowIfNull(targets);
            ArgumentNullException.ThrowIfNull(text);

            SendMessage(targets.Select(t => t.Name), text);
        }

        /// <inheritdoc cref="SendMessage(IEnumerable{string}, string, Encoding)" />
        /// <param name="target">The name of the target to which to send the message.</param>
        public void SendMessage(string target, string text)
        {
            ArgumentNullException.ThrowIfNull(target);
            ArgumentNullException.ThrowIfNull(text);

            SendMessage(new[] {target}, text);
        }

        /// <summary>
        ///     Sends a message to the specified target.
        /// </summary>
        /// <param name="targets">A collection of the names of targets to which to send the message.</param>
        /// <param name="text">The ASCII-encoded text of the message to send.</param>
        /// <param name="encoding">The encoding in which to send the value of <paramref name="text" />.</param>
        /// <exception cref="ArgumentNullException"><paramref name="targets" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="text" /> is <see langword="null" />.</exception>
        public void SendMessage(IEnumerable<string> targets, string text, Encoding encoding = null)
        {
            ArgumentNullException.ThrowIfNull(targets);
            ArgumentNullException.ThrowIfNull(text);

            Client.SendPrivateMessage(targets, text.ChangeEncoding(Client.TextEncoding, encoding));
        }

        /// <inheritdoc cref="SendNotice(IEnumerable{IIrcMessageTarget}, string)" />
        /// <param name="target">The <see cref="IIrcMessageTarget" /> to which to send the notice.</param>
        public void SendNotice(IIrcMessageTarget target, string text)
        {
            ArgumentNullException.ThrowIfNull(target);
            ArgumentNullException.ThrowIfNull(text);

            SendNotice(new[] {target}, text);
        }

        /// <inheritdoc cref="SendNotice(IEnumerable{string}, string, Encoding)" />
        /// <summary>
        ///     <inheritdoc cref="SendNotice(IEnumerable{string}, string, Encoding)" select="/summary/node()" />
        ///     A message target may be an <see cref="IrcUser" />, <see cref="IrcChannel" />, or <see cref="IrcTargetMask" />.
        /// </summary>
        /// <param name="targets">A collection of targets to which to send the notice.</param>
        public void SendNotice(IEnumerable<IIrcMessageTarget> targets, string text)
        {
            ArgumentNullException.ThrowIfNull(targets);
            ArgumentNullException.ThrowIfNull(text);

            SendNotice(targets.Select(t => t.Name), text);
        }

        /// <inheritdoc cref="SendNotice(IEnumerable{string}, string, Encoding)" />
        /// <param name="target">The name of the target to which to send the notice.</param>
        public void SendNotice(string target, string text)
        {
            ArgumentNullException.ThrowIfNull(target);
            ArgumentNullException.ThrowIfNull(text);

            SendNotice(new[] {target}, text);
        }

        /// <summary>
        ///     Sends a notice to the specified target.
        /// </summary>
        /// <param name="targets">A collection of the names of targets to which to send the notice.</param>
        /// <param name="text">The ASCII-encoded text of the notice to send.</param>
        /// <param name="encoding">The encoding in which to send the value of <paramref name="text" />.</param>
        /// <exception cref="ArgumentNullException"><paramref name="targets" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="text" /> is <see langword="null" />.</exception>
        public void SendNotice(IEnumerable<string> targets, string text, Encoding encoding = null)
        {
            ArgumentNullException.ThrowIfNull(targets);
            ArgumentNullException.ThrowIfNull(text);

            Client.SendNotice(targets, text.ChangeEncoding(Client.TextEncoding, encoding));
        }

        /// <summary>
        ///     Sets the nick name of the local user to the specified text.
        /// </summary>
        /// <param name="nickName">The new nick name of the local user.</param>
        /// <exception cref="ArgumentNullException"><paramref name="nickName" /> is <see langword="null" />.</exception>
        public void SetNickName(string nickName)
        {
            ArgumentNullException.ThrowIfNull(nickName);

            Client.SetNickName(nickName);
        }

        /// <summary>
        ///     Sets the local user as away, giving the specified message.
        /// </summary>
        /// <param name="text">The text of the response sent to a user when they try to message you while away.</param>
        /// <exception cref="ArgumentNullException"><paramref name="text" /> is <see langword="null" />.</exception>
        public void SetAway(string text)
        {
            ArgumentNullException.ThrowIfNull(text);

            Client.SetAway(text);
        }

        /// <summary>
        ///     Sets the local user as here (no longer away).
        /// </summary>
        public void UnsetAway()
        {
            Client.UnsetAway();
        }

        /// <summary>
        ///     Requests a list of the current modes of the user.
        /// </summary>
        public void GetModes()
        {
            Client.GetLocalUserModes(this);
        }

        /// <inheritdoc cref="SetModes(IEnumerable{char})" />
        public void SetModes(params char[] newModes)
        {
            SetModes((IEnumerable<char>) newModes);
        }

        /// <inheritdoc cref="SetModes(string)" />
        /// <param name="newModes">
        ///     A collection of mode characters that should become the new modes.
        ///     Any modes in the collection that are not currently set will be set, and any nodes not in the collection that
        ///     are currently set will be unset.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="newModes" /> is <see langword="null" />.</exception>
        public void SetModes(IEnumerable<char> newModes)
        {
            ArgumentNullException.ThrowIfNull(newModes);

            lock (((ICollection) Modes).SyncRoot)
                SetModes(newModes.Except(modes), modes.Except(newModes));
        }

        /// <inheritdoc cref="SetModes(string)" />
        /// <exception cref="ArgumentNullException"><paramref name="setModes" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="unsetModes" /> is <see langword="null" />.</exception>
        public void SetModes(IEnumerable<char> setModes, IEnumerable<char> unsetModes)
        {
            ArgumentNullException.ThrowIfNull(setModes);
            ArgumentNullException.ThrowIfNull(unsetModes);

            SetModes("+" + string.Join(string.Empty, setModes) + "-" + string.Join(string.Empty, unsetModes));
        }

        /// <summary>
        ///     Sets the specified modes on the local user.
        /// </summary>
        /// <param name="modes">
        ///     The mode string that specifies mode changes, which takes the form
        ///     `( "+" / "-" ) *( mode character )`.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="modes" /> is <see langword="null" />.</exception>
        public void SetModes(string modes)
        {
            ArgumentNullException.ThrowIfNull(modes);

            Client.SetLocalUserModes(this, modes);
        }

        internal void HandleModesChanged(string newModes)
        {
            lock (((ICollection) Modes).SyncRoot)
                modes.UpdateModes(newModes);

            OnModesChanged(EventArgs.Empty);
        }

        internal void HandleJoinedChannel(IrcMessage ircMessage, IrcChannel channel)
        {
            OnJoinedChannel(new IrcChannelEventArgs(ircMessage, channel, null));
        }

        internal void HandleLeftChannel(IrcMessage ircMessage, IrcChannel channel)
        {
            OnLeftChannel(new IrcChannelEventArgs(ircMessage, channel, null));
        }

        internal void HandleMessageSent(IrcMessage ircMessage, IList<IIrcMessageTarget> targets, string text)
        {
            OnMessageSent(new IrcMessageEventArgs(ircMessage, this, targets, text, Client.TextEncoding));
        }

        internal void HandleNoticeSent(IrcMessage ircMessage, IList<IIrcMessageTarget> targets, string text)
        {
            OnNoticeSent(new IrcMessageEventArgs(ircMessage, this, targets, text, Client.TextEncoding));
        }

        internal void HandleMessageReceived(IrcMessage ircMessage, IIrcMessageSource source, IList<IIrcMessageTarget> targets, string text)
        {
            var previewEventArgs = new IrcPreviewMessageEventArgs(ircMessage, source, targets, text, Client.TextEncoding);
            OnPreviewMessageReceived(previewEventArgs);
            if (!previewEventArgs.Handled)
                OnMessageReceived(new IrcMessageEventArgs(ircMessage, source, targets, text, Client.TextEncoding));
        }

        internal void HandleNoticeReceived(IrcMessage ircMessage, IIrcMessageSource source, IList<IIrcMessageTarget> targets, string text)
        {
            var previewEventArgs = new IrcPreviewMessageEventArgs(ircMessage, source, targets, text, Client.TextEncoding);
            OnPreviewNoticeReceived(previewEventArgs);
            if (!previewEventArgs.Handled)
                OnNoticeReceived(new IrcMessageEventArgs(ircMessage, source, targets, text, Client.TextEncoding));
        }

        /// <summary>
        ///     Raises the <see cref="ModesChanged" /> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected virtual void OnModesChanged(EventArgs e)
        {
            var handler = ModesChanged;
            if (handler is not null)
                handler(this, e);
        }

        /// <summary>
        ///     Raises the <see cref="JoinedChannel" /> event.
        /// </summary>
        /// <param name="e">The <see cref="IrcChannelEventArgs" /> instance containing the event data.</param>
        protected virtual void OnJoinedChannel(IrcChannelEventArgs e)
        {
            var handler = JoinedChannel;
            if (handler is not null)
                handler(this, e);
        }

        /// <summary>
        ///     Raises the <see cref="LeftChannel" /> event.
        /// </summary>
        /// <param name="e">The <see cref="IrcChannelEventArgs" /> instance containing the event data.</param>
        protected virtual void OnLeftChannel(IrcChannelEventArgs e)
        {
            var handler = LeftChannel;
            if (handler is not null)
                handler(this, e);
        }

        /// <summary>
        ///     Raises the <see cref="MessageSent" /> event.
        /// </summary>
        /// <param name="e">The <see cref="IrcMessageEventArgs" /> instance containing the event data.</param>
        protected virtual void OnMessageSent(IrcMessageEventArgs e)
        {
            var handler = MessageSent;
            if (handler is not null)
                handler(this, e);
        }

        /// <summary>
        ///     Raises the <see cref="MessageReceived" /> event.
        /// </summary>
        /// <param name="e">The <see cref="IrcMessageEventArgs" /> instance containing the event data.</param>
        protected virtual void OnMessageReceived(IrcMessageEventArgs e)
        {
            var handler = MessageReceived;
            if (handler is not null)
                handler(this, e);
        }

        /// <summary>
        ///     Raises the <see cref="PreviewMessageReceived" /> event.
        /// </summary>
        /// <param name="e">The <see cref="IrcPreviewMessageEventArgs" /> instance containing the event data.</param>
        protected virtual void OnPreviewMessageReceived(IrcPreviewMessageEventArgs e)
        {
            var handler = PreviewMessageReceived;
            if (handler is not null)
                handler(this, e);
        }

        /// <summary>
        ///     Raises the <see cref="NoticeSent" /> event.
        /// </summary>
        /// <param name="e">The <see cref="IrcMessageEventArgs" /> instance containing the event data.</param>
        protected virtual void OnNoticeSent(IrcMessageEventArgs e)
        {
            var handler = NoticeSent;
            if (handler is not null)
                handler(this, e);
        }

        /// <summary>
        ///     Raises the <see cref="PreviewNoticeReceived" /> event.
        /// </summary>
        /// <param name="e">The <see cref="IrcPreviewMessageEventArgs" /> instance containing the event data.</param>
        protected virtual void OnPreviewNoticeReceived(IrcPreviewMessageEventArgs e)
        {
            var handler = PreviewNoticeReceived;
            if (handler is not null)
                handler(this, e);
        }

        /// <summary>
        ///     Raises the <see cref="NoticeReceived" /> event.
        /// </summary>
        /// <param name="e">The <see cref="IrcMessageEventArgs" /> instance containing the event data.</param>
        protected virtual void OnNoticeReceived(IrcMessageEventArgs e)
        {
            var handler = NoticeReceived;
            if (handler is not null)
                handler(this, e);
        }

        #region IIrcMessageSendHandler Members

        void IIrcMessageSendHandler.HandleMessageSent(IrcMessage ircMessage, IList<IIrcMessageTarget> targets, string text)
        {
            HandleMessageSent(ircMessage, targets, text);
        }

        void IIrcMessageSendHandler.HandleNoticeSent(IrcMessage ircMessage, IList<IIrcMessageTarget> targets, string text)
        {
            HandleNoticeSent(ircMessage, targets, text);
        }

        #endregion

        #region IIrcMessageReceiveHandler Members

        void IIrcMessageReceiveHandler.HandleMessageReceived(IrcMessage ircMessage, IIrcMessageSource source, IList<IIrcMessageTarget> targets,
            string text)
        {
            HandleMessageReceived(ircMessage, source, targets, text);
        }

        void IIrcMessageReceiveHandler.HandleNoticeReceived(IrcMessage ircMessage, IIrcMessageSource source, IList<IIrcMessageTarget> targets,
            string text)
        {
            HandleNoticeReceived(ircMessage, source, targets, text);
        }

        #endregion
    }
