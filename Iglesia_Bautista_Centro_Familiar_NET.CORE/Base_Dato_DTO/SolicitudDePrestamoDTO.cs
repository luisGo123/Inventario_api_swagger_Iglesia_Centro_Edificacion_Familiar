namespace Iglesia_Bautista_Centro_Familiar_NET.CORE.Base_Dato_DTO
{
    public class SolicitudDePrestamoDTO
    {
        public int IdSolicitudDePrestamo { get; set; }
        public DateTime? FechaOperaciones { get; set; }
        public int? IdInventario { get; set; }
        public int? Cantidad { get; set; }
        public string? Lugar { get; set; }
        public string? FechaEntrega { get; set; }
        public int? IdResponsable { get; set; }

        public string? NombreResponsable { get; set; }

    }
}
