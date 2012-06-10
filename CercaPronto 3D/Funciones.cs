using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using System.Security.Cryptography;
using System.IO;

namespace CercaPronto_3D
{
    class Funciones
    {
        public Funciones() { }

        public bool inicializar()
        {
            //verficar q exista en archivo xml exista
            if (!System.IO.File.Exists(Application.StartupPath + "\\cotizadorxml.xml"))
                return false;
            else
                return true;
        }

        public bool primeraVez()
        {
            if (System.IO.File.Exists(Application.StartupPath + "\\user.xml"))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public string guardarUsuario(string mail, string pwd)
        {
            string mensaje = "Error al guardar usuario";
            DataBase data = new DataBase();
            DataTable usuario = new DataTable();
            if (primeraVez())
            {
                usuario = data.selectUsuario(mail);
                if (usuario.Rows.Count > 0)
                {
                    if (usuario.Rows[0][3].ToString().ToUpper() == getSHA1(pwd))
                    {
                        usuario.WriteXml(Application.StartupPath + "\\user.xml");
                        mensaje = "Credenciales de usuario guardadas en el terminal, por favor vuelva a ingresar.";
                    }
                    else { mensaje = "Credenciales Incorrectas."; }
                }
                else { mensaje = "Credenciales Incorrectas."; }
            }
            return mensaje;
        }

        public bool login(string mail, string pwd)
        {
            DataSet credenciales = new DataSet();
            credenciales.ReadXml(Application.StartupPath + "\\user.xml");

            if (credenciales.Tables.Count > 0)
            {
                if ((mail == credenciales.Tables[0].Rows[0][2].ToString()) && (this.getSHA1(pwd) == credenciales.Tables[0].Rows[0][3].ToString().ToUpper()))
                {
                    return true;
                }
                else { return false; }
            }
            else { return false; }
        }

        public string getMD5(string str)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.
            MD5 md5Hasher = MD5.Create();
            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(str));
            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();
            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        public string getSHA1(string pwd)
        {
            string password;
            HashAlgorithm sha1 = HashAlgorithm.Create("SHA1");  
            Byte[] sha1Data = sha1.ComputeHash(UTF8Encoding.UTF8.GetBytes(pwd));  
            password=BitConverter.ToString(sha1Data).Replace("-", "");  
            return password;
        }

        public bool copiarXml(string path)
        {
            string origen = @path;
            string destino = Application.StartupPath + "\\cotizadorxml.xml";
            File.Copy(origen, destino,true);
            return inicializar();
        }
    }
}
