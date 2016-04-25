using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Step8
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        /// 

        SpriteFont scoreFont;
        int score = 0;

        Texture2D texture1;
        Texture2D texture2;
        Vector2 spritePosition1;
        Vector2 spritePosition2;
        Vector2 spriteSpeed1 = new Vector2(50.0f, 50.0f);
        Vector2 spriteSpeed2 = new Vector2(100.0f, 100.0f);
        Vector2 origin1;
        Vector2 origin2;

        int sprite1Height;
        int sprite1Width;
        int sprite2Height;
        int sprite2Width;

        float RotationAngle;

        SoundEffect soundEffect;
        SoundEffect music;

        SoundEffectInstance soundInstance;
        SoundEffectInstance musicInstance;

        bool collide = false;

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures

            spriteBatch = new SpriteBatch(GraphicsDevice);

            scoreFont = Content.Load<SpriteFont>("SpriteFont1");

            texture1 = Content.Load<Texture2D>("elite-knight");
            texture2 = Content.Load<Texture2D>("elite-knight");

            soundEffect = Content.Load<SoundEffect>("Swords_Collide-Sound");
            music = Content.Load<SoundEffect>("siren");

            soundInstance = soundEffect.CreateInstance();
            musicInstance = music.CreateInstance();
            //musicInstance.IsLooped = true;

            sprite1Height = texture1.Bounds.Height;
            sprite1Width = texture1.Bounds.Width;

            sprite2Height = texture2.Bounds.Height;
            sprite2Width = texture2.Bounds.Width;

            spritePosition1.X = sprite1Width;
            spritePosition1.Y = sprite1Height;

            spritePosition2.X = graphics.GraphicsDevice.Viewport.Width - texture2.Width;
            spritePosition2.Y = graphics.GraphicsDevice.Viewport.Height - texture2.Height;

            origin1.X = sprite1Width / 2;
            origin1.Y = sprite1Height / 2;

            origin2.X = sprite2Width / 2;
            origin2.Y = sprite2Height / 2;

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allow the game to exit

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // The time since Update was called last.
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            RotationAngle += elapsed;
            float circle = MathHelper.Pi * 2;
            RotationAngle = RotationAngle % circle;

            // Move the sprite around
            UpdateSprite(gameTime, ref spritePosition1, ref spriteSpeed1);
            UpdateSprite(gameTime, ref spritePosition2, ref spriteSpeed2);
            CheckForCollision();

            base.Update(gameTime);
        }

        void UpdateSprite(GameTime gameTime, ref Vector2 spritePosition, ref Vector2 spriteSpeed)
        {

            // Move the sprite by speed, scaled by elapsed time 

            spritePosition += spriteSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            int MaxX = graphics.GraphicsDevice.Viewport.Width - texture1.Width/2;
            int MinX = texture1.Width/2;
            int MaxY = graphics.GraphicsDevice.Viewport.Height - texture1.Height/2;
            int MinY = texture1.Height/2;

            // Check for bounce 

            if (spritePosition.X > MaxX)
            {

                spriteSpeed.X *= -1;
                spritePosition.X = MaxX;
            }

            else if (spritePosition.X < MinX)
            {

                spriteSpeed.X *= -1;
                spritePosition.X = MinX;
            }

            if (spritePosition.Y > MaxY)
            {

                spriteSpeed.Y *= -1;
                spritePosition.Y = MaxY;
            }

            else if (spritePosition.Y < MinY)
            {

                spriteSpeed.Y *= -1;
                spritePosition.Y = MinY;
            }
        }

        void CheckForCollision()
        {

            BoundingBox bb1 = new BoundingBox(new Vector3(spritePosition1.X - (sprite1Width / 2), spritePosition1.Y - (sprite1Height / 2), 0), new Vector3(spritePosition1.X + (sprite1Width / 2), spritePosition1.Y + (sprite1Height / 2), 0));

            BoundingBox bb2 = new BoundingBox(new Vector3(spritePosition2.X - (sprite2Width / 2), spritePosition2.Y - (sprite2Height / 2), 0), new Vector3(spritePosition2.X + (sprite2Width / 2), spritePosition2.Y + (sprite2Height / 2), 0));

            if (bb1.Intersects(bb2))
            {
                spriteSpeed1.X *= -1;
                spriteSpeed1.Y *= -1;

                spriteSpeed2.X *= -1;
                spriteSpeed2.Y *= -1;

                score++;

                collide = true;

                musicInstance.Stop();
                
                soundInstance.Play();

            }
            else
            {
                if(musicInstance.State != SoundState.Playing && soundInstance.State != SoundState.Playing && collide)
                {
                    musicInstance.Play();
                    collide = false;
                }
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            // Draw the sprite

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            spriteBatch.DrawString(scoreFont, "Times Collided: " + score.ToString(), new Vector2(10, 10), Color.White);

            spriteBatch.Draw(texture1, spritePosition1, null, Color.White, RotationAngle, origin1, 1.0f, SpriteEffects.None, 0f);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            spriteBatch.Draw(texture2, spritePosition2, null, Color.White, 2*RotationAngle, origin2, 1.0f, SpriteEffects.None, 0f);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
