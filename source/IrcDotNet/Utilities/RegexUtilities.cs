using System.Text.RegularExpressions;

namespace IrcDotNet.Utilities;

partial class RegexUtilities
{
    private const string RegexNickName = @"(?<nick>[^!@]+)";
    private const string RegexUserName = @"(?<user>[^!@]+)";
    private const string RegexHostName = @"(?<host>[^%@]+)";
    private const string RegexChannelName = @"@?(?<channel>[#+!&].+)";
    private const string RegexTargetMask = @"(?<targetMask>[$#].+)";
    private const string RegexServerName = @"(?<server>[^%@]+?\.[^%@]*)";
    private const string RegexNickNameId = $@"{RegexNickName}(?:(?:!{RegexUserName})?@{RegexHostName})?";
    private const string RegexUserNameId = $@"{RegexUserName}(?:(?:%{RegexHostName})?@{RegexServerName}|%{RegexHostName})";
    private const string RegexMessagePrefix = $@"^(?:{RegexServerName}|{RegexNickNameId})$";
    private const string RegexMessageTarget =
        $@"^(?:{RegexChannelName}|{RegexUserNameId}|{RegexTargetMask}|{RegexNickNameId})$";
    
    private const string IsupportPrefix = @"\((?<modes>.*)\)(?<prefixes>.*)";
    
    [GeneratedRegex(RegexNickName)]
    public static partial Regex NickNameRegex();

    [GeneratedRegex(RegexChannelName)]
    public static partial Regex ChannelNameRegex();

    [GeneratedRegex(RegexMessagePrefix)]
    public static partial Regex MessagePrefixRegex();
    
    [GeneratedRegex(RegexMessageTarget)]
    public static partial Regex MessageTargetRegex();
    
    [GeneratedRegex(IsupportPrefix)]
    public static partial Regex IsupportPrefixRegex();
    
    [GeneratedRegex(@"Current local users (?<current>\d+), max (?<max>\d+)")]
    public static partial Regex LocalUsersRegex();
    
    [GeneratedRegex(@"Current global users (?<current>\d+), max (?<max>\d+)")]
    public static partial Regex GlobalUsersRegex();
}
