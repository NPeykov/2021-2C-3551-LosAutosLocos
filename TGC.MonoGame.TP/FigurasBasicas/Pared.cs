using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Linq;

namespace TGC.MonoGame.TP.FigurasBasicas
{
    class Pared
    {
        private Matrix WorldMatrix { get; set; }
        private Texture2D Texture { get; set; }

        public Pared(Matrix MatrizMundo, Texture2D TexturaModelo) {
            WorldMatrix = MatrizMundo;
            Texture = TexturaModelo;
            //FALTARIA EL AABB PARA COLISIONES
        }

        public Matrix getWorldMatrix() {
            return WorldMatrix;
        }
    }
}
