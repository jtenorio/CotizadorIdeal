using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CercaPronto_3D
{
    public partial class Pruebas : Form
    {
        public Pruebas()
        {
            InitializeComponent();
        }

        private void Pruebas_Load(object sender, EventArgs e)
        {
            List<PlantillaPlaca> lista = new List<PlantillaPlaca>();
            DataBase db = new DataBase();
            //lista = db.getPlantillaPlaca(5);
            //MessageBox.Show(lista[1].IdUnitario.ToString());
            //MessageBox.Show("hola");
            dataGridView1.DataSource = db.getPlantillaPlaca(5);
            dataGridView1.DataSource = db.getUnitario(26);
            
        }
    }
}
