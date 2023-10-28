using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AppGestionProyectos.Models
{
    public class Login
    {
        [Required(ErrorMessage = "El campo Correo es requerido.")]
        [EmailAddress(ErrorMessage = "Por favor ingrese una dirección de correo electrónico válida.")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "El campo Contraseña es requerido.")]
        public string Contra { get; set; }

        public string rol { get; set; }
    }
}