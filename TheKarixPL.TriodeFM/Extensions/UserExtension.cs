using Discord;

namespace TheKarixPL.TriodeFM.Extensions;

public static class UserExtension
{
    public static string FormatUsername(this IUser user)
    {
        if (user == null)
            return "null";
        return user.Username + "#" + user.Discriminator;
    }
}