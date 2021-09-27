using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TGC.MonoGame.TP.Cameras;
using TGC.MonoGame.TP.FigurasBasicas;

namespace TGC.MonoGame.TP.Modelos
{

    class AutoNormal : Modelo
    {

        private Matrix World;

        public AutoNormal(Model modelo, Matrix matriz, Color colorModelo) {
            MatrizMundo = matriz;
            MiModelo = modelo;
            ColorModelo = colorModelo;
        }


    }
}
