﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DwarfCorp.GameStates;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace DwarfCorp
{
    [JsonObject(IsReference = true)]
    public class Target : Fixture
    {
        public Target()
        {

        }

        public Target(Vector3 position) :
            base(position, new SpriteSheet(ContentPaths.Entities.Furniture.interior_furniture, 32, 32), new Point(0, 5), PlayState.ComponentManager.RootComponent)
        {
            Name = "Target";
            Tags.Add("Target");
            Tags.Add("Train");
        }
    }
}
