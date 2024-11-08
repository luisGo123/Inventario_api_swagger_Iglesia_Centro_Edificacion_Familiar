using Iglesia_Bautista_Centro_Edificacion_Familiar_Project.DTO;
using Iglesia_Bautista_Centro_Familiar_NET.CORE;
using Iglesia_Bautista_Centro_Familiar_NET.CORE.Base_Dato_DTO;
using Iglesia_Bautista_Centro_Familiar_NET.CORE.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Iglesia_Bautista_Centro_Edificacion_Familiar_Project.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("PermitirTodo")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly dbIglesia_Bautista_Centro_FamiliarContext _dbcontext;

        public ProductoController(dbIglesia_Bautista_Centro_FamiliarContext dbcontext)
        {
            _dbcontext = dbcontext;
        }



        [HttpGet]
        [Route("Lista")]
        public IActionResult Lista()
        {
            try
            {
                var productos = _dbcontext.Productos
                    .Where(r => r.Estaus == true) // Filtrar solo productos activos
                    .Include(p => p.IdCategoriaNavigation)
                    .Include(p => p.IdTipoProductoNavigation)
                    .Include(p => p.IdMarcaNavigation)
                    .Select(r => new ProductoDTO
                    {
                        IdProducto = r.IdProducto,
                        NombreProducto = r.NombreProducto,
                        Color = r.Color,
                        IdCategoria = r.IdCategoria ?? 0,
                        CategoriaNombre = r.IdCategoriaNavigation.Nombre,
                        IdTipoProducto = r.IdTipoProducto ?? 0,
                        TipoProductoNombre = r.IdTipoProductoNavigation.Nombre,
                        IdMarca = r.IdMarca ?? 0,
                        MarcaNombre = r.IdMarcaNavigation.Nombre,
                        Existencia = r.Existencia ?? 0,
                        Estaus = ProductoDTO.ObtenerEstado(r.Estaus) // Convertir a texto
                    })
                    .ToList();

                var totalProductos = productos.Count();  // Usar el número de productos activos

                return StatusCode(StatusCodes.Status200OK, new
                {
                    mensaje = "La petición realizada fue exitosa",
                    response = productos,
                    totalProductos = totalProductos
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = ex.Message });
            }
        }


        [HttpGet]
        [Route("ListaFiltrada")]
        public IActionResult ListaFiltrada()
        {
            try
            {
                // Obtener productos inactivos (suponiendo que inactivo es 'false' o 'null')
                var productosInactivos = _dbcontext.Productos
                    .Where(r => r.Estaus == false || r.Estaus == null)  // Filtrar inactivos
                    .Include(p => p.IdCategoriaNavigation)
                    .Include(p => p.IdTipoProductoNavigation)
                    .Include(p => p.IdMarcaNavigation)
                    .Select(r => new ProductoDTO
                    {
                        IdProducto = r.IdProducto,
                        NombreProducto = r.NombreProducto,
                        Color = r.Color,
                        IdCategoria = r.IdCategoria ?? 0,
                        CategoriaNombre = r.IdCategoriaNavigation.Nombre,
                        IdTipoProducto = r.IdTipoProducto ?? 0,
                        TipoProductoNombre = r.IdTipoProductoNavigation.Nombre,
                        IdMarca = r.IdMarca ?? 0,
                        MarcaNombre = r.IdMarcaNavigation.Nombre,
                        Existencia = r.Existencia ?? 0,
                        Estaus = ProductoDTO.ObtenerEstado(r.Estaus) // Convertir a texto
                    })
                    .ToList();

                // Obtener productos en proceso (suponiendo que en proceso es 'true')
                var productosEnProceso = _dbcontext.Productos
                    .Where(r => r.Estaus == true)  // Filtrar activos (en proceso)
                    .Include(p => p.IdCategoriaNavigation)
                    .Include(p => p.IdTipoProductoNavigation)
                    .Include(p => p.IdMarcaNavigation)
                    .Select(r => new ProductoDTO
                    {
                        IdProducto = r.IdProducto,
                        NombreProducto = r.NombreProducto,
                        Color = r.Color,
                        IdCategoria = r.IdCategoria ?? 0,
                        CategoriaNombre = r.IdCategoriaNavigation.Nombre,
                        IdTipoProducto = r.IdTipoProducto ?? 0,
                        TipoProductoNombre = r.IdTipoProductoNavigation.Nombre,
                        IdMarca = r.IdMarca ?? 0,
                        MarcaNombre = r.IdMarcaNavigation.Nombre,
                        Existencia = r.Existencia ?? 0,
                        Estaus = ProductoDTO.ObtenerEstado(r.Estaus) // Convertir a texto
                    })
                    .ToList();

                // Responder con las dos listas
                return StatusCode(StatusCodes.Status200OK, new
                {
                    mensaje = "La petición realizada fue exitosa",
                    productosInactivos = productosInactivos,
                    productosEnProceso = productosEnProceso
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = ex.Message });
            }
        }



        [HttpGet]
        [Route("BuscarPorNombre/{nombre}")]
        public IActionResult BuscarPorNombre(string nombre)
        {
            try
            {
                var productos = _dbcontext.Productos
                    .Where(r => r.NombreProducto.Contains(nombre)) // Busca productos que contengan el nombre
                    .Select(r => new
                    {
                        r.IdProducto,
                        r.NombreProducto,
                        r.Color,
                        r.IdCategoria,
                        r.IdTipoProducto,
                        r.IdMarca,
                        r.Existencia,
                    })
                    .ToList();

                if (productos.Count == 0)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new { mensaje = "No se encontraron productos que coincidan." });
                }

                return StatusCode(StatusCodes.Status200OK, new { mensaje = "La búsqueda fue exitosa", response = productos });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = ex.Message });
            }
        }

        [HttpGet]
        [Route("Obtener/{IdProducto}")]
        public IActionResult ObtenerProducto(int IdProducto)
        {
            try
            {
                // Buscar el producto por id
                var producto = _dbcontext.Productos
                    .Include(p => p.IdCategoriaNavigation)
                    .Include(p => p.IdTipoProductoNavigation)
                    .Include(p => p.IdMarcaNavigation)
                    .Where(p => p.IdProducto == IdProducto)
                    .Select(r => new ProductoDTO
                    {
                        IdProducto = r.IdProducto,
                        NombreProducto = r.NombreProducto,
                        Color = r.Color,
                        IdCategoria = r.IdCategoria ?? 0,
                        CategoriaNombre = r.IdCategoriaNavigation.Nombre,
                        IdTipoProducto = r.IdTipoProducto ?? 0,
                        TipoProductoNombre = r.IdTipoProductoNavigation.Nombre,
                        IdMarca = r.IdMarca ?? 0,
                        MarcaNombre = r.IdMarcaNavigation.Nombre,
                        Existencia = r.Existencia ?? 0,
                        Estaus = ProductoDTO.ObtenerEstado(r.Estaus) // Convertir a texto
                    })
                    .FirstOrDefault(); // Obtener el primer producto o null si no existe

                // Si el producto no se encuentra, devolver un error
                if (producto == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new { mensaje = "Producto no encontrado" });
                }

                return StatusCode(StatusCodes.Status200OK, new
                {
                    mensaje = "Producto obtenido exitosamente",
                    response = producto
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = ex.Message });
            }
        }





        [HttpPost]
        [Route("Guardar")]
        public IActionResult Guardar([FromBody] ProductoDTO objeto)
        {
            try
            {
                // Verificamos si el producto ya existe
                var productoExistente = _dbcontext.Productos
                    .FirstOrDefault(p => p.NombreProducto.ToLower() == objeto.NombreProducto.ToLower());

                if (productoExistente != null)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { mensaje = "Ya existe un producto con ese nombre." });
                }

                // Validación condicional de campos opcionales
                string? marcaNombre = string.IsNullOrEmpty(objeto.MarcaNombre) ? null : objeto.MarcaNombre;
                string? categoriaNombre = string.IsNullOrEmpty(objeto.CategoriaNombre) ? null : objeto.CategoriaNombre;
                string? tipoProductoNombre = string.IsNullOrEmpty(objeto.TipoProductoNombre) ? null : objeto.TipoProductoNombre;
                string? color = string.IsNullOrEmpty(objeto.Color) ? null : objeto.Color;

                // Convertir el estado de string a bool? (si es necesario)
                bool? estado = null;
                if (objeto.Estaus != null)
                {
                    estado = objeto.Estaus.ToLower() == "activo" ? true :
                             objeto.Estaus.ToLower() == "inactivo" ? false : (bool?)null;
                }

                // Crear instancia del producto para guardar
                var creacionProducto = new Producto
                {
                    NombreProducto = objeto.NombreProducto,
                    Color = color,
                    IdCategoria = objeto.IdCategoria,
                    IdTipoProducto = objeto.IdTipoProducto,
                    IdMarca = objeto.IdMarca,
                    Existencia = objeto.Existencia,
                    Estaus = estado // Usamos el valor bool? convertido
                };

                // Guardamos el producto en la base de datos
                _dbcontext.Productos.Add(creacionProducto);
                _dbcontext.SaveChanges();

                return StatusCode(StatusCodes.Status200OK, new { savedProduct = creacionProducto, mensaje = "El Producto se ha Guardado" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = ex.Message });
            }
        }






        [HttpPut]
        [Route("Editar")]
        public IActionResult Editar([FromBody] ProductoDTO objeto)
        {
            try
            {
                // Verificamos si el producto existe
                var productoExistente = _dbcontext.Productos
                    .FirstOrDefault(p => p.IdProducto == objeto.IdProducto);

                if (productoExistente == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new { mensaje = "Producto no encontrado." });
                }

                // Verificamos si ya existe otro producto con el mismo nombre (excluyendo el producto que estamos editando)
                var productoConMismoNombre = _dbcontext.Productos
                    .FirstOrDefault(p => p.NombreProducto.ToLower() == objeto.NombreProducto.ToLower() && p.IdProducto != objeto.IdProducto);

                if (productoConMismoNombre != null)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { mensaje = "Ya existe un producto con ese nombre." });
                }

                // Validación condicional de campos opcionales
                string? marcaNombre = string.IsNullOrEmpty(objeto.MarcaNombre) ? null : objeto.MarcaNombre;
                string? categoriaNombre = string.IsNullOrEmpty(objeto.CategoriaNombre) ? null : objeto.CategoriaNombre;
                string? tipoProductoNombre = string.IsNullOrEmpty(objeto.TipoProductoNombre) ? null : objeto.TipoProductoNombre;
                string? color = string.IsNullOrEmpty(objeto.Color) ? null : objeto.Color;

                // Convertir el estado de string a bool? (si es necesario)
                bool? estado = null;
                if (objeto.Estaus != null)
                {
                    estado = objeto.Estaus.ToLower() == "activo" ? true :
                             objeto.Estaus.ToLower() == "inactivo" ? false : (bool?)null;
                }

                // Actualizar las propiedades del producto existente
                productoExistente.NombreProducto = objeto.NombreProducto;
                productoExistente.Color = color;
                productoExistente.IdCategoria = objeto.IdCategoria;
                productoExistente.IdTipoProducto = objeto.IdTipoProducto;
                productoExistente.IdMarca = objeto.IdMarca;
                productoExistente.Existencia = objeto.Existencia;
                productoExistente.Estaus = estado; // Usamos el valor bool? convertido

                // Guardamos los cambios en la base de datos
                _dbcontext.Productos.Update(productoExistente);
                _dbcontext.SaveChanges();

                return StatusCode(StatusCodes.Status200OK, new { updatedProduct = productoExistente, mensaje = "El Producto se ha Actualizado" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = ex.Message });
            }
        }

        [HttpPut]
        [Route("CambiarEstado")]
        public IActionResult CambiarEstado([FromBody] ProductoEstadoDTO objeto)
        {
            try
            {
                var productoExistente = _dbcontext.Productos
                    .FirstOrDefault(p => p.IdProducto == objeto.IdProducto);

                if (productoExistente == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new { mensaje = "Producto no encontrado." });
                }

                // Asignamos directamente el estado del producto existente
                productoExistente.Estaus = objeto.Estado;

                _dbcontext.Productos.Update(productoExistente);
                _dbcontext.SaveChanges();

                return StatusCode(StatusCodes.Status200OK, new { mensaje = "El estado del producto se ha actualizado exitosamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = ex.Message });
            }
        }






        [HttpDelete]
        [Route("Eliminar/{IdProducto:int}")]
        public IActionResult Eliminar(int IdProducto)
        {

            Producto Productos = _dbcontext.Productos.Find(IdProducto);

            if (Productos == null)
            {
                return BadRequest(" no encontrado");

            }

            try
            {

                _dbcontext.Productos.Remove(Productos);
                _dbcontext.SaveChanges();

                return StatusCode(StatusCodes.Status200OK, new { mensaje = " Se Elimino Exitosamente" });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException?.Message);
                return StatusCode(StatusCodes.Status200OK, new { mensaje = ex.Message });
            }
        }

    }
}