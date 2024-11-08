using Iglesia_Bautista_Centro_Familiar_NET.CORE.Base_Dato_DTO;
using Iglesia_Bautista_Centro_Familiar_NET.CORE.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Iglesia_Bautista_Centro_Familiar_NET.CORE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventarioController : ControllerBase
    {
        private readonly dbIglesia_Bautista_Centro_FamiliarContext _context;

        public InventarioController(dbIglesia_Bautista_Centro_FamiliarContext context)
        {
            _context = context;
        }

        [HttpPost("Guardar")]
        public async Task<IActionResult> Guardar(Inventario inventario)
        {
            if (inventario == null || inventario.IdProducto == null || inventario.StockMaximo == null)
            {
                return BadRequest("Datos inválidos");
            }

            // Validar que el StockMaximo sea mayor que el StockMinimo y que no sean iguales
            if (inventario.StockMinimo >= inventario.StockMaximo)
            {
                return BadRequest("El Stock Máximo debe ser mayor que el Stock Mínimo y no pueden ser iguales.");
            }

            // Validar que la cantidad no sea mayor que el StockMaximo
            if (inventario.Cantidad > inventario.StockMaximo)
            {
                return BadRequest("La cantidad no puede ser mayor que el Stock Máximo.");
            }

            // Obtiene el producto correspondiente
            var producto = await _context.Productos.FindAsync(inventario.IdProducto);

            if (producto == null)
            {
                return NotFound("Producto no encontrado");
            }

            // Asigna la cantidad del inventario a la existencia del producto
            producto.Existencia = inventario.Cantidad;

            await _context.Inventarios.AddAsync(inventario);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Guardar), new { id = inventario.IdInventario }, inventario);
        }

        [HttpPut("Editar")]
        public async Task<IActionResult> EditarInventario([FromBody] Inventario inventario)
        {
            if (inventario == null || inventario.IdInventario == 0)
            {
                return BadRequest("Datos inválidos");
            }

            // Validar que el StockMaximo sea mayor que el StockMinimo y que no sean iguales
            if (inventario.StockMinimo >= inventario.StockMaximo)
            {
                return BadRequest("El Stock Máximo debe ser mayor que el Stock Mínimo y no pueden ser iguales.");
            }

            // Validar que la cantidad no sea mayor que el StockMaximo
            if (inventario.Cantidad > inventario.StockMaximo)
            {
                return BadRequest("La cantidad no puede ser mayor que el Stock Máximo.");
            }

            var producto = await _context.Productos.FindAsync(inventario.IdProducto);
            if (producto == null)
            {
                return NotFound("Producto no encontrado");
            }

            // Asigna la cantidad del inventario a la existencia del producto
            producto.Existencia = inventario.Cantidad;

            _context.Entry(inventario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InventarioExists(inventario.IdInventario))
                {
                    return NotFound("Inventario no encontrado");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        private bool InventarioExists(int IdInventario)
        {
            return _context.Inventarios.Any(e => e.IdInventario == IdInventario);
        }


        [HttpGet("Lista")]
        public async Task<ActionResult<IEnumerable<InventarioDTO>>> Lista()
        {
            var inventarios = await _context.Inventarios
                .Include(i => i.IdProductoNavigation)
                .Include(i => i.IdAlmacenesNavigation)
                .Where(i => i.Statud == true)
                .Select(i => new InventarioDTO
                {
                    IdInventario = i.IdInventario,
                    FechaInventario = i.FechaInventario,
                    NombreProducto = i.IdProductoNavigation.NombreProducto,
                    ColorProducto = i.IdProductoNavigation.Color,
                    IdCategoria = i.IdProductoNavigation.IdCategoria,
                    NombreCategoria = i.IdProductoNavigation.IdCategoriaNavigation.Nombre,
                    IdMarca = i.IdProductoNavigation.IdMarca,
                    NombreMarca = i.IdProductoNavigation.IdMarcaNavigation.Nombre,
                    StockMinimo = i.StockMinimo,
                    StockMaximo = i.StockMaximo,
                    Cantidad = i.Cantidad,
                    Statud = i.Statud,
                    NombreAlmacen = i.IdAlmacenesNavigation.Nombre
                })
                .ToListAsync();

            return Ok(inventarios);
        }


        [HttpGet("ListaInactivos")]
        public async Task<ActionResult<IEnumerable<InventarioDTO>>> ListaInactivos()
        {

            var inventariosInactivos = await _context.Inventarios
                .Where(i => i.Statud.HasValue && !i.Statud.Value)
                .Include(i => i.IdProductoNavigation)
                .Include(i => i.IdAlmacenesNavigation)
                .Select(i => new InventarioDTO
                {
                    IdInventario = i.IdInventario,
                    FechaInventario = i.FechaInventario,
                    NombreProducto = i.IdProductoNavigation.NombreProducto,
                    ColorProducto = i.IdProductoNavigation.Color,
                    IdCategoria = i.IdProductoNavigation.IdCategoria,
                    NombreCategoria = i.IdProductoNavigation.IdCategoriaNavigation.Nombre,
                    IdMarca = i.IdProductoNavigation.IdMarca,
                    NombreMarca = i.IdProductoNavigation.IdMarcaNavigation.Nombre,
                    StockMinimo = i.StockMinimo,
                    StockMaximo = i.StockMaximo,
                    Cantidad = i.Cantidad,
                    Statud = i.Statud,
                    NombreAlmacen = i.IdAlmacenesNavigation.Nombre
                })
            .ToListAsync();

            return Ok(inventariosInactivos);
        }




        [HttpGet("{IdInventario}")]
        public async Task<ActionResult<InventarioDTO>> ObtenerInventario(int IdInventario)
        {
            var inventario = await _context.Inventarios
                .Include(i => i.IdProductoNavigation)
                .Include(i => i.IdAlmacenesNavigation)
                .Select(i => new InventarioDTO
                {
                    IdInventario = i.IdInventario,
                    FechaInventario = i.FechaInventario,
                    NombreProducto = i.IdProductoNavigation.NombreProducto,
                    ColorProducto = i.IdProductoNavigation.Color,
                    IdCategoria = i.IdProductoNavigation.IdCategoria,
                    NombreCategoria = i.IdProductoNavigation.IdCategoriaNavigation.Nombre,
                    IdMarca = i.IdProductoNavigation.IdMarca,
                    NombreMarca = i.IdProductoNavigation.IdMarcaNavigation.Nombre,
                    StockMinimo = i.StockMinimo,
                    StockMaximo = i.StockMaximo,
                    Cantidad = i.Cantidad,
                    Statud = i.Statud,
                    NombreAlmacen = i.IdAlmacenesNavigation.Nombre
                })
                .FirstOrDefaultAsync(i => i.IdInventario == IdInventario);

            if (inventario == null)
            {
                return NotFound("Inventario no encontrado");
            }

            return Ok(inventario);
        }



   
     

        [HttpPut("CambiarEstado")]
        public async Task<IActionResult> CambiarEstado([FromBody] NuevoEstadoDTO estadoDto)
        {
            if (estadoDto == null)
            {
                return BadRequest("Datos inválidos");
            }


            var inventario = await _context.Inventarios.FindAsync(estadoDto.IdInventario);
            if (inventario == null)
            {
                return NotFound("Inventario no encontrado");
            }


            var tieneSolicitudPrestamo = await _context.SolicitudDePrestamos
                .AnyAsync(sp => sp.IdInventario == estadoDto.IdInventario);

            if (tieneSolicitudPrestamo)
            {
                return Conflict("No se puede cambiar el estado del inventario porque está asociado a una solicitud de préstamo.");
            }

            inventario.Statud = estadoDto.NuevoEstado;


            _context.Entry(inventario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InventarioExists(inventario.IdInventario))
                {
                    return NotFound("Inventario no encontrado");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }



        [HttpDelete("{IdInventario}")]
        public async Task<IActionResult> EliminarInventario(int IdInventario)
        {
            var inventario = await _context.Inventarios.FindAsync(IdInventario);
            if (inventario == null)
            {
                return NotFound("Inventario no encontrado");
            }

            _context.Inventarios.Remove(inventario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}

