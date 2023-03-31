using Microsoft.AspNetCore.Mvc;
using LoggingPractica.Models;
using System.Text;
using System.Security.Cryptography;
using System.Data.SqlClient;
using System.Data;


namespace LoggingPractica.Controllers
{
    public class accesoController : Controller
    {

        static string cadena = "Data Source=(local);Initial Catalog=DB_ACCESO;Integrated Security=true";
        
        
        //GET Acceso
        public IActionResult login()
        {
            return View();
        }

        public IActionResult registrarse()
        {
            return View();
        }

        [HttpPost]
        public IActionResult registrarse(Usuario oUsuario)
        {
            bool registrado;
            string mensaje;

            if (oUsuario.claveUsuario == oUsuario.confirmarClave)
            {
                oUsuario.claveUsuario = convertirSha256(oUsuario.claveUsuario);
            }
            else
            {
                ViewData["Mensaje"] = "Las contraseñas no coinciden";
                return View();
            }

            using (SqlConnection cn = new SqlConnection(cadena))
            {
                SqlCommand cmd = new SqlCommand("sp_RegistrarUsuario", cn);
                cmd.Parameters.AddWithValue("correoUsuarioProcedure", oUsuario.correoUsuario);
                cmd.Parameters.AddWithValue("claveUsuarioProcedure", oUsuario.claveUsuario);
                cmd.Parameters.Add("registrado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("mensaje", SqlDbType.VarChar, 50).Direction = ParameterDirection.Output;
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();

                cmd.ExecuteNonQuery();

                registrado = Convert.ToBoolean(cmd.Parameters["registrado"].Value);
                mensaje = cmd.Parameters["mensaje"].Value.ToString();
            }

            ViewData["mensaje"] = mensaje;

            if (registrado)
            {
                return RedirectToAction("login", "acceso");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public IActionResult login(Usuario oUsuario)
        {
            oUsuario.claveUsuario = convertirSha256(oUsuario.claveUsuario);

            using (SqlConnection cn = new SqlConnection(cadena))
            {
                SqlCommand cmd = new SqlCommand("sp_validarUsuario", cn);
                cmd.Parameters.AddWithValue("correo", oUsuario.correoUsuario);
                cmd.Parameters.AddWithValue("clave", oUsuario.claveUsuario);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();
                oUsuario.idUsuario = Convert.ToInt32(cmd.ExecuteScalar().ToString()) ; //to string por si acaso
            }

            if(oUsuario.idUsuario != 0)
            {
                
                HttpContext.Session.SetString("usuario", oUsuario.correoUsuario);
                return RedirectToAction("Index", "Home");

            }
            else
            {
                ViewData["mensaje"] = "usuario no encontrado";
                return View();
            }

            
        }




        public static string convertirSha256(string texto)
        {
            StringBuilder sb = new StringBuilder();
            using (SHA256 hash = SHA256.Create())
            {
                Encoding enc = Encoding.UTF8;   
                byte[] result = hash.ComputeHash(enc.GetBytes(texto));

                foreach (byte b in result)
                    sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }
    }
}
