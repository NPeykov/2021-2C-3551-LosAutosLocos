using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TGC.MonoGame.TP.Cameras;
using TGC.MonoGame.TP.FigurasBasicas;
using TGC.MonoGame.TP.Modelos;

namespace TGC.MonoGame.TP.Modelos
{
    abstract class Modelo
    {
        public Matrix MatrizMundo { get; set; }

        public Model MiModelo;

        public Color ColorModelo;

        private Boolean EstaSeleccionado = false;

        private Matrix World;

        private float VelocidadTranslacion;

        public void Update(GameTime gameTime)
        {
            EstaSeleccionado = true;

            var elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            ActualizarPosicion(elapsedTime);
        }

        public void ActualizarPosicion(float elapsedTime) {
            KeyboardState estadoTeclado = Keyboard.GetState();

            if (estadoTeclado.IsKeyDown(Keys.LeftShift))
                VelocidadTranslacion = elapsedTime * 200;
            else
                VelocidadTranslacion = elapsedTime * 50;

            //TRANSLACIONES
            if (estadoTeclado.IsKeyDown(Keys.D))
                MatrizMundo *= Matrix.CreateTranslation(MatrizMundo.Right * VelocidadTranslacion);
            if (estadoTeclado.IsKeyDown(Keys.A))
                MatrizMundo *= Matrix.CreateTranslation(MatrizMundo.Left * VelocidadTranslacion);
            if (estadoTeclado.IsKeyDown(Keys.W))
                MatrizMundo *= Matrix.CreateTranslation(MatrizMundo.Up * VelocidadTranslacion);
            if (estadoTeclado.IsKeyDown(Keys.S))
                MatrizMundo *= Matrix.CreateTranslation(MatrizMundo.Down * VelocidadTranslacion);
            if (estadoTeclado.IsKeyDown(Keys.Q))
                MatrizMundo *= Matrix.CreateTranslation(MatrizMundo.Forward * VelocidadTranslacion);
            if (estadoTeclado.IsKeyDown(Keys.E))
                MatrizMundo *= Matrix.CreateTranslation(MatrizMundo.Backward * VelocidadTranslacion);

            //ESCALADOS
            if (estadoTeclado.IsKeyDown(Keys.Subtract) && estadoTeclado.IsKeyDown(Keys.LeftShift))
                MatrizMundo = Matrix.CreateScale(elapsedTime * 50) * MatrizMundo;

            if (estadoTeclado.IsKeyDown(Keys.Add) && estadoTeclado.IsKeyDown(Keys.LeftShift))
                MatrizMundo = Matrix.CreateScale(1 / (elapsedTime * 50)) * MatrizMundo;


            //ROTACIONES
            if (estadoTeclado.IsKeyDown(Keys.Z)) {
                if (estadoTeclado.IsKeyDown(Keys.Add))
                    MatrizMundo =
                    Matrix.CreateFromQuaternion(Quaternion.CreateFromAxisAngle(Vector3.UnitZ, elapsedTime))
                    * MatrizMundo;
                if (estadoTeclado.IsKeyDown(Keys.Subtract))
                    MatrizMundo =
                    Matrix.CreateFromQuaternion(Quaternion.CreateFromAxisAngle(Vector3.UnitZ, -elapsedTime))
                    * MatrizMundo;
            }



            if (estadoTeclado.IsKeyDown(Keys.X)) {
                if (estadoTeclado.IsKeyDown(Keys.Add))
                    MatrizMundo =
                    Matrix.CreateFromQuaternion(Quaternion.CreateFromAxisAngle(Vector3.UnitX, elapsedTime))
                    * MatrizMundo;
                if (estadoTeclado.IsKeyDown(Keys.Subtract))
                    MatrizMundo =
                    Matrix.CreateFromQuaternion(Quaternion.CreateFromAxisAngle(Vector3.UnitX, -elapsedTime))
                    * MatrizMundo;
            }

            if (estadoTeclado.IsKeyDown(Keys.C)) {
                if (estadoTeclado.IsKeyDown(Keys.Add))
                    MatrizMundo =
                    Matrix.CreateFromQuaternion(Quaternion.CreateFromAxisAngle(Vector3.UnitY, elapsedTime))
                    * MatrizMundo;
                if (estadoTeclado.IsKeyDown(Keys.Subtract))
                    MatrizMundo =
                    Matrix.CreateFromQuaternion(Quaternion.CreateFromAxisAngle(Vector3.UnitY, -elapsedTime))
                    * MatrizMundo;
            }
        }

        public void Draw(Effect efecto)
        {
            if(EstaSeleccionado)
                efecto.Parameters["DiffuseColor"].SetValue(Color.Magenta.ToVector3());
            else
                efecto.Parameters["DiffuseColor"].SetValue(ColorModelo.ToVector3());

            foreach (var mesh in MiModelo.Meshes)
            {
                World = mesh.ParentBone.Transform * MatrizMundo;

                efecto.Parameters["World"].SetValue(World);
                mesh.Draw();
            }

            EstaSeleccionado = false;
        }

        public Matrix getWorldMatrix()
        {
            return this.MatrizMundo;
        }

        public String getWorldMatrixAsString()
        {
            return this.MatrizMundo.ToString().Replace('{', '\n').Replace('}', ' ');
        }
    }

}
