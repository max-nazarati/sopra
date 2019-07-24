using Newtonsoft.Json;

namespace KernelPanic.Options
{
    public sealed class OptionsData
    {
        [JsonProperty] internal bool PlayBackgroundMusic { get; set; }
        [JsonProperty] internal bool PlaySoundEffects { get; set; }
        [JsonProperty] internal float MusicVolume { get; set; } = 0.5f;
        [JsonProperty] internal bool IsFullscreen { get; set; }
        [JsonProperty] internal bool ScrollInverted { get; set; }
        [JsonProperty] internal KeyMap KeyMap { get; set; } = new KeyMap();
    }
}
