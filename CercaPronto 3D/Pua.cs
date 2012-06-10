using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CercaPronto_3D
{
    class Pua
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
        
        private float precio;
        public float Precio
        {
            get { return precio; }
            set { precio = value; }
        }
        
        private float medida;
        public float Medida
        {
            get { return medida; }
            set { medida = value; }
        }

        public Pua() { }
    }
}
