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
        KeyboardState oldState;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 1000;
            graphics.PreferredBackBufferWidth = 600;
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
            oldState = Keyboard.GetState();

        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        /// 

        

        SpriteFont scoreFont;
        int score = 0;

        Texture2D ship;
        Vector2 shipPosition;
        Vector2 shipSpeed = new Vector2(0.0f, 0.0f);
        int shipHeight;
        int shipWidth;

        Texture2D rocket;
        List<Vector2> rocketPosition = new List<Vector2>();
        List<Vector2> rocketSpeed = new List<Vector2>();
        int rocketHeight;
        int rocketWidth;
        int rocketMax = 99;

        Texture2D enemy;
        List<Vector2> enemyPosition = new List<Vector2>();
        List<Vector2> enemySpeed = new List<Vector2>();
        int enemyHeight;
        int enemyWidth;
        int totalEnemies = 4;

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

            ship = Content.Load<Texture2D>("Galaga_ship");
            rocket = Content.Load<Texture2D>("rocket");
            enemy = Content.Load<Texture2D>("Enemy_Ship");

            soundEffect = Content.Load<SoundEffect>("Swords_Collide-Sound");
            music = Content.Load<SoundEffect>("siren");

            soundInstance = soundEffect.CreateInstance();
            musicInstance = music.CreateInstance();
            //musicInstance.IsLooped = true;

            shipHeight = ship.Bounds.Height;
            shipWidth = ship.Bounds.Width;

            shipPosition.X = graphics.GraphicsDevice.Viewport.Width/2;
            shipPosition.Y = graphics.GraphicsDevice.Viewport.Height - 3*shipHeight;

            rocketHeight = rocket.Bounds.Height;
            rocketWidth = rocket.Bounds.Width;

            enemyHeight = enemy.Bounds.Height;
            enemyWidth = enemy.Bounds.Width;

            int enemySpacing = graphics.GraphicsDevice.Viewport.Width / totalEnemies;

            for (int i = 0; i < totalEnemies; i++)
            {
                //enemyPosition.Add(new Vector2(graphics.GraphicsDevice.Viewport.Width / i - enemyWidth / 2, 500 ) );
                enemyPosition.Add(new Vector2(enemySpacing * i, 200));
                enemySpeed.Add(new Vector2(-50.0f, 0));
            }

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

            // Move the sprite around
            UpdateShip(gameTime, ref shipPosition, ref shipSpeed);
            UpdateRocket(gameTime, ref rocketPosition, ref rocketSpeed);
            UpdateEnemies(gameTime, ref enemyPosition, ref enemySpeed);
            CheckForCollision();

            base.Update(gameTime);
        }

        void UpdateShip(GameTime gameTime, ref Vector2 shipPosition, ref Vector2 shipSpeed)
        {

            // Move the sprite by speed, scaled by elapsed time 
            KeyboardState newState = Keyboard.GetState();

            if (newState.IsKeyDown(Keys.Right) && newState.IsKeyUp(Keys.Left))
            {
                shipSpeed.X = 200.0f;
            }

            else if (newState.IsKeyDown(Keys.Left) && newState.IsKeyUp(Keys.Right))
            {
                shipSpeed.X = -200.0f;
            }
            else
            {
                shipSpeed.X = 0.0f;
            }

            shipPosition += shipSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            //spritePosition += spriteSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            int MaxX = graphics.GraphicsDevice.Viewport.Width - ship.Width;
            int MinX = 0;
            int MaxY = graphics.GraphicsDevice.Viewport.Height - ship.Height;
            int MinY = 0;

            // Check for bounce 

            if (shipPosition.X > MaxX)
            {

                shipSpeed.X = 0;
                shipPosition.X = MaxX;
            }

            else if (shipPosition.X < MinX)
            {

                shipSpeed.X = 0;
                shipPosition.X = MinX;
            }

        }

        void UpdateRocket(GameTime gameTime, ref List<Vector2> rocketPosition, ref List<Vector2> rocketSpeed)
        {
            int MaxX = graphics.GraphicsDevice.Viewport.Width;
            int MinX = 0;
            int MaxY = graphics.GraphicsDevice.Viewport.Height;
            int MinY = 0;


            // Move the sprite by speed, scaled by elapsed time 
            KeyboardState newState = Keyboard.GetState();

            if (newState.IsKeyDown(Keys.Space) && oldState.IsKeyUp(Keys.Space))
            {
                if (rocketPosition.Count < rocketMax)
                {
                    rocketPosition.Add(new Vector2(shipPosition.X + shipWidth/2 - rocketWidth/2, shipPosition.Y));
                    rocketSpeed.Add(new Vector2(0, -500.0f));
                }
            }


            for (int i = 0; i < rocketPosition.Count; i++)
            {
                rocketPosition[i] += rocketSpeed[i] * (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (rocketPosition[i].Y > MaxY)
                {
                    rocketPosition.RemoveAt(i);
                    rocketSpeed.RemoveAt(i);
                }
                else if (rocketPosition[i].Y < MinY)
                {
                    rocketPosition.RemoveAt(i);
                    rocketSpeed.RemoveAt(i);
                }
            }
            oldState = newState;
        }

        void UpdateEnemies(GameTime gameTime, ref List<Vector2> enemyPosition, ref List<Vector2> enemySpeed)
        {
            int MaxX = graphics.GraphicsDevice.Viewport.Width;
            int MinX = 0;
            Vector2 enemyShiftY = new Vector2(0.0f, 50.0f);

            for (int i = 0; i < totalEnemies; i++)
            {
                enemyPosition[i] += enemySpeed[i] * (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (enemyPosition[i].X > MaxX - enemyWidth)
                {
                    for (int j = 0; j < totalEnemies; j++)
                    {
                        enemySpeed[j] *= -1;
                        enemyPosition[j]  += enemyShiftY;
                    } 
                }
                else if (enemyPosition[i].X < MinX)
                {
                    for (int j = 0; j < totalEnemies; j++)
                    {
                        enemySpeed[j] *= -1;
                        enemyPosition[j] += enemyShiftY;
                    }
                }
            }
        }

        void CheckForCollision()
        {
            List<BoundingBox> enemyBoxes = new List<BoundingBox>();
            List<BoundingBox> rocketBoxes = new List<BoundingBox>();

            for (int i = 0; i < totalEnemies; i++)
            {
                enemyBoxes.Add(new BoundingBox(new Vector3(enemyPosition[i].X - (enemyWidth / 2), enemyPosition[i].Y - (enemyHeight / 2), 0), new Vector3(enemyPosition[i].X + (enemyWidth / 2), enemyPosition[i].Y + (enemyHeight / 2), 0)));
            }

            for (int j = 0; j < rocketPosition.Count; j++)
            {
                rocketBoxes.Add(new BoundingBox(new Vector3(rocketPosition[j].X - (rocketWidth / 2), rocketPosition[j].Y - (rocketHeight / 2), 0), new Vector3(rocketPosition[j].X + (rocketWidth / 2), rocketPosition[j].Y + (rocketHeight / 2), 0)));
            }

            for (int i = 0; i < totalEnemies; i++)
            {
                for (int j = 0; j < rocketPosition.Count; j++)
                {
                    if (enemyBoxes[i].Intersects(rocketBoxes[j]))
                    {
                        enemyPosition.RemoveAt(i);
                        enemySpeed.RemoveAt(i);
                        rocketPosition.RemoveAt(j);
                        rocketSpeed.RemoveAt(j);
                        totalEnemies--;

                        score++;
                    }
                }
            }


            //BoundingBox bb1 = new BoundingBox(new Vector3(spritePosition1.X - (sprite1Width / 2), spritePosition1.Y - (sprite1Height / 2), 0), new Vector3(spritePosition1.X + (sprite1Width / 2), spritePosition1.Y + (sprite1Height / 2), 0));

            //BoundingBox bb2 = new BoundingBox(new Vector3(spritePosition2.X - (sprite2Width / 2), spritePosition2.Y - (sprite2Height / 2), 0), new Vector3(spritePosition2.X + (sprite2Width / 2), spritePosition2.Y + (sprite2Height / 2), 0));

            //if (bb1.Intersects(bb2))
            //{
            //    spriteSpeed1.X *= -1;
            //    spriteSpeed1.Y *= -1;

            //    spriteSpeed2.X *= -1;
              //  spriteSpeed2.Y *= -1;

                //score++;

                //collide = true;

                //musicInstance.Stop();
                
                //soundInstance.Play();

            //}
            //else
            //{
              //  if(musicInstance.State != SoundState.Playing && soundInstance.State != SoundState.Playing && collide)
                //{
                  //  musicInstance.Play();
                    //collide = false;
               // }
            //}
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

            spriteBatch.DrawString(scoreFont, "SCORE " + score.ToString(), new Vector2(10, 10), Color.White);

            spriteBatch.Draw(ship, shipPosition, Color.White);


            for (int i = 0; i < rocketPosition.Count; i++)
            {
                spriteBatch.Draw(rocket, rocketPosition[i], Color.White);
            }

            for (int i = 0; i < totalEnemies; i++)
            {
                spriteBatch.Draw(enemy, enemyPosition[i], Color.White);
            }
            
            spriteBatch.End();

            //spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            //spriteBatch.Draw(texture2, spritePosition2, null, Color.White, 2*RotationAngle, origin2, 1.0f, SpriteEffects.None, 0f);
            //spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
