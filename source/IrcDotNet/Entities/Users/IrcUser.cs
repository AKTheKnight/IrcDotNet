﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using IrcDotNet.Entities.Channels;
using IrcDotNet.Entities.Servers;
using IrcDotNet.Interfaces;

namespace IrcDotNet.Entities.Users;

/// <summary>
///     Represents an IRC user that exists on a specific <see cref="IrcClient" />.
/// </summary>
/// <threadsafety static="true" instance="false" />
[DebuggerDisplay("{ToString(), nq}")]
public class IrcUser : INotifyPropertyChanged, IIrcMessageSource, IIrcMessageTarget
{
    private IrcClient client;

    internal IrcUser(bool isOnline, string nickName, string userName, string realName)
    {
        this.NickName = nickName;
        this.UserName = userName;
        this.RealName = realName;
        this.IsOnline = isOnline;
        ServerName = null;
        ServerInfo = null;
        IsOperator = false;
        IsAway = false;
        AwayMessage = null;
        IdleDuration = TimeSpan.Zero;
        HopCount = 0;
    }

    internal IrcUser()
    {
    }

    /// <summary>
    ///     Gets whether the user is currently connected to the IRC network. This value may not be always be
    ///     up-to-date.
    /// </summary>
    /// <value>
    ///     <see langword="true" /> if the user is currently online; <see langword="false" /> if the user is
    ///     currently offline.
    /// </value>
    public bool IsOnline
    {
        get;
        internal set
        {
            field = value;
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsOnline)));
        }
    }

    /// <summary>
    ///     Gets the current nick name of the user.
    /// </summary>
    /// <value>The nick name of the user.</value>
    public string NickName
    {
        get;
        internal set
        {
            field = value;
            OnNickNameChanged(EventArgs.Empty);
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(NickName)));
        }
    }

    /// <summary>
    ///     Gets the current user name of the user. This value never changes until the user reconnects.
    /// </summary>
    /// <value>The user name of the user.</value>
    public string UserName
    {
        get;
        internal set
        {
            field = value;
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(UserName)));
        }
    }

    /// <summary>
    ///     Gets the real name of the user. This value never changes until the user reconnects.
    /// </summary>
    /// <value>The real name of the user.</value>
    public string RealName
    {
        get;
        internal set
        {
            field = value;
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(RealName)));
        }
    }

    /// <summary>
    ///     Gets the host name of the user.
    /// </summary>
    /// <value>The host name of the user.</value>
    public string HostName
    {
        get;
        internal set
        {
            field = value;
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(HostName)));
        }
    }

    /// <summary>
    ///     Gets the name of the server to which the user is connected.
    /// </summary>
    /// <value>The name of the server to which the user is connected.</value>
    public string ServerName
    {
        get;
        internal set
        {
            field = value;
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(ServerName)));
        }
    }

    /// <summary>
    ///     Gets arbitrary information about the server to which the user is connected.
    /// </summary>
    /// <value>Arbitrary information about the server.</value>
    public string ServerInfo
    {
        get;
        internal set
        {
            field = value;
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(ServerInfo)));
        }
    }

    /// <summary>
    ///     Gets whether the user is a server operator.
    /// </summary>
    /// <value><see langword="true" /> if the user is a server operator; <see langword="false" />, otherwise.</value>
    public bool IsOperator
    {
        get;
        internal set
        {
            field = value;
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsOperator)));
        }
    }

    /// <summary>
    ///     Gets whether the user has been been seen as away. This value is always up-to-date for the local user;
    ///     though it is only updated for remote users when a private message is sent to them or a Who Is response
    ///     is received for the user.
    /// </summary>
    /// <value>
    ///     <see langword="true" /> if the user is currently away; <see langword="false" />, if the user is
    ///     currently here.
    /// </value>
    public bool IsAway
    {
        get;
        internal set
        {
            field = value;
            OnIsAwayChanged(EventArgs.Empty);
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsAway)));
        }
    }

    /// <summary>
    ///     Gets the current away message received when the user was seen as away.
    /// </summary>
    /// <value>The current away message of the user.</value>
    public string AwayMessage
    {
        get;
        internal set
        {
            field = value;
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(AwayMessage)));
        }
    }

    /// <summary>
    ///     Gets the duration for which the user has been idle. This is set when a Who Is response is received.
    /// </summary>
    /// <value>The duration for which the user has been idle.</value>
    public TimeSpan IdleDuration
    {
        get;
        internal set
        {
            field = value;
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(IdleDuration)));
        }
    }

    /// <summary>
    ///     Gets the hop count of the user, which is the number of servers between the user and the server on which the
    ///     client is connected, within the network.
    /// </summary>
    /// <value>The hop count of the user.</value>
    public int HopCount
    {
        get;
        internal set
        {
            field = value;
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(HopCount)));
        }
    }

    /// <summary>
    ///     Gets the client on which the user exists.
    /// </summary>
    /// <value>The client on which the user exists.</value>
    public IrcClient Client
    {
        get { return client; }
        internal set
        {
            client = value;
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(Client)));
        }
    }

    #region IIrcMessageSource Members

    string IIrcMessageSource.Name
    {
        get { return NickName; }
    }

    #endregion

    #region IIrcMessageTarget Members

    string IIrcMessageTarget.Name
    {
        get { return NickName; }
    }

    #endregion

    /// <summary>
    ///     Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    ///     Occurs when the nick name of the user has changed.
    /// </summary>
    public event EventHandler<EventArgs> NickNameChanged;

    /// <summary>
    ///     Occurs when the user has been seen as away or here.
    /// </summary>
    public event EventHandler<EventArgs> IsAwayChanged;

    /// <summary>
    ///     Occurs when an invitation to join a channel has been received.
    /// </summary>
    /// <remarks>
    ///     This event should only be raised for the local user (the instance of <see cref="IrcLocalUser" />).
    /// </remarks>
    public event EventHandler<IrcChannelInvitationEventArgs> InviteReceived;

    /// <summary>
    ///     Occurs when the user has quit the network. This may not always be sent.
    /// </summary>
    public event EventHandler<IrcCommentEventArgs> Quit;

    /// <summary>
    ///     Sends a Who Is query to server for the user.
    /// </summary>
    public void WhoIs()
    {
        client.QueryWhoIs(NickName);
    }

    /// <summary>
    ///     Sends a Who Was query to server for the user.
    /// </summary>
    /// <param name="entriesCount">
    ///     The maximum number of entries that the server should return. A negative number
    ///     specifies an unlimited number of entries.
    /// </param>
    public void WhoWas(int entriesCount = -1)
    {
        client.QueryWhoWas(new[] {NickName}, entriesCount);
    }

    /// <summary>
    ///     Gets a collection of all channel users that correspond to the user.
    ///     Each <see cref="IrcChannelUser" /> represents a channel of which the user is currently a member.
    /// </summary>
    /// <returns>
    ///     A collection of all <see cref="IrcChannelUser" /> object that correspond to the <see cref="IrcUser" />.
    /// </returns>
    public IEnumerable<IrcChannelUser> GetChannelUsers()
    {
        // Get each channel user corresponding to this user that is member of any channel.
        foreach (var channel in client.Channels)
        {
            foreach (var channelUser in channel.Users)
            {
                if (channelUser.User == this)
                    yield return channelUser;
            }
        }
    }

    internal void HandleInviteReceived(IrcMessage ircMessage, IrcUser inviter, IrcChannel channel)
    {
        OnInviteReceived(new IrcChannelInvitationEventArgs(ircMessage, channel, inviter));
    }

    internal void HandleQuit(IrcMessage ircMessage, string comment)
    {
        foreach (var cu in GetChannelUsers().ToArray())
            cu.Channel.HandleUserQuit(cu, comment);
        OnQuit(new IrcCommentEventArgs(ircMessage, comment));
    }

    /// <summary>
    ///     Raises the <see cref="NickNameChanged" /> event.
    /// </summary>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    protected virtual void OnNickNameChanged(EventArgs e)
    {
        NickNameChanged?.Invoke(this, e);
    }

    /// <summary>
    ///     Raises the <see cref="IsAwayChanged" /> event.
    /// </summary>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    protected virtual void OnIsAwayChanged(EventArgs e)
    {
        IsAwayChanged?.Invoke(this, e);
    }

    /// <summary>
    ///     Raises the <see cref="InviteReceived" /> event.
    /// </summary>
    /// <param name="e">The <see cref="IrcChannelEventArgs" /> instance containing the event data.</param>
    protected virtual void OnInviteReceived(IrcChannelInvitationEventArgs e)
    {
        InviteReceived?.Invoke(this, e);
    }

    /// <summary>
    ///     Raises the <see cref="Quit" /> event.
    /// </summary>
    /// <param name="e">The <see cref="IrcCommentEventArgs" /> instance containing the event data.</param>
    protected virtual void OnQuit(IrcCommentEventArgs e)
    {
        Quit?.Invoke(this, e);
    }

    /// <summary>
    ///     Raises the <see cref="E:PropertyChanged" /> event.
    /// </summary>
    /// <param name="e">The <see cref="PropertyChangedEventArgs" /> instance containing the event data.</param>
    protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        PropertyChanged?.Invoke(this, e);
    }

    /// <summary>
    ///     Returns a string representation of this instance.
    /// </summary>
    /// <returns>A string that represents this instance.</returns>
    public override string ToString()
    {
        return NickName;
    }
}
