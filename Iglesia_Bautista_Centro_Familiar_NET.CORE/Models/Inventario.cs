using System;
using System.Collections.Generic;

namespace Iglesia_Bautista_Centro_Familiar_NET.CORE.Models
{
    public partial class Inventario
    {
        public Inventario()
        {
            SolicitudDePrestamos = new HashSet<SolicitudDePrestamo>();
        }

        public int IdInventario { get; set; }
        public DateTime? FechaInventario { get; set; }
        public int? IdProducto { get; set; }
        public int? IdTipoEntrada { get; set; }
        public int? StockMinimo { get; set; }
        public int? StockMaximo { get; set; }
        public int? Cantidad { get; set; }
        public bool? Statud { get; set; }
        public int? IdAlmacenes { get; set; }

        public virtual Almacene? IdAlmacenesNavigation { get; set; }
        public virtual Producto? IdProductoNavigation { get; set; }
        public virtual TipoEntrada? IdTipoEntradaNavigation { get; set; }
        public virtual ICollection<SolicitudDePrestamo> SolicitudDePrestamos { get; set; }
    }
}
