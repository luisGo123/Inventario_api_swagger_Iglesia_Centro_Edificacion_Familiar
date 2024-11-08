using System;
using System.Collections.Generic;

namespace Iglesia_Bautista_Centro_Familiar_NET.CORE.Models
{
    public partial class SolicitudDePrestamo
    {
        public SolicitudDePrestamo()
        {
            AprovacionDeSolicitudPrestamos = new HashSet<AprovacionDeSolicitudPrestamo>();
        }

        public int IdSolicitudDePrestamo { get; set; }
        public DateTime? FechaOperaciones { get; set; }
        public int? IdInventario { get; set; }
        public int? Cantidad { get; set; }
        public string? Lugar { get; set; }
        public string? FechaEntrega { get; set; }
        public int? IdResponsable { get; set; }
        public int? IdSolicitante { get; set; }

        public virtual Inventario? IdInventarioNavigation { get; set; }
        public virtual Responsable? IdResponsableNavigation { get; set; }
        public virtual Solicitante? IdSolicitanteNavigation { get; set; }
        public virtual ICollection<AprovacionDeSolicitudPrestamo> AprovacionDeSolicitudPrestamos { get; set; }
    }
}
