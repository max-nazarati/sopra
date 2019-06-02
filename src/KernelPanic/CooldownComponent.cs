using System;
using Microsoft.Xna.Framework;

namespace KernelPanic
{
    internal sealed class CooldownComponent
    {
        private TimeSpan mCooldown;
        private TimeSpan mRemainingCooldown;

        internal CooldownComponent(TimeSpan time)
        {
            mCooldown = time;
            mRemainingCooldown = time;
        }

        internal delegate void CooledDownDelegate(CooldownComponent cooldownComponent);

        internal event CooledDownDelegate CooledDown;

        internal bool Enabled { get; set; } = true;

        internal void Reset()
        {
            Enabled = true;
            mRemainingCooldown = mCooldown;
        }

        // ReSharper disable once UnusedMember.Global
        internal void Reset(TimeSpan time)
        {
            Enabled = true;
            mCooldown = time;
            mRemainingCooldown = time;
        }

        internal void Update(GameTime time)
        {
            if (!Enabled) { return; }
            mRemainingCooldown -= time.ElapsedGameTime;
            // check if time is over
            if (mRemainingCooldown > TimeSpan.Zero) return;
            mRemainingCooldown = TimeSpan.Zero;
            Enabled = false;
            CooledDown?.Invoke(this);
        }
    }
}
