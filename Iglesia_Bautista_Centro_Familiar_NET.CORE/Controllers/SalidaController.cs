using Iglesia_Bautista_Centro_Familiar_NET.CORE.Base_Dato_DTO;
using Iglesia_Bautista_Centro_Familiar_NET.CORE.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Iglesia_Bautista_Centro_Familiar_NET.CORE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalidaController : ControllerBase
    {
        private readonly dbIglesia_Bautista_Centro_FamiliarContext _dbcontext;

        public SalidaController(dbIglesia_Bautista_Centro_FamiliarContext context)
        {
            _dbcontext = context;
        }

        [HttpPost("Guardar")]
        public async Task<IActionResult> GuardarSalida([FromForm] SalidaDTO salida)
        {
            try
            {
                byte[] archivoPdfBytes = null;

                // Verificar si el archivo ha sido enviado
                if (salida.ArchivoPdf != null)
                {
                    // Crear un MemoryStream para almacenar el archivo en bytes
                    using (var memoryStream = new MemoryStream())
                    {
                        // Copiar el contenido del archivo al MemoryStream
                        await salida.ArchivoPdf.CopyToAsync(memoryStream);

                        // Convertir el MemoryStream a un arreglo de bytes
                        archivoPdfBytes = memoryStream.ToArray();
                    }
                }

                // Crear la salida con los datos recibidos
                var nuevaSalida = new Salida
                {
                    IdProducto = salida.IdProducto,
                    Cantidad = salida.Cantidad,
                    ArchivoPdf = archivoPdfBytes, // Asignar el arreglo de bytes
                };

                // Guardar la nueva salida en la base de datos
                _dbcontext.Salida.Add(nuevaSalida);
                await _dbcontext.SaveChangesAsync();

                // Verificar si el IdProducto no es nulo antes de continuar
                if (!salida.IdProducto.HasValue)
                {
                    return BadRequest(new { mensaje = "El IdProducto no puede ser nulo." });
                }

                // Buscar el producto por IdProducto
                var producto = await _dbcontext.Productos.FirstOrDefaultAsync(p => p.IdProducto == salida.IdProducto.Value);

                // Verificar si el producto existe
                if (producto != null)
                {
                    // Si el producto existe, restamos la cantidad en existencia
                    if (producto.Existencia >= salida.Cantidad)
                    {
                        producto.Existencia -= salida.Cantidad;

                        // Actualizamos el producto en la base de datos
                        _dbcontext.Productos.Update(producto);
                        await _dbcontext.SaveChangesAsync();

                        // Aquí manejamos el inventario
                        var inventario = await _dbcontext.Inventarios
                            .FirstOrDefaultAsync(i => i.IdProducto == salida.IdProducto.Value);

                        if (inventario != null)
                        {
                            // Si el inventario existe, restamos la cantidad al StockMaximo
                            if (inventario.Cantidad >= salida.Cantidad)
                            {
                                inventario.Cantidad -= salida.Cantidad;

                                // Actualizamos el inventario en la base de datos
                                _dbcontext.Inventarios.Update(inventario);
                                await _dbcontext.SaveChangesAsync();
                            }
                            else
                            {
                                return BadRequest(new { mensaje = "La cantidad solicitada excede la cantidad en inventario." });
                            }
                        }
                        else
                        {
                            return BadRequest(new { mensaje = "No se encontró el inventario para el producto." });
                        }
                    }
                    else
                    {
                        return BadRequest(new { mensaje = "La cantidad solicitada excede la cantidad en existencia del producto." });
                    }
                }
                else
                {
                    return BadRequest(new { mensaje = "Producto no encontrado." });
                }

                return Ok(new { mensaje = "Salida creada correctamente", id = nuevaSalida.IdSalida });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al crear la salida.", error = ex.Message });
            }
        }


        [HttpGet("Lista")]
        public async Task<IActionResult> ObtenerListaSalidas()
        {
            try
            {
                // Obtener todas las salidas de la base de datos
                var salidas = await _dbcontext.Salida
                    .Include(s => s.IdProductoNavigation)  // Incluir información del producto asociado a la salida
                    .ToListAsync();           // Convertir a lista

                // Verificar si hay salidas
                if (salidas == null || !salidas.Any())
                {
                    return NotFound(new { mensaje = "No se encontraron salidas." });
                }

                // Retornar las salidas
                return Ok(salidas);
            }
            catch (Exception ex)
            {
                // En caso de error, devolver el mensaje de error
                return StatusCode(500, new { mensaje = "Error al obtener la lista de salidas.", error = ex.Message });
            }
        }






        // Método para listar todas las salidas con la información del producto y sus detalles adicionales



        [HttpDelete("Eliminar/{idSalida}")]
        public async Task<IActionResult> EliminarSalida(int idSalida)
        {
            using var transaction = await _dbcontext.Database.BeginTransactionAsync();
            try
            {
                // Buscar la salida por id
                var salida = await _dbcontext.Salida
                    .Include(s => s.IdProductoNavigation)
                    .FirstOrDefaultAsync(s => s.IdSalida == idSalida);

                if (salida == null)
                {
                    return NotFound("La salida no fue encontrada.");
                }

                // Obtener el producto y restablecer la cantidad eliminada
                var producto = salida.IdProductoNavigation;
                if (producto != null)
                {
                    producto.Existencia += salida.Cantidad;
                    _dbcontext.Productos.Update(producto);
                }

                // Eliminar la salida
                _dbcontext.Salida.Remove(salida);
                await _dbcontext.SaveChangesAsync();

                await transaction.CommitAsync();

                return Ok($"Salida con ID {idSalida} eliminada correctamente y la existencia del producto ha sido actualizada.");
            }
            catch
            {
                await transaction.RollbackAsync();
                return StatusCode(500, "Ocurrió un error al eliminar la salida.");
            }
        }

    }
}