using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppGestionProyectos.Models
{
    public class Usuarios
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Contra { get; set; }
        public string RolID { get; set; }

    }

}