using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CercaPronto_3D
{
    class PlantillaMalla
    {
        private int idPlantillaMalla;

        public int IdPlantillaMalla
        {
            get { return idPlantillaMalla; }
            set { idPlantillaMalla = value; }
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
        private int cantidad;

        public int Cantidad
        {
            get { return cantidad; }
            set { cantidad = value; }
        }
    }
}
