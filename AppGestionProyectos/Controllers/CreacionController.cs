using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Data;
using AppGestionProyectos.Models;
using System.Diagnostics.Contracts;
using System.Data.SqlTypes;
using System.Threading;
using Microsoft.Ajax.Utilities;
using System.Diagnostics;
using System.Reflection;
using System.Web.Security;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;





namespace AppGestionProyectos.Controllers
{
    public class CreacionController : Controller
    {
        private static string conexion = ConfigurationManager.ConnectionStrings["cadena"].ToString();

        private static List<ListaTarea> tareas = new List<ListaTarea>();

        private static List<SeguimientoTarea> listaSeguimiento = new List<SeguimientoTarea>();


        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Login model)
        {
            if (ModelState.IsValid)
            {
                ViewBag.RolLog = model.rol;
                // Verificar las credenciales y autenticar al usuario
                bool credencialesValidas = validarCredenciales(model.Correo, model.Contra);

                if (credencialesValidas)
                {
                    // Obtener el rol del usuario
                    string rol = obtenerRolUsuario(model.Correo);
                    model.rol= rol;


                    // Autenticar al usuario
                    FormsAuthentication.SetAuthCookie(model.Correo, false);


                    // Redirigir al usuario a la página "Inicio" y pasar el rol como parámetro
                    return RedirectToAction("Inicio", "Creacion", new { rol = model.rol });
                }

            }

            // Credenciales inválidas o ModelState no válido, mostrar mensaje de error o redirigir a la vista "Login" con el modelo
            ModelState.AddModelError("", "Credenciales inválidas");
            return View(model);
        }

        private string obtenerRolUsuario(string correo)
        {
            using (SqlConnection connection = new SqlConnection(conexion))
            {
                string query = "SELECT r.Nombre FROM Usuarios u INNER JOIN Roles r ON u.RolID = r.ID WHERE u.Correo = @Correo";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Correo", correo);
                    connection.Open();
                    string rol = (string)command.ExecuteScalar();
                    return rol;
                }
            }
        }

        public ActionResult CerrarSesion()
        {
            // Cerrar sesión y redirigir al usuario a la página de inicio de sesión
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        private bool validarCredenciales(string correo, string contra)
        {
            using (SqlConnection connection = new SqlConnection(conexion))
            {
                string query = "SELECT COUNT(*) FROM Usuarios WHERE Correo = @Correo AND Contra = @Contra";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Correo", correo);
                    command.Parameters.AddWithValue("@Contra", contra);

                    connection.Open();
                    int count = (int)command.ExecuteScalar();

                    return count > 0;
                }
            }
        }
        // GET: Creacion
        [HttpGet]
        public ActionResult Inicio(string rol)
        {
            ViewBag.Title = "Inicio";
            return View();
        }

        [HttpGet]
        public ActionResult CrearTarea()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CrearTarea(CreacionTarea creacionTarea)
        {
            if (ModelState.IsValid)
            {
                // Realizar la inserción en la base de datos
                InsertarTareaEnBaseDeDatos(creacionTarea);

                // Al principal si metió bien la info
                TempData["MensajeExitoCreacion"] = "La tarea se ha creado exitosamente.";
                return RedirectToAction("Inicio" , "Creacion");
            }


            // Si el modelo no es válido, regresar a la página de creación con los mensajes de error
            TempData["ErrorMessageCreacion"] = "Debe ingresar todos los datos correspondientes.";
            return View("CrearTarea", creacionTarea);
        }

        private void InsertarTareaEnBaseDeDatos(CreacionTarea creacionTarea)
        {
            if (string.IsNullOrEmpty(creacionTarea.NombreTarea) ||
                string.IsNullOrEmpty(creacionTarea.Descripcion) ||
                creacionTarea.FechaInicio == default(DateTime) ||
                creacionTarea.FechaVencimiento == default(DateTime) ||
                creacionTarea.Creador == 0 ||
                string.IsNullOrEmpty(creacionTarea.Comentario))
            {
                throw new ArgumentException("Debe ingresar todos los datos correspondientes.");
            }

            // Validar que la fecha de vencimiento no sea mayor que la fecha de inicio
            if (creacionTarea.FechaVencimiento < creacionTarea.FechaInicio)
            {
                // Aquí puedes manejar el error, lanzar una excepción o mostrar un mensaje de error, según tus necesidades
                throw new ArgumentException("La fecha de vencimiento no puede ser anterior a la fecha de inicio.");
            }
            // Conexión a la base de datos
            using (SqlConnection connection = new SqlConnection(conexion))
            {


                // Crear el comando
                using (SqlCommand command = new SqlCommand("CrearTarea", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    // Asignar los parámetros
                    command.Parameters.AddWithValue("@NombreTarea", creacionTarea.NombreTarea);
                    command.Parameters.AddWithValue("@Descripcion", creacionTarea.Descripcion);
                    command.Parameters.AddWithValue("@FechaInicio", creacionTarea.FechaInicio);
                    command.Parameters.AddWithValue("@FechaVencimiento", creacionTarea.FechaVencimiento);
                    command.Parameters.AddWithValue("@Creador", creacionTarea.Creador);
                    command.Parameters.AddWithValue("@Comentario", creacionTarea.Comentario);

                    // Abrir la conexión y ejecutar el comando

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        [HttpGet]
        public ActionResult ListaTareas()
        {
            tareas = new List<ListaTarea>();

            using (SqlConnection connection = new SqlConnection(conexion))
            {
                string query = "SELECT T.ID AS TareaID, T.NombreTarea, T.Descripcion, T.FechaInicio, T.FechaVencimiento, T.Comentario, T.Asignado, U.Nombre AS NombreCreador " +
                                "FROM Tareas T " +
                                "INNER JOIN Usuarios U ON T.Creador = U.ID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        ListaTarea tarea = new ListaTarea();
                        tarea.ID = Convert.ToInt32(reader["TareaID"]);
                        tarea.NombreTarea = reader.GetString(reader.GetOrdinal("NombreTarea"));
                        tarea.Descripcion = reader.GetString(reader.GetOrdinal("Descripcion"));
                        tarea.FechaInicio = reader.GetDateTime(reader.GetOrdinal("FechaInicio"));
                        tarea.FechaVencimiento = reader.GetDateTime(reader.GetOrdinal("FechaVencimiento"));
                        tarea.NombreCreador = reader.GetString(reader.GetOrdinal("NombreCreador"));
                        tarea.Comentarios = reader.GetString(reader.GetOrdinal("Comentario"));
                        tarea.Asignado = reader.IsDBNull(reader.GetOrdinal("Asignado")) ? null : reader.GetString(reader.GetOrdinal("Asignado"));

                        tareas.Add(tarea);
                    }

                }
            }


            return View(tareas);

        }

        [HttpGet]
        public ActionResult AsignarTarea()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AsignarTarea(AsignarTarea model)
        {

            int IDTarea = model.IDTarea;
            int IDUsuario = model.IDUsuario;
            ViewBag.IDTareaAsignada = IDTarea;


            using (SqlConnection connection = new SqlConnection(conexion))
            {
                connection.Open();

                string obtenerNombreTareaQuery = "SELECT NombreTarea FROM Tareas WHERE ID = @IDTarea";

                // Obtener el nombre de la tarea asignada
                using (SqlCommand obtenerNombreTareaCommand = new SqlCommand(obtenerNombreTareaQuery, connection))
                {
                    obtenerNombreTareaCommand.Parameters.AddWithValue("@IDTarea", IDTarea);

                    string nombreTarea = obtenerNombreTareaCommand.ExecuteScalar()?.ToString();

                    // Guardar el nombre de la tarea asignada en TempData
                    TempData["NombreTareaAsignada"] = nombreTarea;
                }

                string validationQuery = "SELECT COUNT(*) FROM Usuarios WHERE ID = @IDUsuario AND EXISTS(SELECT 1 FROM Tareas WHERE ID = @IDTarea)";

                using (SqlCommand validationCommand = new SqlCommand(validationQuery, connection))
                {
                    validationCommand.Parameters.AddWithValue("@IDUsuario", IDUsuario);
                    validationCommand.Parameters.AddWithValue("@IDTarea", IDTarea);

                    int count = (int)validationCommand.ExecuteScalar();

                    if (count == 0)
                    {
                        TempData["Mensaje"] = "El usuario o la tarea no existen.";

                        // Redirigir a la página de lista de tareas u a otra acción de error
                        return RedirectToAction("AsignarTarea", "Creacion");
                    }
                }

                string sqlQuery = "INSERT INTO Asignaciones (IDUsuario, IDTarea)" +
                                  "VALUES (@IDUsuario, @IDTarea)";

                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@IDUsuario", IDUsuario);
                    command.Parameters.AddWithValue("@IDTarea", IDTarea);

                    command.ExecuteNonQuery();
                }

                string updateAsignadoQuery = "UPDATE Tareas SET Asignado = (SELECT Nombre FROM Usuarios WHERE ID = @IDUsuario) WHERE ID = @IDTarea";

                using (SqlCommand updateAsignadoCommand = new SqlCommand(updateAsignadoQuery, connection))
                {
                    updateAsignadoCommand.Parameters.AddWithValue("@IDUsuario", IDUsuario);
                    updateAsignadoCommand.Parameters.AddWithValue("@IDTarea", IDTarea);

                    updateAsignadoCommand.ExecuteNonQuery();
                }

                string obtenerNombreUsuarioQuery = "SELECT Nombre FROM Usuarios WHERE ID = @IDUsuario";

                // Obtener el nombre del usuario asignado a la tarea
                using (SqlCommand obtenerNombreUsuarioCommand = new SqlCommand(obtenerNombreUsuarioQuery, connection))
                {
                    obtenerNombreUsuarioCommand.Parameters.AddWithValue("@IDUsuario", IDUsuario);

                    string nombreUsuario = obtenerNombreUsuarioCommand.ExecuteScalar()?.ToString();

                    // Guardar el nombre del usuario asignado en TempData
                    TempData["NombreUsuarioAsignado"] = nombreUsuario;
                }
            }


            TempData["Mensaje"] = "Usuario asignado exitosamente";

            // Redirigir a la página de lista de tareas u otra acción
            return RedirectToAction("ListaTareas" , "Creacion");
        }


        [HttpGet]
        public ActionResult EditarTarea()
        {
            return View();
   
        }


        [HttpPost]
        public ActionResult EditarTarea(int? id, string nuevaDescripcion, DateTime? nuevaFechaInicio, DateTime? nuevaFechaVencimiento, int? nuevoCreador, string nuevoComentario)
        {
            if (!id.HasValue || id <= 0 || string.IsNullOrEmpty(nuevaDescripcion) || !nuevaFechaInicio.HasValue || !nuevaFechaVencimiento.HasValue || !nuevoCreador.HasValue || nuevoCreador == 0 || nuevoCreador > 4 || string.IsNullOrEmpty(nuevoComentario))
            {
                TempData["MensajeEdicion"] = "Por favor, complete todos los campos.";
                return RedirectToAction("EditarTarea", "Creacion", new { id = id });
            }
            else
            {
                // Realizar la conexión a la base de datos y crear el comando SQL

                using (SqlConnection connection = new SqlConnection(conexion))

                {

                    connection.Open();


                    string modificarTareaQuery = "UPDATE Tareas SET Descripcion = @NuevaDescripcion, FechaInicio = @NuevaFechaInicio, FechaVencimiento = @NuevaFechaVencimiento, Creador = @NuevoCreador, Comentario = @NuevoComentario WHERE ID = @IDTarea";


                    using (SqlCommand modificarTareaCommand = new SqlCommand(modificarTareaQuery, connection))

                    {
                        modificarTareaCommand.Parameters.AddWithValue("@IDTarea", id);
                        modificarTareaCommand.Parameters.AddWithValue("@NuevaDescripcion", nuevaDescripcion);
                        modificarTareaCommand.Parameters.AddWithValue("@NuevaFechaInicio", nuevaFechaInicio);
                        modificarTareaCommand.Parameters.AddWithValue("@NuevaFechaVencimiento", nuevaFechaVencimiento);
                        modificarTareaCommand.Parameters.AddWithValue("@NuevoCreador", nuevoCreador);
                        modificarTareaCommand.Parameters.AddWithValue("@NuevoComentario", nuevoComentario);

                        modificarTareaCommand.ExecuteNonQuery();
                    }
                }
                // Redirigir a la página de visualización de la tarea modificada
                TempData["MensajeEdicionExitosa"] = "La tarea se ha editado exitosamente.";

                return RedirectToAction("ListaTareas", "Creacion");
            }
                
        }





        [HttpGet]
        public ActionResult EliminarTarea()
        {
            return View();
        }


        [HttpPost]
        public ActionResult EliminarTarea(EliminarTarea model)
        {
            int id = model.ID;

            // Realizar la conexión a la base de datos y crear el comando SQL
            using (SqlConnection connection = new SqlConnection(conexion))
            {
                connection.Open();

                // Crear el comando para ejecutar el procedimiento almacenado
                using (SqlCommand eliminarTareaCommand = new SqlCommand("EliminarTarea", connection))
                {
                    eliminarTareaCommand.CommandType = CommandType.StoredProcedure;
                    eliminarTareaCommand.Parameters.AddWithValue("@IDTarea", id);

                    eliminarTareaCommand.ExecuteNonQuery();
                }
            }

            TempData["MensajeEliminacion"] = "Tarea eliminada exitosamente";
            // Redirigir a la página de lista de tareas u otra página apropiada
            return RedirectToAction("ListaTareas", "Creacion");


        }

        [HttpGet]
        public ActionResult DetalleTarea()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DetalleTarea(DetalleTarea model)
        {
            using (SqlConnection connection = new SqlConnection(conexion))
            {
                connection.Open();
                bool realizada = model.Realizada;
                // Actualizar el campo "Realizada" del detalle de la tarea
                string insertarDetalleQuery = "INSERT INTO DetalleTareas (TareaID, Realizada) VALUES (@TareaID, @Realizada)";

                using (SqlCommand insertarDetalleCommand = new SqlCommand(insertarDetalleQuery, connection))
                {
                    insertarDetalleCommand.Parameters.AddWithValue("@TareaID", model.TareaID);
                    insertarDetalleCommand.Parameters.AddWithValue("@Realizada", realizada);
                    insertarDetalleCommand.ExecuteNonQuery();
                }


                //Para la fecha 
                DateTime fechaInicio = DateTime.MinValue;
                string obtenerFechaInicioQuery = "SELECT FechaInicio FROM Tareas WHERE ID = @TareaID";

                using (SqlCommand obtenerFechaInicioCommand = new SqlCommand(obtenerFechaInicioQuery, connection))
                {
                    obtenerFechaInicioCommand.Parameters.AddWithValue("@TareaID", model.TareaID);
                    var result = obtenerFechaInicioCommand.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        fechaInicio = Convert.ToDateTime(result);
                    }
                }

                // Calcular el valor de TiempoInvertido
                DateTime fechaActual = DateTime.Now.Date;
                int tiempoInvertido = (fechaActual - fechaInicio).Days;


                // Insertar la fecha de registro en la tabla "SeguimientoTareas"
                string insertarFechaRegistroQuery = "INSERT INTO SeguimientoTareas (TareaID, FechaRegistro) VALUES (@TareaID, @FechaRegistro)";

                using (SqlCommand insertarFechaRegistroCommand = new SqlCommand(insertarFechaRegistroQuery, connection))
                {
                    insertarFechaRegistroCommand.Parameters.AddWithValue("@TareaID", model.TareaID);
                    insertarFechaRegistroCommand.Parameters.AddWithValue("@FechaRegistro", DateTime.Now);
                    insertarFechaRegistroCommand.ExecuteNonQuery();
                }

                // Actualizar el campo TiempoInvertido en la tabla SeguimientoTareas
                string actualizarTiempoInvertidoQuery = "UPDATE SeguimientoTareas SET TiempoInvertido = @TiempoInvertido WHERE TareaID = @TareaID";

                using (SqlCommand actualizarTiempoInvertidoCommand = new SqlCommand(actualizarTiempoInvertidoQuery, connection))
                {
                    actualizarTiempoInvertidoCommand.Parameters.AddWithValue("@TiempoInvertido", tiempoInvertido);
                    actualizarTiempoInvertidoCommand.Parameters.AddWithValue("@TareaID", model.TareaID);
                    actualizarTiempoInvertidoCommand.ExecuteNonQuery();
                }


                // Mostrar mensaje de éxito
                TempData["MensajeExitoDetalle"] = "Detalle de tarea actualizado correctamente.";
            }

            // Redirigir a la página de detalles de la tarea
            return RedirectToAction("SeguimientoTareas", "Creacion", new { id = model.TareaID });

        }


        [HttpGet]
        public ActionResult SeguimientoTareas()
        {
            Dictionary<int, bool> tareasAgregadas = new Dictionary<int, bool>();
            List<SeguimientoTarea> listaSeguimiento = new List<SeguimientoTarea>();

            using (SqlConnection connection = new SqlConnection(conexion))
            {
                string query = "SELECT T.ID AS TareaID, T.Descripcion, T.FechaInicio, " +
                                "(SELECT TOP 1 Realizada FROM DetalleTareas WHERE TareaID = T.ID ORDER BY ID DESC) AS Realizada, " +
                                "S.TiempoInvertido " +
                                "FROM Tareas T " +
                                "LEFT JOIN SeguimientoTareas S ON T.ID = S.TareaID " +
                                "ORDER BY T.ID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        int tareagID = Convert.ToInt32(reader["TareaID"]);

                        if (!tareasAgregadas.ContainsKey(tareagID))
                        {
                            SeguimientoTarea seguimiento = new SeguimientoTarea();
                            seguimiento.TareaID = Convert.ToInt32(reader["TareaID"]);
                            seguimiento.Descripcion = reader.GetString(reader.GetOrdinal("Descripcion"));
                            seguimiento.FechaInicio = reader.GetDateTime(reader.GetOrdinal("FechaInicio"));
                            seguimiento.Realizada = reader.IsDBNull(reader.GetOrdinal("Realizada")) ? false : reader.GetBoolean(reader.GetOrdinal("Realizada"));

                            DateTime fechaInicio = reader.GetDateTime(reader.GetOrdinal("FechaInicio"));
                            DateTime fechaActual = DateTime.Now.Date;
                            int tiempoInvertido = (fechaActual - fechaInicio).Days;
                            seguimiento.TiempoInvertido = tiempoInvertido;


                            listaSeguimiento.Add(seguimiento);

                            //Tarea agregada
                            tareasAgregadas[tareagID] = true;


                        }

                    }
                }
            }

            return View(listaSeguimiento);
        }



    }

}

