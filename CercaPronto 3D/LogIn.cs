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
    public partial class LogIn : Form
    {
        bool flag;
        Funciones funcion = new Funciones();

        public LogIn()
        {
            InitializeComponent();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void LogIn_Load(object sender, EventArgs e)
        {
            string strFileName = String.Empty;
            flag = funcion.inicializar();
            if (!flag)
            {
                MessageBox.Show("Archivo de Configuración no encontrado", "Cerca Pronto 3D", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
                dialog.InitialDirectory = "c:";
                dialog.Title = "Cargar archivo de configuración";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    strFileName = dialog.FileName;
                    if (!funcion.copiarXml(strFileName))
                    {
                        MessageBox.Show("Error al cargar el archivo", "Cerca Pronto 3D", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Application.Exit();
                    }

                }
                if (strFileName == String.Empty) { Application.Exit(); }
                return;
            }
            else { flag = funcion.primeraVez(); }
        }

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            bool ingreso = false;
            string mensaje;
            if (!flag) ingreso = funcion.login(txtUsuario.Text,txtPswd.Text);
            else
            {
                mensaje = funcion.guardarUsuario(txtUsuario.Text, txtPswd.Text);
                MessageBox.Show(mensaje, "Ingreso Cerca Pronto 3D", MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (mensaje == "Credenciales de usuario guardadas en el terminal, por favor vuelva a ingresar.")
                {
                    borrar();
                    Application.Exit();
                }
                else
                {
                    ingreso = false;
                    MessageBox.Show(mensaje, "Ingreso Cerca Pronto 3D", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    borrar();
                }
            }
            if (ingreso)
            {
                ////Cotizador cotizador = new Cotizador();
                //cotizador.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Credenciales Incorrectas.", "Ingreso Cerca Pronto 3D", MessageBoxButtons.OK, MessageBoxIcon.Information);
                borrar();
            }
        }
        private void borrar()
        {
            txtUsuario.Text = "";
            txtPswd.Text = "";
        }
    }
}
