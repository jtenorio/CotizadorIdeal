using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CercaPronto_3D
{
    class PlantillaEmpotrado
    {
        private int idPlantillaEmpotrado;

        public int IdPlantillaEmpotrado
        {
            get { return idPlantillaEmpotrado; }
            set { idPlantillaEmpotrado = value; }
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

        public PlantillaEmpotrado() { }
    }
}
