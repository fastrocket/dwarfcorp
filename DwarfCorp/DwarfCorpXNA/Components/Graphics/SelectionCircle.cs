// SelectionCircle.cs
// 
//  Modified MIT License (MIT)
//  
//  Copyright (c) 2015 Completely Fair Games Ltd.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// The following content pieces are considered PROPRIETARY and may not be used
// in any derivative works, commercial or non commercial, without explicit 
// written permission from Completely Fair Games:
// 
// * Images (sprites, textures, etc.)
// * 3D Models
// * Sound Effects
// * Music
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DwarfCorp.GameStates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace DwarfCorp
{
    [JsonObject(IsReference = true)]
    public class SelectionCircle : Body
    {
        public SelectionCircle()
            : base()
        {
            UpdateRate = 2;
        }

        public SelectionCircle(ComponentManager manager) :
            base(manager, "Selection", Matrix.CreateRotationX((float)Math.PI), Vector3.One, Vector3.Zero)
        {
            var shadowTransform = Matrix.CreateRotationX((float)Math.PI * 0.5f);
            shadowTransform.Translation = new Vector3(0.0f, -0.25f, 0.0f);

            LocalTransform = shadowTransform;
            CreateCosmeticChildren(manager);
            SetFlagRecursive(Flag.Visible, false);
            UpdateRate = 2;
        }

        public void FitToParent()
        {
            var shadowTransform = Matrix.CreateRotationX((float)Math.PI * 0.5f);
            var bbox = (Parent as Body).GetBoundingBox();
            shadowTransform.Translation = new Vector3(0.0f, -0.5f * (bbox.Max.Y - bbox.Min.Y), 0.0f);
            float scale = bbox.Max.X - bbox.Min.X;
            shadowTransform = shadowTransform * Matrix.CreateScale(scale);
            LocalTransform = shadowTransform;
        }

        public override void CreateCosmeticChildren(ComponentManager Manager)
        {
            AddChild(new SimpleSprite(Manager, "Sprite", Matrix.Identity, new SpriteSheet(ContentPaths.Effects.selection_circle), Point.Zero)
            {
                LightsWithVoxels = false,
                OrientationType = SimpleSprite.OrientMode.Fixed
            }
            ).SetFlag(Flag.ShouldSerialize, false);

            base.CreateCosmeticChildren(Manager);
        }
    }
}