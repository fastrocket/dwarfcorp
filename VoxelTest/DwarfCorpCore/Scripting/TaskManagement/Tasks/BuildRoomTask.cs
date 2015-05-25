﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace DwarfCorp
{
    /// <summary>
    /// Tells a creature that it should find an item with the specified
    /// tags and put it in a given zone.
    /// </summary>
    [Newtonsoft.Json.JsonObject(IsReference = true)]
    internal class BuildRoomTask : Task
    {
        public BuildRoomOrder Zone;

        public BuildRoomTask()
        {
            Priority = PriorityType.Low;
        }

        public BuildRoomTask(BuildRoomOrder zone)
        {
            Name = "Build BuildRoom " + zone.ToBuild.RoomData.Name + zone.ToBuild.ID;
            Zone = zone;
            Priority = PriorityType.Low;
        }

        public override Task Clone()
        {
            return new BuildRoomTask(Zone);
        }

        public override Act CreateScript(Creature creature)
        {
            return new BuildRoomAct(creature.AI, Zone);
        }

        public override float ComputeCost(Creature agent)
        {
            return (Zone == null) ? 1000 : 1.0f;
        }
    }

}