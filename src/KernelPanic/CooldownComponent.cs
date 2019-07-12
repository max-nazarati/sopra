using System;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace KernelPanic
{
    internal sealed class CooldownComponent
    {
        [JsonProperty]
        internal TimeSpan Cooldown { get; set; }

        [JsonProperty]
        internal TimeSpan RemainingCooldown { get; private set; }

        [JsonProperty]
        internal bool Ready { get; private set; }

        [JsonProperty]
        internal bool Enabled { get; set; }

        internal CooldownComponent(TimeSpan time, bool isHot = true)
        {
            Cooldown = time;
            RemainingCooldown = isHot ? time : TimeSpan.Zero;
            Enabled = isHot;
            Ready = !isHot;
        }

        /// <summary>
        /// Used for deserialization.
        /// </summary>
        private CooldownComponent()
        {
        }

        internal delegate void CooledDownDelegate(CooldownComponent cooldownComponent);

        internal event CooledDownDelegate CooledDown;
        
        internal void Reset()
        {
            Enabled = true;
            RemainingCooldown = Cooldown;
            Ready = false;
        }

        // ReSharper disable once UnusedMember.Global
        internal void Reset(TimeSpan time)
        {
            Enabled = true;
            Cooldown = time;
            RemainingCooldown = time;
            Ready = false;
        }

        internal void Update(GameTime time)
        {
            if (!Enabled) { return; }
            RemainingCooldown -= time.ElapsedGameTime;
            // check if time is over
            if (RemainingCooldown > TimeSpan.Zero) return;
            RemainingCooldown = TimeSpan.Zero;
            Enabled = false;
            Ready = true;
            CooledDown?.Invoke(this);
        }
    }
}
