using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TGC.MonoGame.TP.Cameras;
using TGC.MonoGame.TP.FigurasBasicas;
using TGC.MonoGame.TP.Modelos;

namespace TGC.MonoGame.TP
{
    /// <summary>
    ///     Esta es la clase principal  del juego.
    ///     Inicialmente puede ser renombrado o copiado para hacer más ejemplos chicos, en el caso de copiar para que se
    ///     ejecute el nuevo ejemplo deben cambiar la clase que ejecuta Program <see cref="Program.Main()" /> linea 10.
    /// </summary>
    public class TGCGame : Game
    {
        public const string ContentFolder3D = "Models/";
        public const string ContentFolderEffects = "Effects/";
        public const string ContentFolderMusic = "Music/";
        public const string ContentFolderSounds = "Sounds/";
        public const string ContentFolderSpriteFonts = "SpriteFonts/";
        public const string ContentFolderTextures = "Textures/";

        /// <summary>
        ///     Constructor del juego.
        /// </summary>
        public TGCGame()
        {
            // Maneja la configuracion y la administracion del dispositivo grafico.
            Graphics = new GraphicsDeviceManager(this);
            // Descomentar para que el juego sea pantalla completa.
            // Graphics.IsFullScreen = true;
            // Carpeta raiz donde va a estar toda la Media.
            Content.RootDirectory = "Content";
            // Hace que el mouse sea visible.
            IsMouseVisible = true;
        }

        private GraphicsDeviceManager Graphics { get; }
        private SpriteBatch SpriteBatch { get; set; }
        private SpriteFont spriteFont { get; set; }
        private Model CarModel { get; set; }
        private Model battleCarModel { get; set; }
        private Effect Effect { get; set; }
        private Effect TilingEffect { get; set; }
        private float Rotation { get; set; }
        private FreeCamera FreeCamera { get; set; }
        private TargetCamera TargetCamera { get; set; }
        private QuadPrimitive quad { get; set; }
        private String ubicacionModelo { get; set; }
        private TipoDeCamara tipoDeCamara { get; set; }
        private Texture2D FloorTexture { get; set; }
        private List<Texture2D> CarTextures { get; set; }
        private List<Texture2D> BattleCarTextures { get; set; }
        private Texture2D TexturaDeAutoEnEdicion;
        private int IndexListaModelo { get; set; }
        private int CantidadModelos { get; set; }
        private List<Modelo> ModelosUsados { get; set; }
        private List<Pared> Walls { get; set; }
        private Texture2D WallTexture { get; set; }
        private Matrix viewProjection;

        /// <summary>
        ///     Se llama una sola vez, al principio cuando se ejecuta el ejemplo.
        ///     Escribir aqui el codigo de inicializacion: el procesamiento que podemos pre calcular para nuestro juego.
        /// </summary>
        protected override void Initialize()
        {
            // La logica de inicializacion que no depende del contenido se recomienda poner en este metodo.

            // Apago el backface culling.
            // Esto se hace por un problema en el diseno del modelo del logo de la materia.
            // Una vez que empiecen su juego, esto no es mas necesario y lo pueden sacar.
            var rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rasterizerState;
            // Seria hasta aca.

            // Configuramos nuestras matrices de la escena.
            //World = Matrix.Identity;
            //View = Matrix.CreateLookAt(Vector3.UnitZ, Vector3.Zero, Vector3.Up);
            //Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, GraphicsDevice.Viewport.AspectRatio, 1, 250);

            ModelosUsados = new List<Modelo>();
            CarTextures = new List<Texture2D>();
            BattleCarTextures = new List<Texture2D>();
            
            TargetCamera = new TargetCamera(GraphicsDevice.Viewport.AspectRatio, Vector3.One * 100f, Vector3.Zero);
            
            FreeCamera = new FreeCamera(GraphicsDevice.Viewport.AspectRatio, Vector3.One * 100f);

            tipoDeCamara = TipoDeCamara.ORIGINAL_SCENE;

            Walls = new List<Pared>();



            base.Initialize();
        }

        /// <summary>
        ///     Se llama una sola vez, al principio cuando se ejecuta el ejemplo, despues de Initialize.
        ///     Escribir aqui el codigo de inicializacion: cargar modelos, texturas, estructuras de optimizacion, el procesamiento
        ///     que podemos pre calcular para nuestro juego.
        /// </summary>
        protected override void LoadContent()
        {
            // Aca es donde deberiamos cargar todos los contenido necesarios antes de iniciar el juego.
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            

            CarModel = Content.Load<Model>(ContentFolder3D + "cars/RacingCar");

            battleCarModel = Content.Load<Model>(ContentFolder3D + "CombatVehicle/Vehicle");

            // Cargo un efecto basico propio declarado en el Content pipeline.
            // En el juego no pueden usar BasicEffect de MG, deben usar siempre efectos propios.
            Effect = Content.Load<Effect>(ContentFolderEffects + "BasicShader");

            TilingEffect = Content.Load<Effect>(ContentFolderEffects + "TextureTiling");
            TilingEffect.Parameters["Tiling"].SetValue(new Vector2(10f, 10f));

            spriteFont = Content.Load<SpriteFont>(ContentFolderSpriteFonts + "Arial");

            FloorTexture = Content.Load<Texture2D>(ContentFolderTextures + "grass");

            TexturaDeAutoEnEdicion = Content.Load<Texture2D>(ContentFolderTextures + "ground");

            WallTexture = Content.Load<Texture2D>(ContentFolderTextures + "stones");

            // Asigno el efecto que cargue a cada parte del mesh.
            // Un modelo puede tener mas de 1 mesh internamente.

            foreach (var mesh in CarModel.Meshes) {
                CarTextures.Add(((BasicEffect)mesh.Effects[0]).Texture);
                foreach (var meshPart in mesh.MeshParts) {
                    meshPart.Effect = Effect;
                }
            }
            // Un mesh puede tener mas de 1 mesh part (cada 1 puede tener su propio efecto).


            foreach (var mesh in battleCarModel.Meshes)
            {
                BattleCarTextures.Add(((BasicEffect)mesh.Effects[0]).Texture);
                foreach (var meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = Effect;
                }
            }

            //cargo los modelos de los autos comunes en sus posiciones iniciales 
            //a una lista de matrices de mundo

            var rotacion = Quaternion.CreateFromAxisAngle(-Vector3.UnitY, MathHelper.Pi / 3);
            var matrizInicial = Matrix.CreateScale(0.2f) *
                Matrix.CreateFromQuaternion(rotacion) *
                Matrix.CreateTranslation(135, 0, -321);
            ModelosUsados.Add(new Modelo(CarModel, matrizInicial, Color.Red, CarTextures));

            
            rotacion = Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathHelper.Pi / 3);
            matrizInicial = Matrix.CreateScale(0.2f) *
                Matrix.CreateFromQuaternion(rotacion) *
                Matrix.CreateTranslation(-374, 0f, -523);
            ModelosUsados.Add(new Modelo(CarModel, matrizInicial, Color.White, CarTextures));

            
            rotacion = Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathHelper.Pi / 8);
            matrizInicial = Matrix.CreateScale(0.2f) *
                Matrix.CreateFromQuaternion(rotacion) *
                Matrix.CreateTranslation(179f, 0f, -77f);
            ModelosUsados.Add(new Modelo(CarModel, matrizInicial, Color.Red, CarTextures));


            
            rotacion = Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathHelper.Pi / 2);
            matrizInicial = Matrix.CreateScale(0.2f) *
                Matrix.CreateFromQuaternion(rotacion) *
                Matrix.CreateTranslation(115f, 0f, -533f);
            ModelosUsados.Add(new Modelo(CarModel, matrizInicial, Color.Blue, CarTextures));


            
            rotacion = Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathHelper.Pi / 8);
            matrizInicial = Matrix.CreateScale(0.2f) *
                Matrix.CreateFromQuaternion(rotacion) *
                Matrix.CreateTranslation(-81f, 0f, 159f);
            ModelosUsados.Add(new Modelo(CarModel, matrizInicial, Color.Green, CarTextures));


            rotacion = Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathHelper.Pi / 2);
            matrizInicial = Matrix.CreateScale(0.2f) *
                Matrix.CreateFromQuaternion(rotacion) *
                Matrix.CreateTranslation(-80f, 0f, -160f);
            ModelosUsados.Add(new Modelo(CarModel, matrizInicial, Color.GreenYellow, CarTextures));


            matrizInicial = Matrix.CreateScale(0.01f) *
                Matrix.CreateFromQuaternion(rotacion) *
                Matrix.CreateTranslation(-200f, 0f, -120f);
            ModelosUsados.Add(new Modelo(battleCarModel, matrizInicial, Color.Orange, BattleCarTextures));


            //quad
            quad = new QuadPrimitive(GraphicsDevice);

            //walls
            var scale = new Vector3(800f, 1f, 100f);
            var PosNuevaPared = new Matrix();
            PosNuevaPared = Matrix.CreateScale(scale) * Matrix.CreateRotationX(MathHelper.PiOver2) * Matrix.CreateTranslation(-Vector3.UnitZ * 800f + Vector3.UnitY * 100f);
            Walls.Add(new Pared(PosNuevaPared, WallTexture));
            PosNuevaPared = Matrix.CreateScale(scale) * Matrix.CreateRotationX(-MathHelper.PiOver2) * Matrix.CreateTranslation(Vector3.UnitZ * 800f + Vector3.UnitY * 100f);
            Walls.Add(new Pared(PosNuevaPared, WallTexture));
            scale = new Vector3(100, 1f, 800);
            PosNuevaPared = Matrix.CreateScale(scale) * Matrix.CreateRotationZ(-MathHelper.PiOver2) * Matrix.CreateTranslation(-Vector3.UnitX * 800f + Vector3.UnitY * 100f);
            Walls.Add(new Pared(PosNuevaPared, WallTexture));
            PosNuevaPared = Matrix.CreateScale(scale) * Matrix.CreateRotationZ(MathHelper.PiOver2) * Matrix.CreateTranslation(Vector3.UnitX * 800f + Vector3.UnitY * 100f);
            Walls.Add(new Pared(PosNuevaPared, WallTexture));

            viewProjection = new Matrix();

            CantidadModelos = ModelosUsados.Count;

            IndexListaModelo = 0;

            base.LoadContent();
        }

        /// <summary>
        ///     Se llama en cada frame.
        ///     Se debe escribir toda la logica de computo del modelo, asi como tambien verificar entradas del usuario y reacciones
        ///     ante ellas.
        /// </summary>
        protected override void Update(GameTime gameTime)
        {

            KeyboardState estadoTeclado = Keyboard.GetState();

            FreeCamera.Update(gameTime);

            if (estadoTeclado.IsKeyDown(Keys.Escape))
                Exit();

            if (estadoTeclado.IsKeyDown(Keys.F1))
                tipoDeCamara = TipoDeCamara.ORIGINAL_SCENE;

            if (estadoTeclado.IsKeyDown(Keys.F2))
                tipoDeCamara = TipoDeCamara.FREE_VIEW;

            if (estadoTeclado.IsKeyDown(Keys.M)) {
                if (CantidadModelos - 1 > IndexListaModelo)
                    IndexListaModelo++;
                else
                    IndexListaModelo = 0;
            }

            ModelosUsados[IndexListaModelo].Update(gameTime, TexturaDeAutoEnEdicion);

            // Basado en el tiempo que paso se va generando una rotacion.
            Rotation += Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);

            base.Update(gameTime);
        }

        /// <summary>
        ///     Se llama cada vez que hay que refrescar la pantalla.
        ///     Escribir aqui el codigo referido al renderizado.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            // Aca deberiamos poner toda la logia de renderizado del juego.
            GraphicsDevice.Clear(Color.Black);

            //Actualizo projeccion segun tipo de camara
            if (tipoDeCamara == TipoDeCamara.ORIGINAL_SCENE) {
                Effect.Parameters["View"].SetValue(TargetCamera.View);
                Effect.Parameters["Projection"].SetValue(TargetCamera.Projection);
                viewProjection = TargetCamera.View * TargetCamera.Projection;
            }

            if (tipoDeCamara == TipoDeCamara.FREE_VIEW) {
                Effect.Parameters["View"].SetValue(FreeCamera.View);
                Effect.Parameters["Projection"].SetValue(FreeCamera.Projection);
                viewProjection = FreeCamera.View * FreeCamera.Projection;
            }

            TilingEffect.Parameters["WorldViewProjection"].SetValue(Matrix.CreateScale(800) * viewProjection);
            TilingEffect.Parameters["Texture"].SetValue(FloorTexture);
            quad.Draw(TilingEffect);

            TilingEffect.Parameters["Texture"].SetValue(WallTexture);
            TilingEffect.Parameters["WorldViewProjection"].SetValue(Walls[0].getWorldMatrix() * viewProjection);
            quad.Draw(TilingEffect);
            TilingEffect.Parameters["WorldViewProjection"].SetValue(Walls[1].getWorldMatrix() * viewProjection);
            quad.Draw(TilingEffect);
            TilingEffect.Parameters["WorldViewProjection"].SetValue(Walls[2].getWorldMatrix() * viewProjection);
            quad.Draw(TilingEffect);
            TilingEffect.Parameters["WorldViewProjection"].SetValue(Walls[3].getWorldMatrix() * viewProjection);
            quad.Draw(TilingEffect);

            foreach (var auto in ModelosUsados)
            {
                auto.Draw(Effect);
            }

            //Esto es para escribir el texto que aparece en la pantalla con la ubicacion del modelo
            ubicacionModelo = ModelosUsados[IndexListaModelo].getWorldMatrixAsString();

            SpriteBatch.Begin();
            SpriteBatch.DrawString(spriteFont, "MODELO ACTUAL: " + IndexListaModelo.ToString() 
                + '\n' + ubicacionModelo, new Vector2(0, 0), Color.Magenta);
            SpriteBatch.End();
        }

        /// <summary>
        ///     Libero los recursos que se cargaron en el juego.
        /// </summary>
        protected override void UnloadContent()
        {
            // Libero los recursos.
            Content.Unload();

            base.UnloadContent();
        }

    }
}