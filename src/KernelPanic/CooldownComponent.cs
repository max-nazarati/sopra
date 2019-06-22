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

        private bool mReady;

        internal bool Ready
        {
            get => mReady;
            set => mReady = value;
        }
        
        internal CooldownComponent(TimeSpan time, bool isHot = true)
        {
            mCooldown = time;
            mRemainingCooldown = isHot ? time : TimeSpan.Zero;
            Enabled = isHot;
            Ready = !isHot;
        }

        internal delegate void CooledDownDelegate(CooldownComponent cooldownComponent);

        internal event CooledDownDelegate CooledDown;

        internal bool Enabled { get; set; }
        
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
