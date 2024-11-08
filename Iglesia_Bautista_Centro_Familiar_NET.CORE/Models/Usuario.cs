using System;
using System.Collections.Generic;

namespace Iglesia_Bautista_Centro_Familiar_NET.CORE.Models
{
    public partial class Usuario
    {
        public Usuario()
        {
            Almacenes = new HashSet<Almacene>();
        }

        public int IdUsuario { get; set; }
        public string? NombreCompleto { get; set; }
        public string? ApellidoCompleto { get; set; }
        public string? Correo { get; set; }
        public string? Contrasena { get; set; }
        public string? Telefono { get; set; }
        public string? Cedula { get; set; }
        public int? IdPerfil { get; set; }

        public virtual Perfil? IdPerfilNavigation { get; set; }
        public virtual ICollection<Almacene> Almacenes { get; set; }
    }
}
