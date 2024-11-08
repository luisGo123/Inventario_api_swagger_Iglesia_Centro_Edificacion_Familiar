namespace Iglesia_Bautista_Centro_Edificacion_Familiar_Project.DTO
{
    public class ProductoDTO
    {
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; }

        // Permitir que 'Color' sea opcional
        public string? Color { get; set; }

        public int IdCategoria { get; set; }
        // Permitir que 'CategoriaNombre' sea opcional
        public string? CategoriaNombre { get; set; }

        public int IdTipoProducto { get; set; }
        // Permitir que 'TipoProductoNombre' sea opcional
        public string? TipoProductoNombre { get; set; }

        public int IdMarca { get; set; }
        // Permitir que 'MarcaNombre' sea opcional
        public string? MarcaNombre { get; set; }

        public int Existencia { get; set; }
        public string Estaus { get; set; }

        public static string ObtenerEstado(bool? estaus)
        {
            if (estaus == true)
                return "Activo";
            else if (estaus == false)
                return "Inactivo";
            else
                return "En Proceso";
        }
    }
}
