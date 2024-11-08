using System;
using System.Collections.Generic;

namespace Iglesia_Bautista_Centro_Familiar_NET.CORE.Models
{
    public partial class Responsable
    {
        public Responsable()
        {
            SolicitudDePrestamos = new HashSet<SolicitudDePrestamo>();
        }

        public int IdResponsable { get; set; }
        public string? Nombre { get; set; }
        public string? Cedula { get; set; }
        public int? IdCargo { get; set; }

        public virtual Cargo? IdCargoNavigation { get; set; }
        public virtual ICollection<SolicitudDePrestamo> SolicitudDePrestamos { get; set; }
    }
}
