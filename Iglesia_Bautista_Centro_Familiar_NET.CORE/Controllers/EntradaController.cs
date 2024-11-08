using Iglesia_Bautista_Centro_Familiar_NET.CORE.Base_Dato_DTO;
using Iglesia_Bautista_Centro_Familiar_NET.CORE.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;


namespace Iglesia_Bautista_Centro_Familiar_NET.CORE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EntradaController : ControllerBase
    {
        private readonly dbIglesia_Bautista_Centro_FamiliarContext _dbcontext;

        public EntradaController(dbIglesia_Bautista_Centro_FamiliarContext context)
        {
            _dbcontext = context;
        }

        // POST: api/Entrada/Guardar
        [HttpPost("Guardar")]
        public async Task<IActionResult> GuardarEntrada([FromForm] EntradaDTO entrada)
        {
            try
            {
                byte[] archivoPdfBytes = null;

                // Verificar si el archivo ha sido enviado
                if (entrada.ArchivoPdf != null)
                {
                    // Crear un MemoryStream para almacenar el archivo en bytes
                    using (var memoryStream = new MemoryStream())
                    {
                        // Copiar el contenido del archivo al MemoryStream
                        await entrada.ArchivoPdf.CopyToAsync(memoryStream);

                        // Convertir el MemoryStream a un arreglo de bytes
                        archivoPdfBytes = memoryStream.ToArray();
                    }
                }

                // Crear la entrada con los datos recibidos
                var nuevaEntrada = new Entrada
                {
                    IdProducto = entrada.IdProducto,
                    Cantidad = entrada.Cantidad,
                    ArchivoPdf = archivoPdfBytes, // Asignar el arreglo de bytes
                };

                // Guardar la nueva entrada en la base de datos
                _dbcontext.Entrada.Add(nuevaEntrada);
                await _dbcontext.SaveChangesAsync();

                // Verificar si el IdProducto no es nulo antes de continuar
                if (!entrada.IdProducto.HasValue)
                {
                    return BadRequest(new { mensaje = "El IdProducto no puede ser nulo." });
                }

                // Buscar el producto por IdProducto
                var producto = await _dbcontext.Productos.FirstOrDefaultAsync(p => p.IdProducto == entrada.IdProducto.Value);

                // Verificar si el producto existe
                if (producto != null)
                {
                    // Si el producto existe, actualizamos la cantidad en existencia
                    producto.Existencia += entrada.Cantidad;

                    // Actualizamos el producto en la base de datos
                    _dbcontext.Productos.Update(producto);
                    await _dbcontext.SaveChangesAsync();

                    // Aquí manejamos el inventario
                    var inventario = await _dbcontext.Inventarios
                        .FirstOrDefaultAsync(i => i.IdProducto == entrada.IdProducto.Value);

                    if (inventario != null)
                    {
                        // Si el inventario existe, sumamos la cantidad al StockMaximo
                        inventario.StockMaximo += entrada.Cantidad;

                        // Además, actualizamos la cantidad del inventario
                        inventario.Cantidad += entrada.Cantidad;

                        // Actualizamos el inventario en la base de datos
                        _dbcontext.Inventarios.Update(inventario);
                        await _dbcontext.SaveChangesAsync();
                    }
                    else
                    {
                        // Si el inventario no existe, lo creamos con el StockMaximo inicializado
                        var nuevoInventario = new Inventario
                        {
                            IdProducto = entrada.IdProducto.Value,
                            StockMaximo = entrada.Cantidad,
                            Cantidad = entrada.Cantidad // Inicializamos la cantidad con la misma cantidad
                        };

                        // Guardamos el nuevo inventario en la base de datos
                        _dbcontext.Inventarios.Add(nuevoInventario);
                        await _dbcontext.SaveChangesAsync();
                    }
                }
                else
                {
                    // Si el producto no existe, lo creamos
                    var nuevoProducto = new Producto
                    {
                        IdProducto = entrada.IdProducto.Value, // Usar el valor de IdProducto (que no es null)
                        Existencia = entrada.Cantidad, // La cantidad que estamos agregando al inventario
                                                       // Otros campos del producto si es necesario
                    };

                    // Guardar el nuevo producto en la base de datos
                    _dbcontext.Productos.Add(nuevoProducto);
                    await _dbcontext.SaveChangesAsync();

                    // Crear el inventario para el producto
                    var nuevoInventario = new Inventario
                    {
                        IdProducto = entrada.IdProducto.Value,
                        StockMaximo = entrada.Cantidad,
                        Cantidad = entrada.Cantidad
                    };

                    // Guardar el inventario del nuevo producto
                    _dbcontext.Inventarios.Add(nuevoInventario);
                    await _dbcontext.SaveChangesAsync();
                }

                return Ok(new { mensaje = "Entrada creada correctamente", id = nuevaEntrada.IdEntrada });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al crear la entrada.", error = ex.Message });
            }
        }




        [HttpGet("Lista")]
        public async Task<IActionResult> ObtenerListaEntradas()
        {
            try
            {
                // Obtener la lista de entradas de la base de datos
                var listaEntradas = await _dbcontext.Entrada
                    .Include(e => e.IdProductoNavigation)  // Para incluir información del producto si es necesario
                    .ToListAsync();

                // Si la lista está vacía, devolver un mensaje adecuado
                if (listaEntradas == null || listaEntradas.Count == 0)
                {
                    return NotFound(new { mensaje = "No se encontraron entradas." });
                }

                // Devolver la lista de entradas
                return Ok(listaEntradas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener las entradas.", error = ex.Message });
            }
        }







        // GET: api/Entrada/Listar


        // DELETE: api/Entrada/Eliminar/{idEntrada}
        [HttpDelete("Eliminar/{idEntrada}")]
        public async Task<IActionResult> EliminarEntrada(int idEntrada)
        {
            using var transaction = await _dbcontext.Database.BeginTransactionAsync();
            try
            {
                // Buscar la entrada por su ID
                var entrada = await _dbcontext.Entrada
                    .Include(e => e.IdProductoNavigation)
                    .FirstOrDefaultAsync(e => e.IdEntrada == idEntrada);

                if (entrada == null)
                {
                    return NotFound(new { mensaje = "La entrada no fue encontrada." });
                }

                // Obtener el producto relacionado
                var producto = entrada.IdProductoNavigation;
                if (producto != null)
                {
                    // Actualizar la existencia del producto restando la cantidad de la entrada eliminada
                    producto.Existencia = (producto.Existencia ?? 0) - (entrada.Cantidad ?? 0);
                    _dbcontext.Productos.Update(producto);
                }

                // Eliminar la entrada
                _dbcontext.Entrada.Remove(entrada);
                await _dbcontext.SaveChangesAsync();

                // Confirmar la transacción
                await transaction.CommitAsync();

                return Ok(new { mensaje = $"Entrada con ID {idEntrada} eliminada correctamente y la existencia del producto ha sido actualizada." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { mensaje = "Ocurrió un error al eliminar la entrada.", error = ex.Message });
            }
        }
    }
}
