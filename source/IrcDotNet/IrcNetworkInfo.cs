namespace IrcDotNet;

/// <summary>
///     Stores information about a specific IRC network.
/// </summary>
public struct IrcNetworkInfo
{
    /// <summary>
    ///     The number of visible users on the network.
    /// </summary>
    public int? VisibleUsersCount;

    /// <summary>
    ///     The number of invisible users on the network.
    /// </summary>
    public int? InvisibleUsersCount;

    /// <summary>
    ///     The number of servers in the network.
    /// </summary>
    public int? ServersCount;

    /// <summary>
    ///     The number of operators on the network.
    /// </summary>
    public int? OperatorsCount;

    /// <summary>
    ///     The number of unknown connections to the network.
    /// </summary>
    public int? UnknownConnectionsCount;

    /// <summary>
    ///     The number of channels that currently exist on the network.
    /// </summary>
    public int? ChannelsCount;

    /// <summary>
    ///     The number of clients connected to the server.
    ///     This should be the same as <see cref="CurrentLocalUsersCount"/>, but they are kept separate for clarity.
    /// </summary>
    public int? ServerClientsCount;

    /// <summary>
    ///     The number of others servers connected to the server.
    /// </summary>
    public int? ServerServersCount;

    /// <summary>
    ///     The number of services connected to the server.
    /// </summary>
    public int? ServerServicesCount;

    /// <summary>
    ///     "The number of clients currently (...) connected directly to this server".
    ///     This should be the same as <see cref="ServerClientsCount"/>, but they are kept separate for clarity.
    /// </summary>
    public int? CurrentLocalUsersCount;
    /// <summary>
    ///     "The maximum number of clients that have been connected directly to this server at one time".
    /// </summary>
    public int? MaxLocalUsersCount;
    
    /// <summary>
    ///     "The number of clients currently connected to this server, globally (directly and through other server links)."
    /// </summary>
    public int? CurrentGlobalUsersCount;
    /// <summary>
    ///     "The maximum number of clients that have been connected to this server at one time, globally (directly and through other server links)".
    /// </summary>
    public int? MaxGlobalUsersCount;
}
