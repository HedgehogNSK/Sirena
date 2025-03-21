using RxTelegram.Bot.Interface.BaseTypes;
using System.Globalization;

namespace Hedgey.Telegram.Bot;
public interface IRequestContext
{
    /// <summary>
    /// Returns only the parameters sent with the command, excluding the command name.
    /// </summary>
    /// <returns>Parameters as a string.</returns>
    string GetArgsString();
    /// <summary>
    /// Returns the chat that was the source of the request.
    /// </summary>
    /// <returns>The chat object associated with the request.</returns>
    Chat GetChat();
    /// <summary>
    /// Returns only the command name without the slash (e.g., "start" instead of "/start").
    /// </summary>
    /// <returns>Command name as a string.</returns>
    string GetCommandName();
    /// <summary>
    /// Returns the culture information of the user, which can be used for localization.
    /// </summary>
    /// <returns>The user's culture information.</returns>
    CultureInfo GetCultureInfo();
    /// <summary>
    /// Returns a message object associated with the request.
    /// </summary>
    /// <returns>The message object (if any).</returns>
    Message GetMessage();
    /// <summary>
    /// Returns the raw query sent by the user, either as text or via callback.
    /// </summary>
    /// <returns>Raw query string.</returns>
    string GetQuery();
    /// <summary>
    /// Returns the chat ID that is the target for the bot's processing result.
    /// </summary>
    /// <returns>The target chat's ID.</returns>
    long GetTargetChatId();
    /// <summary>
    /// Returns the User object that initiated the request.
    /// </summary>
    /// <returns>The user object associated with the request.</returns>
    User GetUser();
}