using System.Text.Json.Serialization;
using System.Windows.Media;

namespace ZenlessCompanion;

// ---------------------------------------------------------------------------
// API / storage models
// ---------------------------------------------------------------------------

/// <summary>One Signal Search exactly as returned by the official getGachaLog API.</summary>
public sealed class SignalSearchItem
{
    [JsonPropertyName("uid")]        public string Uid { get; set; } = "";
    [JsonPropertyName("gacha_type")] public string GachaType { get; set; } = "";
    [JsonPropertyName("item_id")]    public string ItemId { get; set; } = "";
    [JsonPropertyName("count")]      public string Count { get; set; } = "1";
    [JsonPropertyName("time")]       public string Time { get; set; } = "";
    [JsonPropertyName("name")]       public string Name { get; set; } = "";
    [JsonPropertyName("lang")]       public string Lang { get; set; } = "";
    [JsonPropertyName("item_type")]  public string ItemType { get; set; } = "";
    [JsonPropertyName("rank_type")]  public string RankType { get; set; } = "";
    [JsonPropertyName("id")]         public string Id { get; set; } = "";

    /// <summary>Unique key used for de-duplication (search ids are unique per account).</summary>
    [JsonIgnore] public string Key => $"{Uid}:{Id}";
}

public sealed class SignalSearchApiResponse
{
    [JsonPropertyName("retcode")] public int Retcode { get; set; }
    [JsonPropertyName("message")] public string? Message { get; set; }
    [JsonPropertyName("data")]    public SignalSearchApiData? Data { get; set; }
}

public sealed class SignalSearchApiData
{
    [JsonPropertyName("list")] public List<SignalSearchItem>? List { get; set; }
}

/// <summary>Contents of signal_search_history.json.</summary>
public sealed class SignalSearchStore
{
    public string? LastUid { get; set; }
    public DateTime? LastSyncUtc { get; set; }
    public List<SignalSearchItem> Items { get; set; } = new();
}

/// <summary>Contents of settings.json.</summary>
public sealed class AppSettings
{
    public string? GameFolder { get; set; }
    public long LastReleaseSequence { get; set; }
}

public sealed class ReleaseNote
{
    public string Title { get; init; } = "";
    public string Details { get; init; } = "";
}

// ---------------------------------------------------------------------------
// Banner definitions + dashboard view-model
// ---------------------------------------------------------------------------

/// <param name="FetchType">gacha_type sent to the Zenless Zone Zero Signal Search history API.</param>
public sealed record BannerDef(
    string Key, string Title, string Subtitle,
    string FetchType, int HardPity, int SoftPity, Color Accent);

/// <summary>Everything one dashboard card needs. Bound from XAML.</summary>
public sealed class BannerStats
{
    private static readonly Brush SoftBrush = Make(0xFF, 0xB4, 0x54);
    private static readonly Brush NormalBrush = Make(0xF2, 0xF3, 0xF8);

    public string Title { get; init; } = "";
    public string Subtitle { get; init; } = "";
    public Brush Accent { get; init; } = Brushes.White;

    public int Pity { get; init; }            // searches since last S-Rank
    public int HardPity { get; init; }
    public int SoftPity { get; init; }
    public int FourStarPity { get; init; }    // searches since last A-Rank (guaranteed at 10)
    public int TotalPulls { get; init; }
    public int FourStarCount { get; init; }
    public int FiveStarCount { get; init; }
    public string LastFourStar { get; init; } = "—";
    public string LastFiveStar { get; init; } = "—";

    private bool InSoftPity => Pity + 1 >= SoftPity;   // the NEXT pull is at/after soft pity

    public Brush PityBrush => InSoftPity ? SoftBrush : NormalBrush;

    public string StateText =>
        Pity + 1 >= HardPity ? "Next search is a guaranteed S-Rank" :
        InSoftPity ? "Soft pity — S-Rank chance is rising" : "";

    private static Brush Make(byte r, byte g, byte b)
    {
        var br = new SolidColorBrush(Color.FromRgb(r, g, b));
        br.Freeze();
        return br;
    }
}

// ---------------------------------------------------------------------------
// Exceptions (so the UI can show friendly, specific messages)
// ---------------------------------------------------------------------------

public sealed class AuthKeyExpiredException : Exception
{
    public AuthKeyExpiredException() : base("The authkey has expired or is invalid.") { }
}

public sealed class CacheNotFoundException : Exception
{
    public CacheNotFoundException(string message) : base(message) { }
}

public sealed class ApiException : Exception
{
    public ApiException(string message) : base(message) { }
}
