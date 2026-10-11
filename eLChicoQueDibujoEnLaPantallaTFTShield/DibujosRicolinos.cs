using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace eLChicoQueDibujoEnLaPantallaTFTShield
{
    public class DibujosRicolinos
    {
        public int capa;
        public String Forma;
        public Vector2 posicioninicial;
        public Vector2 tamano; //maldito teclado Ingles ):<
        public Vector4 color;
        public DibujosRicolinos(string forma, Vector2 posinicial, Vector2 tam, int CAPA, Vector4 colorcito) 
        {
            Forma = forma;
            posicioninicial = posinicial;
            tamano = tam;
            capa = CAPA;
            color = colorcito;
        }
    }
}
