using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace AppGestionProyectos.Models
{
    public class ListaTarea
    {
        public int ID { get; set; }
        public string NombreTarea { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public string NombreCreador { get; set; }
        public string Comentarios { get; set; }
        public string Asignado { get; set; }
        public string nuevaDescripcion { get; set; }
        public DateTime nuevaFechaInicio { get; set; }
        public DateTime nuevaFechaVencimiento { get; set; }
        public int nuevoCreador { get; set; }
        public string nuevoComentario { get; set; }

    }
}