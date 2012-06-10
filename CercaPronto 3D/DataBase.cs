using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows.Forms;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace CercaPronto_3D
{
    class DataBase
    {
        private DataSet datos;

       # region Con Conexión
                private string server;
                private string user;
                private string pwd;
                private string bd;

                private string conextion;

                private MySqlConnection conn;
                private MySqlCommand command;
                private MySqlDataReader reader;
                private MySqlDataAdapter adapter;

                   
        # endregion

        public DataBase() 
        {
            datos = new DataSet();
            datos.ReadXml(Application.StartupPath + "\\cotizadorxml.xml");

            //cargar las configuraciones de la base de datos
                DataSet conf = new DataSet();
                conf.ReadXml(Application.StartupPath + "\\cot_externo.xml");
                this.server = conf.Tables[0].Rows[0][0].ToString();
                conf.Reset();

                conf.ReadXml(Application.StartupPath + "\\cot_conn.xml");
                this.user = conf.Tables[0].Rows[0][0].ToString();
                this.pwd = conf.Tables[0].Rows[0][1].ToString();
                this.bd = conf.Tables[0].Rows[0][2].ToString();
                this.conextion = "Server=" + this.server + ";Database=" + this.bd + ";Uid=" + this.user + ";Pwd=" + this.pwd;
                this.conn = new MySqlConnection(this.conextion);
        }

        public DataSet select(string comando)
        {
            DataSet resultado = new DataSet(); 
            try
            {
                this.conn.Open();
                this.adapter = new MySqlDataAdapter(comando, this.conn);
                adapter.Fill(resultado);
                this.conn.Close();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            return resultado;
        }

        public DataSet getDatos() { return datos; }

        public DataTable selectUsuario(string mail)
        {
            DataTable usuarios = datos.Tables["usuario"];
            EnumerableRowCollection<DataRow> query =
            from usuario in usuarios.AsEnumerable()
            where usuario.Field<string>("email") == mail
            select usuario;
            return query.AsDataView().ToTable();
        }

        public DataTable getAlturas()
        {
            DataTable alturas = datos.Tables["altura"];
            EnumerableRowCollection<DataRow> query =
            from altura in alturas.AsEnumerable()
            select altura;
            return query.AsDataView().ToTable();
        }
        
        public DataTable getColores()
        {
            DataTable colores = datos.Tables["color"];
            EnumerableRowCollection<DataRow> query =
            from color in colores.AsEnumerable()
            select color;
            return query.AsDataView().ToTable();
        }
        
        public DataTable getIdPlantilla(int idAltura, int idColor)
        {
            DataTable plantillas = datos.Tables["plantilla"];
            EnumerableRowCollection<DataRow> query =
            from plantilla in plantillas.AsEnumerable()
            where plantilla.Field<string>("idAltura") == idAltura.ToString() && 
            plantilla.Field<string>("idColor")== idColor.ToString()
            select plantilla;
            return query.AsDataView().ToTable();
        }
        
        public List<PlantillaPlaca> getPlantillaPlaca(int idPlantilla) 
        {
            DataTable aux;
            List<PlantillaPlaca> lista = new List<PlantillaPlaca>();
           
            DataTable plantillas = datos.Tables["plantilla_placa"];
            EnumerableRowCollection<DataRow> query =
            from plantilla in plantillas.AsEnumerable()
            where plantilla.Field<string>("idPlantilla") == idPlantilla.ToString()
            select plantilla;
            aux = query.AsDataView().ToTable();

            foreach (DataRow uni in aux.Rows)
            {
                PlantillaPlaca elementos = new PlantillaPlaca();
                elementos.IdPlantillaPlaca = int.Parse(uni.Field<string>("idPlantillaPlaca"));
                elementos.IdPlantilla = int.Parse(uni.Field<string>("idPlantilla"));
                elementos.IdUnitario = int.Parse(uni.Field<string>("idUnitario"));
                elementos.Cantidad = float.Parse(uni.Field<string>("cantidad"));
                lista.Add(elementos);
            }
            return lista;
        }
        
        public List<PlantillaEmpotrado> getPlantillaEmpotrado(int idPlantilla)
        {
            DataTable aux;
            List<PlantillaEmpotrado> lista = new List<PlantillaEmpotrado>();

            DataTable plantillas = datos.Tables["plantilla_empotrado"];
            EnumerableRowCollection<DataRow> query =
            from plantilla in plantillas.AsEnumerable()
            where plantilla.Field<string>("idPlantilla") == idPlantilla.ToString()
            select plantilla;
            aux = query.AsDataView().ToTable();

            foreach (DataRow uni in aux.Rows)
            {
                PlantillaEmpotrado elementos = new PlantillaEmpotrado();
                elementos.IdPlantillaEmpotrado = int.Parse(uni.Field<string>("idPlantillaEmpotrado"));
                elementos.IdPlantilla = int.Parse(uni.Field<string>("idPlantilla"));
                elementos.IdUnitario = int.Parse(uni.Field<string>("idUnitario"));
                elementos.Cantidad = float.Parse(uni.Field<string>("cantidad"));
                lista.Add(elementos);
            }
            return lista;
        }
        
        public List <Unitario> getUnitario(int idUnitario) 
        {
            DataTable aux;
            List<Unitario> lista = new List<Unitario>();
            DataTable unitarios = datos.Tables["unitarios"];
            EnumerableRowCollection<DataRow> query =
            from unitario in unitarios.AsEnumerable()
            where unitario.Field<string>("idUnitario") == idUnitario.ToString()
            select unitario;
            aux = query.AsDataView().ToTable();

            foreach(DataRow uni in aux.Rows)
            {
                Unitario u = new Unitario();
                u.Id = int.Parse(uni.Field<string>("idUnitario"));
                u.CodigoIdeal = uni.Field<string>("codigoIdeal");
                u.Descripcion = uni.Field<string>("descripcion");
                u.Precio = float.Parse(uni.Field<string>("pvpTeorico"));
                if (uni.Field<string>("adicional") == "1") u.Adicional = true;
                if (uni.Field<string>("brazo") == "1") u.Brazo = true;
                if (uni.Field<string>("poste") == "1") u.Poste = true;
                if (uni.Field<string>("puas") == "1") u.Puas = true;
                if (uni.Field<string>("puerta") == "1") u.Puerta = true;
                if (uni.Field<string>("tapa") == "1") u.Tapa = true;
                if (uni.Field<string>("jumbo") == "1") u.Jumbo = true;
                if (uni.Field<string>("perno") == "1") u.Perno = true;
                if (uni.Field<string>("tuerca") == "1") u.Tuerca = true;
                if (uni.Field<string>("fijaciones") == "1") u.Fijacion = true;
                if (uni.Field<string>("corrugado") == "1") u.Corrugado = true;
                u.Unidad = uni.Field<string>("unidad");

                lista.Add(u);
            }
            return lista;
        }

        public List<Unitario> getAdicionales()
        {
            DataTable aux;
            List<Unitario> lista = new List<Unitario>();
            DataTable unitarios = datos.Tables["unitarios"];
            EnumerableRowCollection<DataRow> query =
            from unitario in unitarios.AsEnumerable()
            where unitario.Field<string>("adicional") == "1"
            select unitario;
            aux = query.AsDataView().ToTable();

            foreach (DataRow uni in aux.Rows)
            {
                Unitario u = new Unitario();
                u.Id = int.Parse(uni.Field<string>("idUnitario"));
                u.CodigoIdeal = uni.Field<string>("codigoIdeal");
                u.Descripcion = uni.Field<string>("descripcion");
                u.Precio = float.Parse(uni.Field<string>("pvpTeorico"));
                u.Cantidad = 1;
                lista.Add(u);
            }
            return lista;
        }

        public List<PlantillaMalla> getMalla(int idPlantilla)
        {
            DataTable aux;
            List<PlantillaMalla> pMalla = new List<PlantillaMalla>();

            DataTable mallas = datos.Tables["plantilla_mallas"];
            EnumerableRowCollection<DataRow> query =
            from malla in mallas.AsEnumerable()
            where malla.Field<string>("idPlantilla") == idPlantilla.ToString()
            select malla;
            aux = query.AsDataView().ToTable();

            foreach (DataRow uni in aux.Rows)
            {
                PlantillaMalla u = new PlantillaMalla();
                u.Cantidad = (int)float.Parse(uni.Field<string>("cantidad"));
                u.IdPlantilla = int.Parse(uni.Field<string>("idPlantilla"));
                u.IdPlantillaMalla = int.Parse(uni.Field<string>("idPlantillaMalla"));
                u.IdUnitario = int.Parse(uni.Field<string>("idUnitario"));
                pMalla.Add(u);
            }

            return pMalla;
        }

        public List<Ciudad> getCiudades()
        {
            DataTable aux;
            List<Ciudad> lista = new List<Ciudad>();
            DataTable ciudades = datos.Tables["ciudades"];
            EnumerableRowCollection<DataRow> query =
            from ciudad in ciudades.AsEnumerable()
            select ciudad;
            aux = query.AsDataView().ToTable();

            foreach (DataRow ciu in aux.Rows)
            {
                Ciudad c = new Ciudad();
                c.Id = int.Parse(ciu.Field<string>("id"));
                c.Origen = ciu.Field<string>("origen");
                c.Destino = ciu.Field<string>("destino");
                c.Distancia = float.Parse(ciu.Field<string>("distancia"));
                lista.Add(c);
            }
            return lista;
        }

        public List<Uni> getUnitarios()
        {
            DataTable aux;
            List<Uni> lista = new List<Uni>();
            DataTable unitarios = datos.Tables["unitarios"];
            EnumerableRowCollection<DataRow> query =
            from unitario in unitarios.AsEnumerable()
            select unitario;
            aux = query.AsDataView().ToTable();

            foreach(DataRow uni in aux.Rows)
            {
                Uni u = new Uni();
                u.Id = int.Parse(uni.Field<string>("idUnitario"));
                u.CodigoIdeal = uni.Field<string>("codigoIdeal");
                u.Descripcion = uni.Field<string>("descripcion");
                u.PvpTeorico = float.Parse(uni.Field<string>("pvpTeorico"));    
                lista.Add(u);
            }
            return lista;
        }

        public List<Pua> getPuas()
        {
            DataTable aux;
            List<Pua> lista = new List<Pua>();
            DataTable unitarios = datos.Tables["unitarios"];
            EnumerableRowCollection<DataRow> query =
            from unitario in unitarios.AsEnumerable()
            where unitario.Field<string>("puas")=="1"
            select unitario;
            aux = query.AsDataView().ToTable();

            foreach (DataRow uni in aux.Rows)
            {
                Pua u = new Pua();
                u.Id = int.Parse(uni.Field<string>("idUnitario"));
                u.CodigoIdeal = uni.Field<string>("codigoIdeal");
                u.Descripcion = uni.Field<string>("descripcion");
                u.Precio = float.Parse(uni.Field<string>("pvpTeorico"));
                u.Medida = float.Parse(uni.Field<string>("medida"));
                lista.Add(u);
            }
            return lista;
        }

        public string getFecha()
        {
            string fecha = "";
            DataTable aux;
            List<Uni> lista = new List<Uni>();
            DataTable unitarios = datos.Tables["fechaxml"];
            EnumerableRowCollection<DataRow> query =
            from unitario in unitarios.AsEnumerable()
            select unitario;
            aux = query.AsDataView().ToTable();

            foreach (DataRow uni in aux.Rows)
            {
                fecha = uni.Field<string>("last");
            }
            return fecha;
        }

        }
    }
    

