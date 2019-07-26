using System;
using System.Globalization;
using KernelPanic.Table;
using KernelPanic.Tracking;
using Newtonsoft.Json;

namespace KernelPanic.Serialization
{
    internal struct Storage
    {
        [JsonProperty]
        internal Board Board { get; set; }

        [JsonProperty]
        internal TimeSpan GameTime { get; set; }

        [JsonProperty]
        internal AchievementPool.Data? AchievementData { get; set; }

        internal struct Info
        {
            /// <summary>
            /// The timestamp the associated data was saved.
            /// </summary>
            [JsonProperty]
            internal DateTime Timestamp { /*internal*/ private get; set; }

            internal string Label => Timestamp.ToString("d.M., HH:mm", CultureInfo.CurrentCulture);
        }
    }
}