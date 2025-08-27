
using Identity.Api.DTO;
using Identity.Api.Interfaces;
using Identity.Api.Paginado;
using Identity.Api.Reporteria;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Modelo.laconcordia.Modelo.Database;
using QuestPDF.Infrastructure;
using FluentFTP;
using Microsoft.Extensions.Configuration;
using System.Net;
using FluentFTP.Exceptions;
using System.IO;
using Microsoft.AspNetCore.Http;


//se usar FluentFTP par aenvio ft

namespace Identity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class FichapersonaController : Controller
    {
        private readonly DbAa5796GmoraContext _context;

        private readonly IFichapersona _fichapersona;
        public FichapersonaController(IFichapersona fichapersona)
        {
            _fichapersona = fichapersona;
        }
        [HttpGet("GetFichaPersonalInfoAll")]
        public IActionResult GetAll()
        {
            var lista = _fichapersona.GetFichaPersonalInfoAll();
            return Ok(lista);
        }
        [HttpGet("GetFichaPersonalById/{cedula}")]
        public IActionResult GetById(string cedula)
        {
            var item = _fichapersona.GetFichaPersonalById(cedula);
            if (item == null)
                return NotFound("Ficha personal no encontrada.");
            return Ok(item);
        }

        [HttpPost("InsertFichaPersonal")]
        public IActionResult Create([FromBody] FichapersonalDTO nueva)
        {
            try
            {
                _fichapersona.InsertFichaPersonal(nueva);
                return Ok("Ficha personal creada correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest("Error al crear: " + ex.Message);
            }
        }

        [HttpPut("UpdateFichaPersonal")]
        public IActionResult Update([FromBody] FichapersonalDTO actualizada)
        {
            try
            {
                _fichapersona.UpdateFichaPersonal(actualizada);
                return Ok("Ficha personal actualizada correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest("Error al actualizar: " + ex.Message);
            }
        }

        [HttpDelete("DeleteFichaPersonalById/{cedula}")]
        public IActionResult Delete(string cedula)
        {
            try
            {
                _fichapersona.DeleteFichaPersonalById(cedula);
                return Ok("Ficha personal eliminada correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest("Error al eliminar: " + ex.Message);
            }
        }

        [HttpGet("GetFichaPersonalPaginados")]
        public async Task<IActionResult> GetFichaPersonalPaginados(
            int pagina = 1,
            int pageSize = PaginadorHelper.NumeroDeDatosPorPagina,
            string? filtro = null,
            string? estado = null)
        {
            try
            {
                var resultado = await _fichapersona.GetFichaPersonalPaginados(pagina, pageSize, filtro, estado);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("ExportarFichaCompleta")]
        public IActionResult ExportarFichaCompleta([FromBody] ExportFichaDTO exportData)
        {
            if (string.IsNullOrWhiteSpace(exportData?.Ficha?.Cedula))
                return BadRequest("No se recibió la cédula para exportar.");

            QuestPDF.Settings.License = LicenseType.Community;

            // 🔹 Usar DbContext temporal para no afectar el resto
            using var context = new DbAa5796GmoraContext();

            // 🔹 Buscar ficha personal con relaciones
            var ficha = context.Fichapersonals
                .Include(f => f.FknacionalidadNavigation)
                .Include(f => f.FkestadocivilNavigation)
                .Include(f => f.FkcargoNavigation)
                .Include(f => f.FktipolicenciaNavigation)
                .Include(f => f.FkniveleducacionNavigation)
                .FirstOrDefault(f => f.Cedula == exportData.Ficha.Cedula);

            if (ficha == null)
                return NotFound("No se encontró la ficha personal.");

            // 🔹 Buscar unidad si existe
            Unidad unidad = null;
            if (!string.IsNullOrEmpty(ficha.Fkunidad))
                unidad = context.Unidads.FirstOrDefault(u => u.Unidad1 == ficha.Fkunidad);

            // 🔹 Buscar dueño del puesto si existe
            Duenopuesto duenopuesto = null;
            if (!string.IsNullOrEmpty(ficha.Fkdpuesto))
                duenopuesto = context.Duenopuestos.FirstOrDefault(d => d.Cedula == ficha.Fkdpuesto);

            // 🔹 Beneficiarios y ayudas
            var beneficiarios = context.Segurovida
                .Where(b => b.CiAfiliado == ficha.Cedula)
                .Where(b => b.CiAfiliado == ficha.Cedula)
                .Include(b => b.PkparentescoNavigation)
                .AsEnumerable()
                .Select(b => new SegurovidumDTO
                {
                    CiBeneficiario = b.CiBeneficiario,
                    Nombres = b.Nombres,
                    Apellidos = b.Apellidos,
                    Pkparentesco = b.Pkparentesco,
                    ParentescoDescripcion = b.PkparentescoNavigation?.Parentesco1,
                    CiAfiliado = b.CiAfiliado,
                    Telefono = b.Telefono,
                    Tipo = b.Tipo // 1 = BENEFICIARIO, 2 = AYUDA
                })
                .ToList();

            // 🔹 Armar DTO completo para el PDF
            var exportDataCompleto = new ExportFichaDTO
            {
                Ficha = new FichapersonalDTO
                {
                    Cedula = ficha.Cedula,
                    Nombre = ficha.Nombre,
                    Apellidos = ficha.Apellidos,
                    TipoLicenciaDescripcion = ficha.FktipolicenciaNavigation?.Tipolicencia,
                    Fechanacimiento = ficha.Fechanacimiento,
                    Fechaingreso = ficha.Fechaingreso,
                    NacionalidadDescripcion = ficha.FknacionalidadNavigation?.Nacionalidad1,
                    EstadoCivilDescripcion = ficha.FkestadocivilNavigation?.Descripcion,
                    NivelEducacionDescripcion = ficha.FkniveleducacionNavigation?.Descripcion,
                    Telefono = ficha.Telefono,
                    Celular = ficha.Celular,
                    Correo = ficha.Correo,
                    CargoDescripcion = ficha.FkcargoNavigation?.Cargo1,
                    Domicilio = ficha.Domicilio,
                    Referencia = ficha.Referencia,
                    Cuotaf = ficha.Cuotaf,
                    Fkunidad = ficha.Fkunidad,
                    Fkdpuesto = ficha.Fkdpuesto
                },
                Unidad = unidad != null ? new UnidadDTO
                {
                    Unidad1 = unidad.Unidad1,
                    Placa = unidad.Placa,
                    Marca = unidad.Marca,
                    Modelo = unidad.Modelo,
                    Anio = unidad.Anio,
                    Color = unidad.Color,
                    Estado = unidad.Estado
                } : null,
                Duenopuesto = duenopuesto != null ? new DuenopuestoDTO
                {
                    Cedula = duenopuesto.Cedula,
                    Nombres = duenopuesto.Nombres,
                    Apellidos = duenopuesto.Apellidos
                } : null,
                Beneficiarios = beneficiarios
            };

            // 🔹 Generar PDF
            var pdfBytes = FichaPersonalPdfGenerator.GenerarPdf(exportDataCompleto);

            if (pdfBytes == null || pdfBytes.Length == 0)
                return StatusCode(StatusCodes.Status500InternalServerError, "No se pudo generar el PDF.");

            // 🔹 Retornar archivo
            return File(pdfBytes, "application/pdf", "FichaCompleta.pdf");
        }


        // 📌 Subir imagen del chofer
        [HttpPost("SubirImagenChofer")]
        public IActionResult SubirImagenChofer([FromForm] IFormFile? archivo, [FromForm] string cedula)
        {
            try
            {
                if (archivo != null && archivo.Length > 0)
                {
                    var extension = Path.GetExtension(archivo.FileName);
                    var nombreArchivo = $"{cedula}{extension}";
                    var rutaRemota = $"/documentos/{nombreArchivo}";

                    using (var client = new FtpClient("win8104.site4now.net", new NetworkCredential("lconcordiadoc", "Geo100100.")))
                    {
                        client.Connect();

                        // 🔄 Borrar archivos viejos con esa cédula
                        foreach (var item in client.GetListing("/documentos"))
                        {
                            if (item.Type == FtpObjectType.File && item.Name.StartsWith(cedula))
                                client.DeleteFile(item.FullName);
                        }

                        // Subir archivo
                        using (var stream = archivo.OpenReadStream())
                        {
                            client.UploadStream(stream, rutaRemota, FtpRemoteExists.Overwrite, true);
                        }
                    }

                    return Ok(new { mensaje = "Imagen subida correctamente", nombreArchivo });
                }

                return BadRequest("No se envió ningún archivo.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al subir la imagen: {ex.Message}");
            }

        }


        // 📌 Buscar imagen por cédula en FTP
        [HttpGet("BuscarImagenChofer/{cedula}")]
        public IActionResult BuscarImagenChofer(string cedula)
        {
            string host = "win8104.site4now.net";
            string user = "lconcordiadoc";
            string pass = "Geo100100.";
            string basePath = "/documentos";

            try
            {
                using var client = new FtpClient(host, new NetworkCredential(user, pass));
                client.Connect();

                if (!client.DirectoryExists(basePath))
                    return NotFound("Carpeta '/documentos' no encontrada en FTP.");

                // Buscar archivo que empiece con la cédula
                var archivo = client.GetListing(basePath)
                    .FirstOrDefault(f => f.Name.StartsWith(cedula, StringComparison.OrdinalIgnoreCase));

                if (archivo == null)
                    return NotFound("No se encontró ninguna imagen para esta cédula.");

                // ⚡ Aquí en vez de descargar devolvemos la URL pública
                string urlPublica = $"https://laconcordia.somee.com/documentos/{archivo.Name}";

                return Ok(urlPublica);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al buscar la imagen: {ex.Message}");
            }
        }




        // 📌 Eliminar imagen por cédula en FTP
        [HttpDelete("EliminarImagenChofer/{cedula}")]
        public IActionResult EliminarImagenChofer(string cedula)
        {
            string host = "win8104.site4now.net";
            string user = "lconcordiadoc";
            string pass = "Geo100100.";
            string basePath = "/documentos";

            var log = new List<string>();

            try
            {
                using var client = new FtpClient(host, new NetworkCredential(user, pass));
                client.Connect();
                log.Add("Conexión FTP establecida.");

                if (!client.DirectoryExists(basePath))
                    return Ok(new { mensaje = "No hay carpeta '/documentos' en FTP.", log });

                var archivos = client.GetListing(basePath)
                    .Where(f => f.Name.StartsWith(cedula))
                    .Select(f => f.FullName)
                    .ToList();

                if (!archivos.Any())
                    return Ok(new { mensaje = "No se encontró ninguna imagen para esta cédula.", log });

                foreach (var file in archivos)
                {
                    client.DeleteFile(file);
                    log.Add($"Archivo '{file}' eliminado.");
                }

                return Ok(new { mensaje = "Imagen eliminada correctamente del FTP", log });
            }
            catch (Exception ex)
            {
                log.Add($"Error al eliminar imagen: {ex.Message}");
                return StatusCode(500, new { mensaje = "Error al eliminar imagen", log });
            }
        }



        // 📌 Método de debug para verificar existencia y permisos de la imagen

        [HttpGet("ProbarFtp")]
        public IActionResult ProbarFtp()
        {
            string host = "win8104.site4now.net";
            string user = "lconcordiadoc";
            string pass = "Geo100100.";
            string basePath = "/documentos";

            var log = new List<string>();
            FtpClient client = null;

            try
            {
                // 🔹 Inicializar cliente FTP
                client = new FtpClient(host, new NetworkCredential(user, pass));
                log.Add("Cliente FTP creado.");

                // 🔹 Conectar al servidor FTP
                client.Connect();
                log.Add("Conexión al servidor FTP establecida.");

                if (!client.IsConnected)
                {
                    log.Add("Autenticación fallida.");
                    return StatusCode(403, new { mensaje = "Error de autenticación FTP", log });
                }
                log.Add("Autenticación exitosa.");

                // 🔹 Listar la raíz para depuración
                var raiz = client.GetListing("/");
                log.Add("Contenido raíz del FTP:");
                foreach (var item in raiz)
                {
                    log.Add($"- {item.Type}: {item.FullName}");
                }

                // 🔹 Verificar existencia de la carpeta y crear si no existe
                if (!client.DirectoryExists(basePath))
                {
                    log.Add($"Carpeta '{basePath}' no existe. Creándola...");
                    client.CreateDirectory(basePath);
                    log.Add($"Carpeta '{basePath}' creada correctamente.");
                }
                else
                {
                    log.Add($"Carpeta '{basePath}' encontrada.");
                }

                // 🔹 Listar archivos dentro de la carpeta
                var archivos = client.GetListing(basePath);
                log.Add($"Se encontraron {archivos.Length} archivos en '{basePath}'.");

                // 🔹 Retornar información
                return Ok(new
                {
                    mensaje = "Conexión FTP exitosa ✅",
                    cantidadArchivos = archivos.Length,
                    ejemplos = archivos.Take(5).Select(a => a.FullName),
                    log
                });
            }
            catch (FluentFTP.Exceptions.FtpCommandException ftpEx)
            {
                log.Add($"Error FTP: Código {ftpEx.CompletionCode}, Mensaje: {ftpEx.Message}");
                return StatusCode(403, new { mensaje = "Error de autenticación FTP", log });
            }
            catch (Exception ex)
            {
                log.Add($"Error inesperado: {ex.Message}");
                return StatusCode(500, new { mensaje = "Error inesperado", log });
            }
            finally
            {
                client?.Dispose();
            }
        }






    }
}
