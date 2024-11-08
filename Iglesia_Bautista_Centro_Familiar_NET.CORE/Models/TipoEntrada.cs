using System;
using System.Collections.Generic;

namespace Iglesia_Bautista_Centro_Familiar_NET.CORE.Models
{
    public partial class TipoEntrada
    {
        public TipoEntrada()
        {
            Inventarios = new HashSet<Inventario>();
        }

        public int IdTipoEntrada { get; set; }
        public string? Nombre { get; set; }

        public virtual ICollection<Inventario> Inventarios { get; set; }
    }
}
