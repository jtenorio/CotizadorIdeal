using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CercaPronto_3D
{
    class PlantillaPlaca
    {
        private int idPlantillaPlaca;

        public int IdPlantillaPlaca
        {
            get { return idPlantillaPlaca; }
            set { idPlantillaPlaca = value; }
        }
        private int idPlantilla;

        public int IdPlantilla
        {
            get { return idPlantilla; }
            set { idPlantilla = value; }
        }
        private int idUnitario;

        public int IdUnitario
        {
            get { return idUnitario; }
            set { idUnitario = value; }
        }
        private float cantidad;

        public float Cantidad
        {
            get { return cantidad; }
            set { cantidad = value; }
        }

        public PlantillaPlaca() { }
    }
}
