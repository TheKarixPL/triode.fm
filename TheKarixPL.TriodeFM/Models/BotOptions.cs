namespace TheKarixPL.TriodeFM.Models;

public sealed class BotOptions
{
    public const string SectionName = "Bot";

    public string Token { get; set; }
    public string Prefix { get; set; }
    public string EmbedColor { get; set; }
}