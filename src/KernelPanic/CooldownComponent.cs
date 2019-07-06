using System;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace KernelPanic
{
    internal sealed class CooldownComponent
    {
        [JsonProperty]
        private TimeSpan mCooldown;

        [JsonProperty]
        private TimeSpan mRemainingCooldown;

        [JsonProperty]
        internal bool Ready { get; private set; }

        [JsonProperty]
        internal bool Enabled { get; set; }

        [JsonIgnore]
        internal TimeSpan TimeSpan => mCooldown;

        internal CooldownComponent(TimeSpan time, bool isHot = true)
        {
            mCooldown = time;
            mRemainingCooldown = isHot ? time : TimeSpan.Zero;
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
            mRemainingCooldown = mCooldown;
            Ready = false;
        }

        // ReSharper disable once UnusedMember.Global
        internal void Reset(TimeSpan time)
        {
            Enabled = true;
            mCooldown = time;
            mRemainingCooldown = time;
            Ready = false;
        }

        internal void Update(GameTime time)
        {
            if (!Enabled) { return; }
            mRemainingCooldown -= time.ElapsedGameTime;
            // check if time is over
            if (mRemainingCooldown > TimeSpan.Zero) return;
            mRemainingCooldown = TimeSpan.Zero;
            Enabled = false;
            Ready = true;
            CooledDown?.Invoke(this);
        }
    }
}
