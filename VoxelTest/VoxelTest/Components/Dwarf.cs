﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Newtonsoft.Json;

namespace DwarfCorp
{
    public class Dwarf : Creature
    {
        public Dwarf(CreatureStats stats, string allies, PlanService planService, GameMaster master, ComponentManager manager, string name, ChunkManager chunks, GraphicsDevice graphics, ContentManager content, Texture2D dwarfTexture, Vector3 position) :
            base(stats, allies, planService, master, new PhysicsComponent(manager, "dwarf", manager.RootComponent, Matrix.CreateTranslation(position), new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0.0f, -0.25f, 0.0f), 1.0f, 1.0f, 0.999f, 0.999f, new Vector3(0, -10, 0)), 
                    manager, chunks, graphics, content, name)
        {
            Initialize(dwarfTexture);
        }

        public void Initialize(Texture2D dwarfSprites)
        {
            Physics.OrientWithVelocity = true;
            int frameWidth = 32;
            int frameHeight = 40;
            Sprite = new CharacterSprite(Graphics, Manager, "Dwarf Sprite", Physics, Matrix.CreateTranslation(new Vector3(0, 0.1f, 0)));
            
            Sprite.AddAnimation(Creature.CharacterMode.Walking, OrientedAnimation.Orientation.Forward, dwarfSprites, 15.0f, frameWidth, frameHeight,  0,   0, 1, 2, 1, 0, 1, 2, 3);
            Sprite.AddAnimation(Creature.CharacterMode.Walking, OrientedAnimation.Orientation.Right, dwarfSprites, 15.0f, frameWidth, frameHeight, 2, 0, 1, 2, 1, 0, 1, 2, 3);
            Sprite.AddAnimation(Creature.CharacterMode.Walking, OrientedAnimation.Orientation.Left, dwarfSprites, 15.0f, frameWidth, frameHeight, 1, 0, 1, 2, 1, 0, 1, 2, 3);
            Sprite.AddAnimation(Creature.CharacterMode.Walking, OrientedAnimation.Orientation.Backward, dwarfSprites, 15.0f, frameWidth, frameHeight, 3, 0, 1, 2, 1);

            Sprite.AddAnimation(Creature.CharacterMode.Idle, OrientedAnimation.Orientation.Forward, dwarfSprites, 0.8f, frameWidth, frameHeight, 0, 1, 3, 1);
            Sprite.AddAnimation(Creature.CharacterMode.Idle, OrientedAnimation.Orientation.Right, dwarfSprites, 0.8f, frameWidth, frameHeight, 2, 2, 0, 2);
            Sprite.AddAnimation(Creature.CharacterMode.Idle, OrientedAnimation.Orientation.Left, dwarfSprites, 0.8f, frameWidth, frameHeight, 1, 1, 3, 1);
            Sprite.AddAnimation(Creature.CharacterMode.Idle, OrientedAnimation.Orientation.Backward, dwarfSprites, 0.8f, frameWidth, frameHeight, 3, 1);

            Sprite.AddAnimation(Creature.CharacterMode.Attacking, OrientedAnimation.Orientation.Forward, dwarfSprites, 8.0f, frameWidth, frameHeight, 8, 0, 1, 2, 3);
            Sprite.AddAnimation(Creature.CharacterMode.Attacking, OrientedAnimation.Orientation.Right, dwarfSprites, 8.0f, frameWidth, frameHeight, 10, 0, 1, 2, 3);
            Sprite.AddAnimation(Creature.CharacterMode.Attacking, OrientedAnimation.Orientation.Left, dwarfSprites, 8.0f, frameWidth, frameHeight, 9, 0, 1, 2, 3);
            Sprite.AddAnimation(Creature.CharacterMode.Attacking, OrientedAnimation.Orientation.Backward, dwarfSprites, 8.0f, frameWidth, frameHeight, 11, 0, 1, 2, 3);

            Sprite.AddAnimation(Creature.CharacterMode.Falling, OrientedAnimation.Orientation.Forward, dwarfSprites, 15.0f, frameWidth, frameHeight, 4, 1);
            Sprite.AddAnimation(Creature.CharacterMode.Falling, OrientedAnimation.Orientation.Right, dwarfSprites, 15.0f, frameWidth, frameHeight, 6, 1);
            Sprite.AddAnimation(Creature.CharacterMode.Falling, OrientedAnimation.Orientation.Left, dwarfSprites, 15.0f, frameWidth, frameHeight, 5, 1);
            Sprite.AddAnimation(Creature.CharacterMode.Falling, OrientedAnimation.Orientation.Backward, dwarfSprites, 15.0f, frameWidth, frameHeight, 7, 1);

            Sprite.AddAnimation(Creature.CharacterMode.Jumping, OrientedAnimation.Orientation.Forward, dwarfSprites, 15.0f, frameWidth, frameHeight,  4, 0);
            Sprite.AddAnimation(Creature.CharacterMode.Jumping, OrientedAnimation.Orientation.Right, dwarfSprites, 15.0f, frameWidth, frameHeight,    6, 0);
            Sprite.AddAnimation(Creature.CharacterMode.Jumping, OrientedAnimation.Orientation.Left, dwarfSprites, 15.0f, frameWidth, frameHeight,     5, 0);
            Sprite.AddAnimation(Creature.CharacterMode.Jumping, OrientedAnimation.Orientation.Backward, dwarfSprites, 15.0f, frameWidth, frameHeight, 7, 0);

            Sprite.AddAnimation(Creature.CharacterMode.Swimming, OrientedAnimation.Orientation.Forward, dwarfSprites, 15.0f, frameWidth, frameHeight, 12, 0, 1, 2, 1, 0, 1, 2, 3);
            Sprite.AddAnimation(Creature.CharacterMode.Swimming, OrientedAnimation.Orientation.Right, dwarfSprites, 15.0f, frameWidth, frameHeight, 14, 0, 1, 2, 1, 0, 1, 2, 3);
            Sprite.AddAnimation(Creature.CharacterMode.Swimming, OrientedAnimation.Orientation.Left, dwarfSprites, 15.0f, frameWidth, frameHeight, 13, 0, 1, 2, 1, 0, 1, 2, 3);
            Sprite.AddAnimation(Creature.CharacterMode.Swimming, OrientedAnimation.Orientation.Backward, dwarfSprites, 15.0f, frameWidth, frameHeight, 15, 0, 1, 2, 1);


            Hands = new Grabber(Manager, "hands", Physics, Matrix.Identity, new Vector3(0.1f, 0.1f, 0.1f), Vector3.Zero);

            Sensors = new EnemySensor(Manager, "EnemySensor", Physics, Matrix.Identity, new Vector3(20, 5, 20), Vector3.Zero);

            CreatureAIComponent dwarfAI = new CreatureAIComponent(this, "Dwarf AI", Sensors, PlanService); 

            Weapon = new Weapon("Pickaxe", 1.0f, 2.0f, 1.0f, dwarfAI, "pick");

            Health = new HealthComponent(Manager, "Health", Physics, Stats.MaxHealth, 0.0f, Stats.MaxHealth);

            Matrix shadowTransform = Matrix.CreateRotationX((float)Math.PI * 0.5f);
            shadowTransform.Translation = new Vector3(0.0f, -0.5f, 0.0f);

            Shadow = new ShadowComponent(Manager, "Shadow", Physics, shadowTransform, Content.Load<Texture2D>("shadowcircle"));
            List<Point> shP = new List<Point>();
            shP.Add(new Point(0, 0));
            Animation shadowAnimation = new Animation(Graphics, Content.Load<Texture2D>("shadowcircle"), "sh", 32, 32, shP, false, Color.Black, 1, 0.7f, 0.7f, false);
            Shadow.AddAnimation(shadowAnimation);
            shadowAnimation.Play();
            Shadow.SetCurrentAnimation("sh");
            Physics.Tags.Add("Dwarf");

            DeathEmitter = new EmitterComponent("blood_particle", Manager, "Death Gibs", Physics, Matrix.Identity, Vector3.One, Vector3.Zero);
            DeathEmitter.TriggerOnDeath = true;
            DeathEmitter.TriggerAmount = 100;

            //string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            //System.IO.File.WriteAllText(@"C:\Users\Mklingen\Desktop\Dwarf.json", json);

        }
    }
}
