using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppGestionProyectos.Models
{
    public class AsignarTarea
    {
        public int ID { get; set; } 
        public int IDTarea { get; set; }
        public int IDUsuario { get; set; }
    }
}