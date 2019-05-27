using System;
using Microsoft.Xna.Framework;

namespace KernelPanic
{
    internal class CooldownComponent
    {
        private bool mEnabled = true;
        private TimeSpan mCooldown;
        private TimeSpan mRemainingCooldown;

        public CooldownComponent(TimeSpan time)
        {
            mCooldown = time;
            mRemainingCooldown = time;
        }

        public delegate void CooleddownDelegate(CooldownComponent cooldownComponent);

        public event CooleddownDelegate CooledDown;

        // ReSharper disable once UnusedMember.Global
        public void Reset()
        {
            mEnabled = true;
            mRemainingCooldown = mCooldown;
        }

        // ReSharper disable once UnusedMember.Global
        public void Reset(TimeSpan time)
        {
            mEnabled = true;
            mCooldown = time;
            mRemainingCooldown = time;
        }

        public void Update(GameTime time)
        {
            if (!mEnabled) { return; }
            mRemainingCooldown -= time.ElapsedGameTime;
            // check if time is over
            if (mRemainingCooldown <= TimeSpan.Zero)
            {
                mRemainingCooldown = TimeSpan.Zero;
                mEnabled = false;
                CooledDown?.Invoke(this);
            }
        }
    }
}
