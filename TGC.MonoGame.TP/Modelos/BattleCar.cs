using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TGC.MonoGame.TP.Cameras;
using TGC.MonoGame.TP.FigurasBasicas;
using TGC.MonoGame.TP.Modelos;

namespace TGC.MonoGame.TP.Modelos
{
    class BattleCar
    {
        Matrix WorldMatrix { get; set; }
        Matrix World { get; set; }
        ContentManager ContentManager { get; set; }
        Color ColorModelo { get; set; }
        Texture2D BattleCarTexture { get; set; }
        Effect Efecto { get; set; }
        Model BattleCarModel { get; set; }

        public BattleCar(ContentManager content, Matrix MatrizMundo, Color Color) {
            WorldMatrix = MatrizMundo;
            ContentManager = content;
            ColorModelo = Color;

            BattleCarModel = ContentManager.Load<Model>("Models/CombatVehicle/Vehicle");
            BattleCarTexture = ContentManager.Load<Texture2D>("Textures/BattleCarTextures/Tex_0006_6");
            Efecto = ContentManager.Load<Effect>("Effects/BasicShader");


            foreach (var mesh in BattleCarModel.Meshes)
            {
                foreach (var meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = Efecto;
                }
            }
        }

        public void Draw() {
            Efecto.Parameters["ModelTexture"].SetValue(BattleCarTexture);

            foreach (var mesh in BattleCarModel.Meshes)
            {
                World = mesh.ParentBone.Transform * WorldMatrix;

                Efecto.Parameters["World"].SetValue(World);
                mesh.Draw();
            }
        }

    }
}
