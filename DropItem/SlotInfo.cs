using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DropItem
{
    internal class SlotInfo
    {
        public readonly static SlotInfo[] LockerSlotInfo = new SlotInfo[]
        {
            new(new(-0.724f, -0.252f, 1.754f), new(0.42f, 0.433f, 0.3f)),
            new(new(-0.672f, -0.252f, 1.454f), new(0.5f, 0.433f, 0.25f)),
            new(new(-0.672f, -0.252f, 1.154f), new(0.5f, 0.433f, 0.25f)),
            new(new(-0.672f, -0.252f, 0.854f), new(0.5f, 0.433f, 0.25f)),
            new(new(-0.672f, -0.252f, 0.354f), new(0.5f, 0.433f, 0.67f)),
            new(new(-0.276f, -0.252f, 1.754f), new(0.42f, 0.433f, 0.3f))
        };

        public readonly static SlotInfo[] BoxSlotInfo = new SlotInfo[]
        {
            new(new(0.009f, 0.0f, 0.163f), new(0.33f, 0.54f, 0.323f)),
            new(new(-0.343f, 0.0f, 0.163f), new(0.34f, 0.54f, 0.323f)),
            new(new(0.358f, 0.0f, 0.163f), new(0.33f, 0.54f, 0.323f))
        };

        public readonly Vector3 LocalPosition;
        public readonly Vector3 LocalScale;

        public SlotInfo(Vector3 position, Vector3 scale)
        {
            LocalPosition = position;
            LocalScale = scale;
        }
    }
}
