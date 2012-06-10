using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CercaPronto_3D
{
    class Uni
    {
        private int id;
        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        
        private string codigoIdeal;
        public string CodigoIdeal
        {
            get { return codigoIdeal; }
            set { codigoIdeal = value; }
        }
        
        private string descripcion;
        public string Descripcion
        {
            get { return descripcion; }
            set { descripcion = value; }
        }
        
        private float pvpTeorico;
        public float PvpTeorico
        {
            get { return pvpTeorico; }
            set { pvpTeorico = value; }
        }
    }
}
