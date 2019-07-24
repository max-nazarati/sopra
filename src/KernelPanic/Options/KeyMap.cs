using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;

namespace KernelPanic.Options
{
    public sealed class KeyMap
    {
        [JsonProperty]
        private List<(Keys, Keys)> mKeyList = new List<(Keys, Keys)>();

        internal Keys this[Keys key]
        {
            get => IndexOf(key, out var index) ? mKeyList[index].Item2 : key;

            set
            {
                if (IndexOf(key, out var index))
                {
                    mKeyList[index] = (key, value);
                    return;
                }

                mKeyList.Insert(~index, (key, value));
            }
        }

        private bool IndexOf(Keys key, out int index)
        {
            index = mKeyList.BinarySearch((key, default(Keys)), new KeyComparer());
            return index >= 0;
        }

        private struct KeyComparer : IComparer<(Keys, Keys)>
        {
            public int Compare((Keys, Keys) x, (Keys, Keys) y)
            {
                return x.Item1.CompareTo(y.Item1);
            }
        }
    }
}