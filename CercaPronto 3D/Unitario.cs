using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CercaPronto_3D
{
    class Unitario
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

        private float cantidad;

        public float Cantidad
        {
            get { return cantidad; }
            set { cantidad = value; }
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

        private bool poste;

        public bool Poste
        {
            get { return poste; }
            set { poste = value; }
        }

        private bool brazo;

        public bool Brazo
        {
            get { return brazo; }
            set { brazo = value; }
        }

        private bool puas;

        public bool Puas
        {
            get { return puas; }
            set { puas = value; }
        }

        private bool puerta;

        public bool Puerta
        {
            get { return puerta; }
            set { puerta = value; }
        }

        private bool adicional;

        public bool Adicional
        {
            get { return adicional; }
            set { adicional = value; }
        }

        private bool corrugado;

        public bool Corrugado
        {
            get { return corrugado; }
            set { corrugado = value; }
        }

        private bool fijacion;

        public bool Fijacion
        {
            get { return fijacion; }
            set { fijacion = value; }
        }

        private bool tuerca;

        public bool Tuerca
        {
            get { return tuerca; }
            set { tuerca = value; }
        }

        private bool jumbo;

        public bool Jumbo
        {
            get { return jumbo; }
            set { jumbo = value; }
        }

        private bool tapa;

        public bool Tapa
        {
            get { return tapa; }
            set { tapa = value; }
        }

        private bool perno;

        public bool Perno
        {
            get { return perno; }
            set { perno = value; }
        }

        private string unidad;

        public string Unidad
        {
            get { return unidad; }
            set { unidad = value; }
        }

        public Unitario() { }
    }
}
