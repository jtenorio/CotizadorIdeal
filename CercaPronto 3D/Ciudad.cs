using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CercaPronto_3D
{
    class Ciudad
    {
        private int id;

        private string origen;
      
        private string destino;

        private float distancia;

        public Ciudad() { }

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public string Origen
        {
            get { return origen; }
            set { origen = value; }
        }

        public float Distancia
        {
            get { return distancia; }
            set { distancia = value; }
        }

        public string Destino
        {
            get { return destino; }
            set { destino = value; }
        }
    }
}
