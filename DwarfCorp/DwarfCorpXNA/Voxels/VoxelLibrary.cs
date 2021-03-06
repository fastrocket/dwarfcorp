// VoxelLibrary.cs
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
//using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace DwarfCorp
{
    /// <summary>
    /// A static collection of voxel types and their properties.
    /// </summary>
    [JsonObject(IsReference = true)]
    public class VoxelLibrary
    {
        public static Dictionary<String, BoxPrimitive> PrimitiveMap = new Dictionary<String, BoxPrimitive>();
        public static VoxelType emptyType = null;
        public static VoxelType DesignationType = null;

        public static Dictionary<string, VoxelType> Types = new Dictionary<string, VoxelType>();
        public static List<VoxelType> TypeList = null;

        public static void Cleanup()
        {
            PrimitiveMap = new Dictionary<String, BoxPrimitive>();
            emptyType = null;
            DesignationType = null;
            Types = new Dictionary<string, VoxelType>();
            TypeList = null;
        }

        public VoxelLibrary()
        {
        }

        public static Dictionary<BoxTransition, BoxPrimitive.BoxTextureCoords> CreateTransitionUVs(GraphicsDevice graphics, Texture2D textureMap, int width, int height, Point[] tiles,  VoxelType.TransitionType transitionType = VoxelType.TransitionType.Horizontal)
        {
            var transitionTextures = new Dictionary<BoxTransition, BoxPrimitive.BoxTextureCoords>();

            for(int i = 0; i < 16; i++)
            {
                Point topPoint = new Point(tiles[0].X + i, tiles[0].Y);

                if (transitionType == VoxelType.TransitionType.Horizontal)
                {
                    BoxTransition transition = new BoxTransition()
                    {
                        Top = (TransitionTexture) i
                    };
                    transitionTextures[transition] = new BoxPrimitive.BoxTextureCoords(textureMap.Width,
                        textureMap.Height, width, height, tiles[2], tiles[2], topPoint, tiles[1], tiles[2], tiles[2]);
                }
                else
                {
                    for (int j = 0; j < 16; j++)
                    { 
                         Point sidePoint = new Point(tiles[0].X + j, tiles[0].Y);
                        // TODO: create every iteration of frontback vs. left right. There should be 16 of these.
                        BoxTransition transition = new BoxTransition()
                        {
                            Left = (TransitionTexture)i,
                            Right = (TransitionTexture)i,
                            Front = (TransitionTexture)j,
                            Back = (TransitionTexture)j
                        };
                        transitionTextures[transition] = new BoxPrimitive.BoxTextureCoords(textureMap.Width,
                            textureMap.Height, width, height, sidePoint, sidePoint, tiles[2], tiles[1], topPoint, topPoint);
                    }
                }
            }

            return transitionTextures;
        }

        public static BoxPrimitive CreatePrimitive(GraphicsDevice graphics, Texture2D textureMap, int width, int height, Point top, Point sides, Point bottom)
        {
            BoxPrimitive.BoxTextureCoords coords = new BoxPrimitive.BoxTextureCoords(textureMap.Width, textureMap.Height, width, height, sides, sides, top, bottom, sides, sides);
            BoxPrimitive cube = new BoxPrimitive(graphics, 1.0f, 1.0f, 1.0f, coords);

            return cube;
        }

        public static void InitializeDefaultLibrary(GraphicsDevice graphics)
        {
            if (TypeList != null) return;

            var cubeTexture = AssetManager.GetContentTexture(ContentPaths.Terrain.terrain_tiles);
            TypeList = FileUtils.LoadJsonListFromDirectory<VoxelType>(ContentPaths.voxel_types, null, v => v.Name);

            emptyType = TypeList.FirstOrDefault(v => v.Name == "_empty");
            DesignationType = TypeList.FirstOrDefault(v => v.Name == "_designation");

            // Todo: Stabalize ids across save games.
            short id = 2;
            foreach (VoxelType type in TypeList)
            {
                Types[type.Name] = type;

                if (type.Name == "_empty")
                {
                    emptyType = type;
                    type.ID = 0;
                }
                else
                {
                    PrimitiveMap[type.Name] = CreatePrimitive(graphics, cubeTexture, 32, 32, type.Top, type.Bottom, type.Sides);
                    if (type.Name == "_designation")
                    {
                        DesignationType = type;
                        type.ID = 1;
                    }
                    else
                    {
                        type.ID = id;
                        id += 1;
                    }
                }

                if (type.HasTransitionTextures)
                    type.TransitionTextures = CreateTransitionUVs(graphics, cubeTexture, 32, 32, type.TransitionTiles, type.Transitions);

                type.ExplosionSound = SoundSource.Create(type.ExplosionSoundResource);
                type.HitSound = SoundSource.Create(type.HitSoundResources);
                if (type.ReleasesResource)
                {
                    if (ResourceLibrary.GetResourceByName(type.Name) == null)
                    {
                        var resource = new Resource(ResourceLibrary.GetResourceByName(type.ResourceToRelease))
                        {
                            Name = type.Name,
                            ShortName = type.Name,
                            Tint = type.Tint,
                            Generated = false
                        };
                        ResourceLibrary.Add(resource);
                        type.ResourceToRelease = resource.Name;
                    }
                }
            }

            TypeList = TypeList.OrderBy(v => v.ID).ToList();

            if (TypeList.Count > VoxelConstants.MaximumVoxelTypes)
                throw new InvalidProgramException(String.Format("There can be only {0} voxel types.", VoxelConstants.MaximumVoxelTypes));
        }

        public static VoxelType GetVoxelType(short id)
        {
            return TypeList[id];
        }

        public static VoxelType GetVoxelType(string name)
        {
            return Types[name];
        }

        public static BoxPrimitive GetPrimitive(string name)
        {
            if (PrimitiveMap.ContainsKey(name))
                return PrimitiveMap[name];
            return null;
        }

        public static BoxPrimitive GetPrimitive(VoxelType type)
        {
            return GetPrimitive(type.Name);
        }

        public static BoxPrimitive GetPrimitive(short id)
        {
            return GetPrimitive(GetVoxelType(id));
        }

        public static List<VoxelType> GetTypes()
        {
            return TypeList;
        }

        public static Dictionary<int, String> GetVoxelTypeMap()
        {
            var r = new Dictionary<int, String>();
            for (var i = 0; i < TypeList.Count; ++i)
                r.Add(i, TypeList[i].Name);
            return r;
        }

        // Do not delete: Used to generate block icon texture for menu.
        // Todo: Use Sheet.TileHeight as well.
        [TextureGenerator("Voxels")]
        public static Texture2D RenderIcons(GraphicsDevice device, Microsoft.Xna.Framework.Content.ContentManager Content, Gui.JsonTileSheet Sheet)
        {
            InitializeDefaultLibrary(device);

            var shader = new Shader(Content.Load<Effect>(ContentPaths.Shaders.TexturedShaders), true);

            var sqrt = (int)(Math.Ceiling(Math.Sqrt(PrimitiveMap.Count)));
            var width = MathFunctions.NearestPowerOf2(sqrt * Sheet.TileWidth);
            var height = MathFunctions.NearestPowerOf2(sqrt * Sheet.TileWidth);

            RenderTarget2D toReturn = new RenderTarget2D(device, width, height, false, SurfaceFormat.Color, DepthFormat.Depth16, 0, RenderTargetUsage.PreserveContents);
        
            device.SetRenderTarget(toReturn);
            device.Clear(Color.Transparent);
            shader.SetIconTechnique();
            shader.MainTexture = AssetManager.GetContentTexture(ContentPaths.Terrain.terrain_tiles);
            shader.SelfIlluminationEnabled = true;
            shader.SelfIlluminationTexture = AssetManager.GetContentTexture(ContentPaths.Terrain.terrain_illumination);
            shader.EnableShadows = false;
            shader.EnableLighting = false;
            shader.ClippingEnabled = false;
            shader.CameraPosition = new Vector3(-0.5f, 0.5f, 0.5f);
            shader.VertexColorTint = Color.White;
            shader.LightRamp = Color.White;
            shader.SunlightGradient = AssetManager.GetContentTexture(ContentPaths.Gradients.sungradient);
            shader.AmbientOcclusionGradient = AssetManager.GetContentTexture(ContentPaths.Gradients.ambientgradient);
            shader.TorchlightGradient = AssetManager.GetContentTexture(ContentPaths.Gradients.torchgradient);

            Viewport oldview = device.Viewport;
            int rows = height  / Sheet.TileWidth;
            int cols = width/ Sheet.TileWidth;
            device.ScissorRectangle = new Rectangle(0, 0, Sheet.TileWidth, Sheet.TileWidth);
            device.RasterizerState = RasterizerState.CullNone;
            device.DepthStencilState = DepthStencilState.Default;
            Vector3 half = Vector3.One*0.5f;
            half = new Vector3(half.X, half.Y, half.Z);

            List<VoxelType> voxelsByType = Types.Select(type => type.Value).ToList();
            voxelsByType.Sort((a, b) => a.ID < b.ID ? -1 : 1);

            foreach (EffectPass pass in shader.CurrentTechnique.Passes)
            {
                foreach (var type in voxelsByType)
                {
                    int row = type.ID/cols;
                    int col = type.ID%cols;
                    BoxPrimitive primitive = GetPrimitive(type);
                    if (primitive == null)
                        continue;

                    if (type.HasTransitionTextures)
                        primitive = new BoxPrimitive(device, 1, 1, 1, type.TransitionTextures[new BoxTransition()]);

                    device.Viewport = new Viewport(col * Sheet.TileWidth, row * Sheet.TileWidth, Sheet.TileWidth, Sheet.TileWidth);
                    Matrix viewMatrix = Matrix.CreateLookAt(new Vector3(-1.2f, 1.0f, -1.5f), Vector3.Zero, Vector3.Up);
                    Matrix projectionMatrix = Matrix.CreateOrthographic(1.5f, 1.5f, 0, 5);
                    shader.View = viewMatrix;
                    shader.Projection = projectionMatrix;
                    shader.World = Matrix.CreateTranslation(-half);
                    shader.VertexColorTint = type.Tint;
                    pass.Apply();
                    primitive.Render(device);
                }
            }
            device.Viewport = oldview;
            device.SetRenderTarget(null);
            return (Texture2D) toReturn;
        }
    }

}