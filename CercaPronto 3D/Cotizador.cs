using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using iTextSharp.text.pdf;
using System.IO;

namespace CercaPronto_3D
{
    public partial class Cotizador : Form
    {
        # region variables
            int placa = 0;
            int instala = 0;
            int gradoEscalonamiento = 0;

            int idAltura = 0;
            int idColor = 0;
            int idPlantilla = 0;

            double longuitud = 0;
            float subTotal = 0;

            int auxIdAdic = 0;
       
            bool flagAltura = false;
            bool flagColor = false;
            bool flagPlaca = false;
            bool flagInstalacion = false;
            bool flagFirst = true;
    
            DataBase db = new DataBase();
            Unitario auxU = new Unitario();

            List<Unitario> elementos = new List<Unitario>();
            List<Unitario> adicionales = new List<Unitario>();
            List<Unitario> auxAdicionales = new List<Unitario>();
            List<Unitario> puertas = new List<Unitario>(); 
            List<Unitario> vista = new List<Unitario>();
            List<Uni> FactorI = new List<Uni>();
            List<Uni> FactorS = new List<Uni>();
            Unitario MO = new Unitario();
         
        # endregion 

        public Cotizador()
        {
            InitializeComponent();
        }

        private void cmbColor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (flagColor)
            {
                if (cmbColor.SelectedIndex != -1)
                {
                    idColor = Convert.ToInt16(cmbColor.SelectedValue.ToString());
                    flagColor = false;
                }
            }
        }

        private void cmbColor_SelectionChangeCommitted(object sender, EventArgs e)
        {
            flagColor = true;
        }

        private void cmbAltura_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (flagAltura)
            {
                if (cmbAltura.SelectedIndex != -1)
                {
                    idAltura = Convert.ToInt16(cmbAltura.SelectedValue.ToString());
                    flagAltura = false;
                }
            }
        }

        private void cmbAltura_SelectionChangeCommitted(object sender, EventArgs e)
        {
            flagAltura = true;
        }

        private void cmbPlaca_SelectionChangeCommitted(object sender, EventArgs e)
        {
            placa = cmbPlaca.SelectedIndex;
            flagPlaca = true;
        }

        private void cmbPlaca_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (flagPlaca)
            {
                
                
                    if (cmbPlaca.SelectedIndex == 1) { placa = 1;}
                    if (cmbPlaca.SelectedIndex == 0) { placa = 0; }
                    flagPlaca = false;
            }
        }

        private void cmbInstalacion_SelectionChangeCommitted(object sender, EventArgs e)
        {
            instala = cmbInstalacion.SelectedIndex;
            flagInstalacion = true;
        }

        private void cmbInstalacion_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (flagInstalacion)
            {


                if (cmbInstalacion.SelectedIndex == 1) { instala = 1; }
                if (cmbInstalacion.SelectedIndex == 0) { instala = 0; }
                flagPlaca = false;
            }
        }

        private void btnValidar_Click(object sender, EventArgs e)
        {
            if (validarPlantilla())
            {
                if (cmbPlaca.SelectedIndex == 1) { placa = 1; }
                if (cmbPlaca.SelectedIndex == 0) { placa = 0; }

                if (placa == 1)
                {
                    if (!validarPlantillaPlaca())
                    {
                        bloqueo(1);
                        MessageBox.Show("Combinación Inexistente para Placa", "Cerca Pronto 3D", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else bloqueo(0);
                }

                if (placa == 0)
                {
                    if (!validarPlantillaEmpotrado())
                    {
                        bloqueo(1);
                        MessageBox.Show("Combinación Inexistente para Empotrado", "Cerca Pronto 3D", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else bloqueo(0);
                }
            }
            else 
            {
                bloqueo(1);
                MessageBox.Show("Combinación Inexistente", "Cerca Pronto 3D", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        private void btnContinuar_Click(object sender, EventArgs e)
        {
            try
            {
                longuitud = double.Parse(txtLong.Text);
            }
            catch(Exception exc)
            {
                MessageBox.Show("Longuitud Incorrecta", "Cerca Pronto 3D", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            if (longuitud != 0)
            {
                if (flagFirst)
                {
                    cargarDatos();
                    longuitud = double.Parse(txtLong.Text);
                    aplicarTramos();
                    if (cmbInstalacion.Text == "Si") instalacion(idPlantilla);
                }
                recalcular();
                addAdicionales();
                calcularSubTotal();
                actualizarDGV();
                flagFirst = false;
                gbMain.Enabled = false;
                gbInstalación.Enabled = false;
                gbLongitud.Enabled = false;
                gbBrazo.Enabled = false;
                tabWorkArea.SelectTab(tabUnitarios);
            }
            else
            {
                MessageBox.Show("Longuitud Incorrecta", "Cerca Pronto 3D", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void Cotizador_Load(object sender, EventArgs e)
        {
            DataTable alturas = db.getAlturas();
            DataTable colores = db.getColores();

            getFactorInstalacion();

            this.cmbAltura.DataSource = alturas;
            this.cmbAltura.DisplayMember = "medida";
            this.cmbAltura.ValueMember = "idAltura";

            this.cmbColor.DataSource = colores;
            this.cmbColor.DisplayMember = "nombre";
            this.cmbColor.ValueMember = "idColor";

            this.cmbCiudad.DataSource = getDestino().ToList(); 
            this.cmbDespacho.DataSource = getOrigen().ToList();
            this.cmbDestino.DataSource = getDestino().ToList();

            this.cmbFactorInstalacion.DataSource = FactorI;
            this.cmbFactorInstalacion.DisplayMember = "descripcion";
            this.cmbFactorInstalacion.ValueMember = "id";

            this.cmbColor.SelectedItem = 0;
            this.cmbAltura.SelectedItem = 0;

            this.cmbPlaca.SelectedItem = 0;
            this.cmbPlaca.SelectedIndex = 0;

            this.cmbGrados.SelectedItem = 0;
            this.cmbGrados.SelectedIndex = 0;

            this.cmbCiudad.SelectedItem = 0;
            this.cmbCiudad.SelectedIndex = 0;

            this.cmbDespacho.SelectedItem = 0;
            this.cmbDespacho.SelectedIndex = 0;

            this.cmbDestino.SelectedItem = 0;
            this.cmbDestino.SelectedIndex = 0;

            this.cmbInstalacion.SelectedItem = 0;
            this.cmbInstalacion.SelectedIndex = 0;

            idColor = Convert.ToInt16(cmbColor.SelectedValue.ToString());
            idAltura = Convert.ToInt16(cmbAltura.SelectedValue.ToString());

            getFecha();

            lblFecha.Text = db.getFecha();

            //bloqueo de inicio
            bloqueo(1);

            actualizarDGVAdicionales();
        }

        private void tabWorkArea_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (flagUnitarios == true && tabWorkArea.SelectedTab == tabUnitarios)
            //{
            //    tabWorkArea.SelectTab(tabUnitarios);
            //}
            //else
            //{
            //    tabWorkArea.SelectTab(tabRequerimientos);
            //}
        }

        private void dataGridUnitarios_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            string auxId;
          
            auxId = dataGridUnitarios.Rows[e.RowIndex].Cells[0].FormattedValue.ToString();
            auxU = buscar(int.Parse(auxId));

            gbModificarUnitario.Visible = true;
            txtCantidad.Text = auxU.Cantidad.ToString();
            txtDescripcion.Text = auxU.Descripcion;
        }

        private void dgAdicionales_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            string auxId;
            auxId = dgAdicionales.Rows[e.RowIndex].Cells[0].FormattedValue.ToString();
            auxIdAdic = int.Parse(auxId);
            txtCantAdicional.Text = buscarAdicional(auxIdAdic).Cantidad.ToString();
            txtDescripAdicional.Text = buscarAdicional(auxIdAdic).Descripcion;
        }

        private void dgResumen_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            string auxId;
            auxId = dgResumen.Rows[e.RowIndex].Cells[0].FormattedValue.ToString();
            auxIdAdic = int.Parse(auxId);
            gbElimAdicional.Visible = true;
            txtResumenCantidad.Text = buscarAdicional(auxIdAdic).Cantidad.ToString();
            txtResumenDetalle.Text = buscarAdicional(auxIdAdic).Descripcion;
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            gbModificarUnitario.Visible = false;
        }

        private void btnMas_Click(object sender, EventArgs e)
        {
            int cantidad;
            cantidad = int.Parse(txtCantidad.Text);
            cantidad++;
            txtCantidad.Text = cantidad.ToString();
        }

        private void btnMenos_Click(object sender, EventArgs e)
        {
            int cantidad;
            cantidad = int.Parse(txtCantidad.Text);
            if (cantidad == 0) cantidad = 0;
            else
            {
                cantidad--;
                txtCantidad.Text = cantidad.ToString();
            }
            
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            auxU.Cantidad = int.Parse(txtCantidad.Text);
            foreach (Unitario u in elementos)
            {
                if (u.Id == auxU.Id)
                {
                    u.Cantidad = auxU.Cantidad;
                }
            }
            gbModificarUnitario.Visible = false;
            recalcular();
            actualizarDGV();
            calcularSubTotal();
        }

        private void monthCalendar1_DateSelected(object sender, DateRangeEventArgs e)
        {
            getFecha();
        }

        private void rbBrazoSi_CheckedChanged(object sender, EventArgs e)
        {
            if (rbBrazoSi.Checked)
            {
                gbPuas.Visible = true;
            }
        }

        private void rbBrazoNo_CheckedChanged(object sender, EventArgs e)
        {
            if (rbBrazoNo.Checked)
            {
                gbPuas.Visible = false;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            bool flag = true;
            Unitario adic = new Unitario();

            adic = buscarAdicional(auxIdAdic);

            foreach (Unitario u in auxAdicionales)
            {
                if (adic.Id == u.Id)
                {
                    flag = false;
                    MessageBox.Show("Unitario ya agregado", "Cerca Pronto 3D", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

            if (flag)
            {
                if (!adic.Puerta)
                {
                    adic.Cantidad = int.Parse(txtCantAdicional.Text);
                    adic.Precio = adic.Precio * adic.Cantidad;
                    auxAdicionales.Add(adic);
                }
                else
                {
                    adic.Cantidad = int.Parse(txtCantAdicional.Text);
                    adic.Precio = adic.Precio * adic.Cantidad;
                    puertas.Add(adic);
                }
            }

            BindingSource source = new BindingSource();
            source.DataSource = auxAdicionales.AsEnumerable();

            dgResumen.DataSource = source;
            dgResumen.Columns["id"].Visible = false;
            dgResumen.Columns["poste"].Visible = false;
            dgResumen.Columns["brazo"].Visible = false;
            dgResumen.Columns["puas"].Visible = false;
            dgResumen.Columns["puerta"].Visible = false;
            dgResumen.Columns["adicional"].Visible = false;
            dgResumen.Columns["corrugado"].Visible = false;
            dgResumen.Columns["fijacion"].Visible = false;
            dgResumen.Columns["tapa"].Visible = false;
            dgResumen.Columns["perno"].Visible = false;
            dgResumen.Columns["tuerca"].Visible = false;
            dgResumen.Columns["jumbo"].Visible = false;
            dgResumen.Refresh();
           
        }

        private void btnAdicCancelar_Click(object sender, EventArgs e)
        {
            gbElimAdicional.Visible = false;
        }

        private void btnAdicEliminar_Click(object sender, EventArgs e)
        {

            foreach (Unitario i in auxAdicionales)
            {
                if (i.Id == auxIdAdic)
                {
                    auxAdicionales.Remove(i);
                    break;
                }
            }
            gbElimAdicional.Visible = false;
            BindingSource source = new BindingSource();
            source.DataSource = auxAdicionales.AsEnumerable();

            dgResumen.DataSource = source;
            dgResumen.Columns["id"].Visible = false;
            dgResumen.Columns["poste"].Visible = false;
            dgResumen.Columns["brazo"].Visible = false;
            dgResumen.Columns["puas"].Visible = false;
            dgResumen.Columns["puerta"].Visible = false;
            dgResumen.Columns["adicional"].Visible = false;
            dgResumen.Refresh();
        }

        # region Funciones

            private void cargarDatos()
            {
                //busca y agrega mallas
                List<PlantillaMalla> malla = new List<PlantillaMalla>();
                malla = db.getMalla(idPlantilla);
                
                foreach (PlantillaMalla p in malla)
                {
                    Unitario uni = new Unitario();
                    List <Unitario> mallas = new List<Unitario>();

                    mallas = db.getUnitario(p.IdUnitario);
                    foreach (Unitario u in mallas)
                    {
                        uni.Cantidad = p.Cantidad;
                        uni.Descripcion = u.Descripcion;
                        uni.Precio = u.Precio * uni.Cantidad;
                        uni.Id = u.Id;
                        uni.CodigoIdeal = u.CodigoIdeal;
                        uni.Poste = u.Poste;
                        uni.Adicional = u.Adicional;
                        uni.Brazo = u.Brazo;
                        uni.Corrugado = u.Corrugado;
                        uni.Fijacion = u.Fijacion;
                        uni.Perno = u.Perno;
                        uni.Puerta = u.Puerta;
                        uni.Tapa = u.Tapa;
                        uni.Jumbo = u.Jumbo;
                        uni.Tuerca = u.Tuerca;
                        uni.Unidad = u.Unidad;
                        elementos.Add(uni);
                    }
                }
                //carga plantillas
                if (placa == 1) // con placa
                {
                    List<PlantillaPlaca> plantilla = db.getPlantillaPlaca(idPlantilla);
                    foreach (PlantillaPlaca p in plantilla)
                    {
                        List<Unitario> aux = new List<Unitario>();
                        Unitario uni = new Unitario();
                   
                        uni.Cantidad = p.Cantidad;
                        aux = db.getUnitario(p.IdUnitario);
                        foreach (Unitario u in aux)
                        {
                            uni.Descripcion = u.Descripcion;
                            uni.Precio = u.Precio * uni.Cantidad;
                            uni.Id = u.Id;
                            uni.CodigoIdeal = u.CodigoIdeal;
                            uni.Poste = u.Poste;
                            uni.Adicional = u.Adicional;
                            uni.Brazo = u.Brazo;
                            uni.Corrugado = u.Corrugado;
                            uni.Fijacion = u.Fijacion;
                            uni.Perno = u.Perno;
                            uni.Puerta = u.Puerta;
                            uni.Tapa = u.Tapa;
                            uni.Jumbo = u.Jumbo;
                            uni.Tuerca = u.Tuerca;
                            uni.Unidad = u.Unidad;
                        }

                        if (uni.Brazo == true && rbBrazoSi.Checked) elementos.Add(uni);
                        if (uni.Jumbo == true && rbBrazoSi.Checked) elementos.Add(uni);

                        if (flagInstalacion == true && (uni.CodigoIdeal.Contains("MO")))
                        {
                            MO.Cantidad = 1;
                            MO.CodigoIdeal = uni.CodigoIdeal;
                            MO.Descripcion = uni.Descripcion;
                            MO.Id = uni.Id;
                            MO.Precio = uni.Precio;
                            //elementos.Add(uni);
                        }
                        if(uni.Brazo == false && uni.Jumbo == false && uni.Corrugado == false && !(uni.CodigoIdeal.Contains("MO"))) elementos.Add(uni);
                    }
                   
                }

                if (placa == 0)
                {
                    List<PlantillaEmpotrado> plantilla = db.getPlantillaEmpotrado(idPlantilla);
                    foreach (PlantillaEmpotrado p in plantilla)
                    {
                        List<Unitario> aux = new List<Unitario>();
                        Unitario uni = new Unitario();
                   
                        uni.Cantidad = p.Cantidad;
                        aux = db.getUnitario(p.IdUnitario);
                        foreach (Unitario u in aux)
                        {
                            uni.Descripcion = u.Descripcion;
                            uni.Precio = u.Precio * uni.Cantidad;
                            uni.Id = u.Id;
                            uni.CodigoIdeal = u.CodigoIdeal;
                            uni.Poste = u.Poste;
                            uni.Adicional = u.Adicional;
                            uni.Brazo = u.Brazo;
                            uni.Corrugado = u.Corrugado;
                            uni.Fijacion = u.Fijacion;
                            uni.Perno = u.Perno;
                            uni.Puerta = u.Puerta;
                            uni.Tapa = u.Tapa;
                            uni.Jumbo = u.Jumbo;
                            uni.Tuerca = u.Tuerca;
                            uni.Unidad = u.Unidad;
                        }

                        if (uni.Brazo == true && rbBrazoSi.Checked) elementos.Add(uni);
                        if (uni.Jumbo == true && rbBrazoSi.Checked) elementos.Add(uni);

                        if (flagInstalacion == true && (uni.CodigoIdeal.Contains("MO")))
                        {
                            MO.Cantidad = 1;
                            MO.CodigoIdeal = uni.CodigoIdeal;
                            MO.Descripcion = uni.Descripcion;
                            MO.Id = uni.Id;
                            MO.Precio = uni.Precio;
                            //elementos.Add(uni);
                        }
                        if (uni.Brazo == false && uni.Jumbo == false && !(uni.CodigoIdeal.Contains("MO"))) elementos.Add(uni);
                        
                    }
                }
            }

            private void aplicarTramos()
            {
                float cantidadO;
                double tramos;
                decimal adicional;

                tramos = longuitud / 2.53;
                tramos = Math.Ceiling(tramos);
            
                gradoEscalonamiento = int.Parse(cmbGrados.Text);

                foreach (Unitario u in elementos)
                {
                    if (!(u.CodigoIdeal.Contains("MO")))
                    {
                        cantidadO = u.Cantidad;
                        u.Cantidad = (float)tramos * u.Cantidad;
                        if (!(u.Unidad == "malla"))
                        {
                            adicional = (decimal)(u.Cantidad * (gradoEscalonamiento / 100.0));
                            u.Cantidad = (float)Math.Ceiling((decimal)u.Cantidad + adicional);
                            if (u.Poste == true) u.Cantidad = u.Cantidad + 1;
                            if (u.Tapa == true) u.Cantidad = u.Cantidad + 1;
                            if (u.Brazo == true && rbBrazoSi.Checked) u.Cantidad = u.Cantidad + 1;
                            if (u.Jumbo == true && rbBrazoSi.Checked) u.Cantidad = u.Cantidad + 3;
                            if (u.Fijacion == true) u.Cantidad = u.Cantidad + cantidadO;
                            if (u.Perno == true) u.Cantidad = u.Cantidad + cantidadO;
                            if (u.Tuerca == true) u.Cantidad = u.Cantidad + cantidadO;
                            //perno y tuerca si hay brazo
                            if (rbBrazoSi.Checked)
                            {
                                if (u.Perno == true)
                                {
                                    u.Cantidad = (float)tramos * (cantidadO + 1);
                                }
                                if (u.Tuerca == true)
                                {
                                    u.Cantidad = (float)tramos * (cantidadO + 1);
                                }
                            }
                            
                            //si no hay bazos
                            if (u.Brazo == true && !rbBrazoSi.Checked) u.Cantidad = 0;
                            if (u.Jumbo == true && !rbBrazoSi.Checked) u.Cantidad = 0;
                            // pendiente alambre corrugado
                            if (u.Corrugado == true && placa == 0) u.Cantidad = (float)Math.Ceiling(((u.Cantidad + 1) / 23));
                            if (u.Corrugado == true && placa == 1) u.Cantidad = 0;
                        }
                    }
                }
              
                if (rbPuaSi.Checked) calcularPuas();
            }

            private void calcularPuas()
            {
                float div = 1.0F;
                int index = 0;
                int intpart;
                float floatpart;
                float auxLong;
                int count;
                List<Pua> puas = new List<Pua>();
                puas = db.getPuas();
                var aux = from p in puas
                               orderby p.Medida descending
                               select p;
                Pua[] ordenado = aux.ToArray();
                auxLong = (float)longuitud * 3.0F;
                count = ordenado.Count(); 
                while (div > 0)
                {
                    div = auxLong / ordenado[index].Medida;
                    intpart = (int)div;
                    if (div >= 1)
                    {
                        Unitario Upua = new Unitario();
                        Upua.Id = ordenado[index].Id;
                        Upua.CodigoIdeal = ordenado[index].CodigoIdeal;
                        Upua.Descripcion = ordenado[index].Descripcion;
                        Upua.Cantidad = intpart;
                        elementos.Add(Upua);
                        auxLong = auxLong - (ordenado[index].Medida * intpart);
                        if (div == 1) break;
                    }

                    if ((index + 1) >= count)
                    {
                        Unitario Upua = new Unitario();
                        Upua.Id = ordenado[index].Id;
                        Upua.CodigoIdeal = ordenado[index].CodigoIdeal;
                        Upua.Descripcion = ordenado[index].Descripcion;
                        Upua.Cantidad = 1;
                        elementos.Add(Upua);
                        break;
                    }
                    index++;
                }
               
            }

            private bool validarPlantilla()
            {
                bool existe = false;
                DataTable aux = db.getIdPlantilla(idAltura, idColor);
                if (aux.Rows.Count > 0)
                {
                    idPlantilla = Convert.ToInt16(aux.Rows[0][0].ToString());
                    existe = true;
                }
                return existe;
            }

            private bool validarPlantillaPlaca()
            {
                bool existe = false;
                List<PlantillaPlaca> aux = db.getPlantillaPlaca(idPlantilla);
                if (aux.Count > 0) { existe = true; }
                return existe;
            }

            private bool validarPlantillaEmpotrado()
            {
                bool existe = false;
                List<PlantillaEmpotrado> aux = db.getPlantillaEmpotrado(idPlantilla);
                if (aux.Count > 0) { existe = true; }
                return existe;
            }

            private void recalcular()
            {
                    foreach (Unitario uni in elementos)
                    {
                        if (uni.CodigoIdeal != "MO000")
                        {
                            List<Unitario> aux = new List<Unitario>();
                            aux = db.getUnitario(uni.Id);
                            foreach (Unitario u in aux)
                            {
                                uni.Precio = uni.Cantidad * u.Precio;
                            }
                        }
                    }
            }

            private Unitario buscar(int id)
            {
                Unitario encontrado = new Unitario();
                foreach (Unitario u in elementos)
                {
                    if (u.Id == id)
                    {
                        encontrado.Adicional = u.Adicional;
                        encontrado.Brazo = u.Brazo;
                        encontrado.Cantidad = u.Cantidad;
                        encontrado.CodigoIdeal = u.CodigoIdeal;
                        encontrado.Corrugado = u.Corrugado;
                        encontrado.Descripcion = u.Descripcion;
                        encontrado.Id = u.Id;
                        encontrado.Jumbo = u.Jumbo;
                        encontrado.Perno = u.Perno;
                        encontrado.Poste = u.Poste;
                        encontrado.Precio = u.Precio;
                        encontrado.Puas = u.Puas;
                        encontrado.Puerta = u.Puerta;
                        encontrado.Tapa = u.Tapa;
                        encontrado.Tuerca = u.Tuerca;

                        return encontrado;
                    } 
                }
                return null;
            }

            private Unitario buscarAdicional(int id)
            {
                Unitario encontrado = new Unitario();
                foreach (Unitario u in adicionales)
                {
                    if (u.Id == id)
                    {
                        encontrado.Adicional = u.Adicional;
                        encontrado.Brazo = u.Brazo;
                        encontrado.Cantidad = u.Cantidad;
                        encontrado.CodigoIdeal = u.CodigoIdeal;
                        encontrado.Corrugado = u.Corrugado;
                        encontrado.Descripcion = u.Descripcion;
                        encontrado.Id = u.Id;
                        encontrado.Jumbo = u.Jumbo;
                        encontrado.Perno = u.Perno;
                        encontrado.Poste = u.Poste;
                        encontrado.Precio = u.Precio;
                        encontrado.Puas = u.Puas;
                        encontrado.Puerta = u.Puerta;
                        encontrado.Tapa = u.Tapa;
                        encontrado.Tuerca = u.Tuerca;
                        return encontrado;
                    }
                }
                return null;
            }

            private void instalacion(int idPlantilla)
            {
                double numSalidas = 0;
                int final = 0;
                float distancia = 0.0f;
                int idFactor = 0;
                string auxDist = "";
                float ValorxPlinto = 0;
                float factorSalida = 0;
                double adicional;
               
                double tramos;
                int numPlintos;

                float mo=0.0f;

                tramos = longuitud / 2.53;
                tramos = Math.Ceiling(tramos);
                numPlintos = int.Parse(tramos.ToString()) + 1;

               

                List<Uni> aux = db.getUnitarios();
                List<int> distancias = new List<int>();
                List<Ciudad> ciudades = db.getCiudades();
                Unitario instalacion = new Unitario();

                numSalidas = Math.Ceiling(longuitud / 60);

                //////
                if (placa == 1) // con placa
                {
                    List<PlantillaPlaca> plantilla = db.getPlantillaPlaca(idPlantilla);
                    foreach (PlantillaPlaca p in plantilla)
                    {
                        List<Unitario> auxU = new List<Unitario>();
                        auxU = db.getUnitario(p.IdUnitario);
                        foreach (Unitario u in auxU)
                        {
                            if (u.CodigoIdeal.Contains("MO"))
                            {
                                mo = u.Precio;
                            }
                        }
                    }
                }
               
                 if (placa == 0) // sin placa
                {
                     List<PlantillaEmpotrado> plantillaE = db.getPlantillaEmpotrado(idPlantilla);

                    foreach (PlantillaEmpotrado p in plantillaE)
                    {
                         List<Unitario> auxP = new List<Unitario>();
                        auxP = db.getUnitario(p.IdUnitario);
                        foreach(Unitario u in auxP)
                        {
                            if(u.CodigoIdeal.Contains("MO"))
                            {
                                mo = u.Precio;
                            }
                        }
                    }
                 }
                //////
                foreach (Uni u in aux)
                {
                    if (u.CodigoIdeal.Contains("FS"))
                    {
                        FactorS.Add(u);
                        final = u.CodigoIdeal.Length;
                        distancias.Add(int.Parse(u.CodigoIdeal.Substring(2, (final-2))));
                    }
                }

                foreach (Ciudad c in ciudades)
                {
                    if ((cmbDestino.Text == c.Destino) && (cmbDespacho.Text == c.Origen))
                    {
                        distancia = c.Distancia;
                    }
                }

                foreach (int d in distancias)
                {
                    if (distancia <= d)
                    {
                        auxDist = "FS" + d.ToString();
                        break;
                    }
                }

                foreach (Uni u in aux)
                {
                    if (u.CodigoIdeal.Equals(auxDist))
                    {
                        factorSalida = u.PvpTeorico;
                    }
                }

                ValorxPlinto = mo + ((float.Parse(numSalidas.ToString()) * factorSalida) / numPlintos);

                adicional = Math.Ceiling(numPlintos * (gradoEscalonamiento / 100.0));
                numPlintos = Convert.ToInt32(numPlintos + adicional);

                instalacion.Precio = (ValorxPlinto * numPlintos);
 
                idFactor = Convert.ToInt16(cmbFactorInstalacion.SelectedValue.ToString());

                if (idFactor != 0)
                {
                    foreach (Uni f in FactorI)
                    {
                        if (idFactor == f.Id) instalacion.Precio = instalacion.Precio * f.PvpTeorico;
                    }
                }
                instalacion.CodigoIdeal = "MO000";
                instalacion.Cantidad = 1;
                instalacion.Descripcion = "Instalación y Mano de Obra";
                elementos.Add(instalacion);
            }
                

            private void getFactorInstalacion()
            {
                List<Uni> aux = db.getUnitarios();
                Uni ninguno = new Uni();
                
                ninguno.CodigoIdeal = "FI000";
                ninguno.Descripcion = "Ninguno";
                ninguno.Id = 0;
                ninguno.PvpTeorico = 1;

                FactorI.Add(ninguno);
               
                foreach (Uni u in aux)
                {
                    if (u.CodigoIdeal.Contains("FI"))
                    {
                        FactorI.Add(u);
                    }
                }
            }

            private IEnumerable<string> getOrigen()
            {
                List<string> origen = new List<string>();
                List<Ciudad> aux = db.getCiudades();
                foreach (Ciudad c in aux)
                {
                    origen.Add(c.Origen);
                }
                IEnumerable<string> lista = origen.Distinct();
                return origen.Distinct();
            }

            private IEnumerable<string> getDestino()
            {
                List<string> destino = new List<string>();
                List<Ciudad> aux = db.getCiudades();
                foreach (Ciudad c in aux)
                {
                    destino.Add(c.Destino);
                }
                IEnumerable<string> lista = destino.Distinct();
                return destino.Distinct();
            }
        
            private void getFecha()
            {
                DateTime date = new DateTime();
                string fecha;
                date = monthCalendar1.SelectionStart;
                fecha = date.ToLongDateString();
                txtFecha.Text = fecha;
            }

            private void calcularSubTotal()
            {
                foreach (Unitario i in elementos)
                {
                    subTotal = subTotal + i.Precio;
                }

                if (auxAdicionales.Count != 0)
                {
                    foreach (Unitario i in auxAdicionales)
                    {
                        subTotal = subTotal + i.Precio;
                    }
                }
                if (puertas.Count != 0)
                {
                    foreach (Unitario i in puertas)
                    {
                        subTotal = subTotal + i.Precio;
                    }
                }
                txtSubtotal.Text = subTotal.ToString();
            }

            private void actualizarDGV()
            {
                BindingSource source = new BindingSource();
                source.DataSource = vista.AsEnumerable();

                //tabWorkArea.SelectTab(tabUnitarios);
                dataGridUnitarios.DataSource = source;
                dataGridUnitarios.Columns["id"].Visible = false;
                dataGridUnitarios.Columns["poste"].Visible = false;
                dataGridUnitarios.Columns["brazo"].Visible = false;
                dataGridUnitarios.Columns["puas"].Visible = false;
                dataGridUnitarios.Columns["puerta"].Visible = false;
                dataGridUnitarios.Columns["adicional"].Visible = false;
                dataGridUnitarios.Columns["corrugado"].Visible = false;
                dataGridUnitarios.Columns["fijacion"].Visible = false;
                dataGridUnitarios.Columns["tapa"].Visible = false;
                dataGridUnitarios.Columns["perno"].Visible = false;
                dataGridUnitarios.Columns["tuerca"].Visible = false;
                dataGridUnitarios.Columns["jumbo"].Visible = false;
                dataGridUnitarios.Columns["unidad"].Visible = false;
                
               
                // datagrid.textmatrix(rowno,colno)=format(datagrid.textmatrix(rowno,colno),"0.00") 
                dataGridUnitarios.Refresh();
            }

            private void actualizarDGVAdicionales()
            {
                adicionales = db.getAdicionales();
                BindingSource source = new BindingSource();
                source.DataSource = adicionales.AsEnumerable();


                dgAdicionales.DataSource = source;
                dgAdicionales.Columns["id"].Visible = false;
                dgAdicionales.Columns["poste"].Visible = false;
                dgAdicionales.Columns["brazo"].Visible = false;
                dgAdicionales.Columns["puas"].Visible = false;
                dgAdicionales.Columns["puerta"].Visible = false;
                dgAdicionales.Columns["adicional"].Visible = false;
                dgAdicionales.Columns["corrugado"].Visible = false;
                dgAdicionales.Columns["fijacion"].Visible = false;
                dgAdicionales.Columns["tapa"].Visible = false;
                dgAdicionales.Columns["perno"].Visible = false;
                dgAdicionales.Columns["tuerca"].Visible = false;
                dgAdicionales.Columns["jumbo"].Visible = false;
                dgAdicionales.Columns["unidad"].Visible = false;
                dgAdicionales.Refresh();

                BindingSource sourceResumen = new BindingSource();
                sourceResumen.DataSource = auxAdicionales.AsEnumerable();

                dgResumen.DataSource = sourceResumen;
                dgResumen.Columns["id"].Visible = false;
                dgResumen.Columns["poste"].Visible = false;
                dgResumen.Columns["brazo"].Visible = false;
                dgResumen.Columns["puas"].Visible = false;
                dgResumen.Columns["puerta"].Visible = false;
                dgResumen.Columns["adicional"].Visible = false;
                dgResumen.Columns["corrugado"].Visible = false;
                dgResumen.Columns["fijacion"].Visible = false;
                dgResumen.Columns["tapa"].Visible = false;
                dgResumen.Columns["perno"].Visible = false;
                dgResumen.Columns["tuerca"].Visible = false;
                dgResumen.Columns["jumbo"].Visible = false;
                dgResumen.Refresh();
            }

            private void fillPDF()
            {
                int i = 1;

                {
                    string auxName;
                    string pdfTemplate = Application.StartupPath.ToString() + "\\plantilla.pdf";
                    string fecha = DateTime.Now.Date.ToString();
                    string anio = DateTime.Now.Year.ToString();
                    string mes = DateTime.Now.Month.ToString();
                    string dia = DateTime.Now.Day.ToString();
                    string hora = DateTime.Now.Hour.ToString();
                    string minut = DateTime.Now.Minute.ToString();
                    string segund = DateTime.Now.Second.ToString();
                    string newFile = anio + "-" + mes + "-" + dia + "-" + hora + "-" + minut + "-" + segund + ".pdf";
                    newFile = "c:\\Cotizaciones\\PDF\\" + newFile;
                    try
                    {
                        PdfReader pdfReader = new PdfReader(pdfTemplate);
                        PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(newFile, FileMode.Create));
                        AcroFields pdfFormFields = pdfStamper.AcroFields;
                        pdfFormFields.SetField("datos", txtNombre.Text + "\n" + txtDireccion.Text + "\n" + cmbCiudad.Text);
                        pdfFormFields.SetField("sitio", txtDireccionProyecto.Text + " - " + cmbDestino.Text);
                        pdfFormFields.SetField("fecha", txtFecha.Text);
                        pdfFormFields.SetField("telefono", txtTelf.Text);
                        pdfFormFields.SetField("fax", txtFax.Text);
                        pdfFormFields.SetField("celular", txtCel.Text);
                        pdfFormFields.SetField("email", txtEmail.Text);
                        i = 1;
                        foreach(Unitario u in elementos)
                        {
                            auxName = "u" + i.ToString() + "cod";
                            pdfFormFields.SetField(auxName , u.CodigoIdeal);
                            auxName = "u" + i.ToString() + "cant";
                            pdfFormFields.SetField(auxName , u.Cantidad.ToString());
                            auxName = "u" + i.ToString() + "des";
                            pdfFormFields.SetField(auxName, u.Descripcion);
                            /////
                            auxName = "u" + i.ToString() + "prec";
                            List<Unitario> aux = new List<Unitario>();
                            aux = db.getUnitario(u.Id);
                            foreach (Unitario un in aux)
                            {
                                pdfFormFields.SetField(auxName, un.Precio.ToString());
                            }
                            ///
                            auxName = "u" + i.ToString() + "val";
                            pdfFormFields.SetField(auxName, u.Precio.ToString());

                            i++;
                        }
                        if (auxAdicionales.Count != 0)
                        {
                            //i++;
                            auxName = "u" + i.ToString() + "des";
                            pdfFormFields.SetField(auxName, "----------Adicionales----------");
                            
                            i++;
                            
                            foreach (Unitario j in auxAdicionales)
                            {
                                auxName = "u" + i.ToString() + "cod";
                                pdfFormFields.SetField(auxName, j.CodigoIdeal);
                                auxName = "u" + i.ToString() + "cant";
                                pdfFormFields.SetField(auxName, j.Cantidad.ToString());
                                auxName = "u" + i.ToString() + "des";
                                pdfFormFields.SetField(auxName, j.Descripcion);
                                /////
                                auxName = "u" + i.ToString() + "prec";
                                List<Unitario> aux = new List<Unitario>();
                                aux = db.getUnitario(j.Id);
                                foreach (Unitario un in aux)
                                {
                                    pdfFormFields.SetField(auxName, un.Precio.ToString());
                                }
                                ///
                                auxName = "u" + i.ToString() + "val";
                                pdfFormFields.SetField(auxName, j.Precio.ToString());

                                i++;
                            }
                        }
                        if (puertas.Count != 0)
                        {
                            //i++;
                            auxName = "u" + i.ToString() + "des";
                            pdfFormFields.SetField(auxName, "----------Puertas----------");

                            i++;

                            foreach (Unitario j in puertas)
                            {
                                auxName = "u" + i.ToString() + "cod";
                                pdfFormFields.SetField(auxName, j.CodigoIdeal);
                                auxName = "u" + i.ToString() + "cant";
                                pdfFormFields.SetField(auxName, j.Cantidad.ToString());
                                auxName = "u" + i.ToString() + "des";
                                pdfFormFields.SetField(auxName, j.Descripcion);
                                /////
                                auxName = "u" + i.ToString() + "prec";
                                List<Unitario> aux = new List<Unitario>();
                                aux = db.getUnitario(j.Id);
                                foreach (Unitario un in aux)
                                {
                                    pdfFormFields.SetField(auxName, un.Precio.ToString());
                                }
                                ///
                                auxName = "u" + i.ToString() + "val";
                                pdfFormFields.SetField(auxName, j.Precio.ToString());

                                i++;
                            }
                        }
                        
                        pdfFormFields.SetField("subtotal", txtSubtotal.Text);
                        //pdfFormFields.SetField();
                        pdfFormFields.SetField("iva", txtIva.Text);
                        pdfFormFields.SetField("total", txtTotal.Text);

                        pdfStamper.FormFlattening = false;
                        pdfStamper.Close();
                        MessageBox.Show("Archivo generado correctamente", "Cotizador", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception exc) { MessageBox.Show("Error al generar el archivo pdf. " + exc.Message, "Cotizador", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                }
            }

            private void bloqueo(int opc)
            {
                if (opc == 1)
                {
                    gbLongitud.Enabled = false;
                    gbInstalación.Enabled = false;
                    gbAdicionales.Enabled = false;
                    gbBrazo.Enabled = false;
                    btnIngresar.Enabled = false;
                    btnContinuar.Enabled = false;
                    btnImprimir.Enabled = false;
                    btnIngresar.Enabled = false;
                }
                if (opc == 0)
                {
                    gbLongitud.Enabled = true;
                    gbInstalación.Enabled = true;
                    gbAdicionales.Enabled = true;
                    gbBrazo.Enabled = true;
                    btnIngresar.Enabled = true;
                    btnContinuar.Enabled = true;
                    btnImprimir.Enabled = true;
                    btnIngresar.Enabled = true;
                }
            }

            private void addAdicionales()
            {
                vista.Clear();
                foreach (Unitario i in elementos)
                {
                    vista.Add(i);
                }
                if (auxAdicionales.Count != 0)
                {
                    foreach(Unitario i in auxAdicionales)
                    {
                        vista.Add(i);
                    }
                }
                if (puertas.Count != 0)
                {
                    foreach(Unitario i in puertas)
                    {
                        vista.Add(i);
                    }
                }
              
             }

            private void limpiar()
            {
                //datos proyecto
                txtNombre.Text = "";
                txtDireccion.Text = "";
                txtEmail.Text = "";
                txtTelf.Text = "";
                txtCel.Text = "";
                txtFax.Text = "";
                txtDireccionProyecto.Text = "";

                // requerimientos
                txtLong.Text = "";

                //variables
                placa = 0;
                instala = 0;
                gradoEscalonamiento = 0;

                idAltura = 0;
                idColor = 0;
                idPlantilla = 0;

                longuitud = 0;
                subTotal = 0;

                auxIdAdic = 0;

                flagAltura = false;
                flagColor = false;
                flagPlaca = false;
                flagInstalacion = false;
                flagFirst = true;

                //opciones load
                this.cmbColor.SelectedItem = 0;
                this.cmbColor.SelectedIndex = 0;

                this.cmbAltura.SelectedItem = 0;
                this.cmbAltura.SelectedIndex = 0;

                this.cmbPlaca.SelectedItem = 0;
                this.cmbPlaca.SelectedIndex = 0;

                this.cmbGrados.SelectedItem = 0;
                this.cmbGrados.SelectedIndex = 0;

                this.cmbCiudad.SelectedItem = 0;
                this.cmbCiudad.SelectedIndex = 0;

                this.cmbDespacho.SelectedItem = 0;
                this.cmbDespacho.SelectedIndex = 0;

                this.cmbDestino.SelectedItem = 0;
                this.cmbDestino.SelectedIndex = 0;

                this.cmbInstalacion.SelectedItem = 0;
                this.cmbInstalacion.SelectedIndex = 0;

                idColor = Convert.ToInt16(cmbColor.SelectedValue.ToString());
                idAltura = Convert.ToInt16(cmbAltura.SelectedValue.ToString());

                rbBrazoNo.Checked = true;
                rbPuaNo.Checked = true;

                getFecha();

                lblFecha.Text = db.getFecha();

                //bloqueo de inicio
                bloqueo(1);
                gbMain.Enabled = true;

                //variables
                elementos.Clear();
                auxAdicionales.Clear();
                puertas.Clear();
                vista.Clear();
                
                //DGV

                actualizarDGVAdicionales();
                actualizarDGV();
                tabWorkArea.SelectTab(tabDatos);

                //Tab Unitarios
                btnImprimir.Enabled = false;

            }


        # endregion

            private void btnImprimir_Click(object sender, EventArgs e)
            {
                fillPDF();
            }

            private void limpiarToolStripMenuItem_Click(object sender, EventArgs e)
            {
                limpiar();
            }

            private void cargarArchivoToolStripMenuItem_Click(object sender, EventArgs e)
            {
                Funciones funcion = new Funciones();
                string strFileName = String.Empty;
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
                    else
                    {
                        MessageBox.Show("Archivo cargado correctamente, la aplicación se cerrará, por favor vuelvala a iniciar.","Cerca Pronto 3D",MessageBoxButtons.OK,MessageBoxIcon.Information);
                        Application.Exit();
                    }
                }
                if (strFileName == String.Empty) { Application.Exit(); }
                return;
            }

            private void btnIngresar_Click(object sender, EventArgs e)
            {
                double subt, descu, aux;
                DataSet desc = new DataSet();
                DateTime dt = new DateTime(); 
                    try
                    {
                        string descuento = Microsoft.VisualBasic.Interaction.InputBox("Ingrese el código del descuento", "Descuento", "", 200, 200);
                        desc = db.select("select * from descuentos where codigoDescuento =" + "'" + descuento + "'");
                        if (desc.Tables[0].Rows.Count == 0) 
                        {
                            MessageBox.Show("Descuento no encontrado", "Cerca Pronto 3D", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        
                        dt = Convert.ToDateTime(desc.Tables[0].Rows[0][4].ToString());
                        dt = dt.AddMinutes(15);
                        if (dt >= DateTime.Now)
                        {
                            //lblDescuento.Text = "Descuento " + desc.Tables[0].Rows[0][2].ToString() + "%:";
                            subt = Convert.ToDouble(txtSubtotal.Text);
                            descu = Convert.ToDouble(desc.Tables[0].Rows[0][2].ToString());
                            descu = subt * (descu / 100);
                            txtDescuento.Text = descu.ToString();
                            subt = subt - descu;
                            txtSubtotalFinal.Text = subt.ToString();
                            aux = subt * 0.12;
                            txtIva.Text = aux.ToString();
                            aux = aux + subt;
                            txtTotal.Text = aux.ToString();
                            MessageBox.Show("Descuento aplicado", "Cerca Pronto 3D", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else MessageBox.Show("Descuento descartado, el descuento está fuera del límite de tiempo", "Cerca Pronto 3D", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (Exception exc) { }
            
            }

            private void salirToolStripMenuItem_Click(object sender, EventArgs e)
            {
                Application.Exit();
            }


    }
}