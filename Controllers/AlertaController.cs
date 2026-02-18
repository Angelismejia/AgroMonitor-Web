using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgroMonitor.Data;
using AgroMonitor.Models;

namespace AgroMonitor.Controllers
{
    public class AlertaController : Controller
    {
        private readonly AgroMonitorDbContext _context;

        public AlertaController(AgroMonitorDbContext context)
        {
            _context = context;
        }

        // GET: Alerta/Index
        public async Task<IActionResult> Index(string? prioridad, string? estado)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Index", "Home");

            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.PrioridadFiltro = prioridad;
            ViewBag.EstadoFiltro = estado;

            var query = _context.Alertas
                .Include(a => a.Finca)
                .Include(a => a.Cultivo)
                .Where(a => a.Finca != null && a.Finca.IdUsuario == userId);

            if (!string.IsNullOrEmpty(prioridad))
            {
                query = query.Where(a => a.NivelPrioridad == prioridad);
            }

            if (!string.IsNullOrEmpty(estado))
            {
                query = query.Where(a => a.Estado == estado);
            }

            var alertas = await query
                .OrderByDescending(a => a.FechaGenerada)
                .ToListAsync();

            return View(alertas);
        }

        // POST: Alerta/MarcarAtendida
        [HttpPost]
        public async Task<IActionResult> MarcarAtendida(int id)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId == null)
                    return Json(new { success = false, message = "No autorizado" });

                var alerta = await _context.Alertas
                    .Include(a => a.Finca)
                    .FirstOrDefaultAsync(a => a.IdAlerta == id &&
                                            a.Finca != null &&
                                            a.Finca.IdUsuario == userId);

                if (alerta == null)
                    return Json(new { success = false, message = "Alerta no encontrada" });

                alerta.Estado = "atendida";
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Alerta marcada como atendida" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // POST: Alerta/GenerarAlertas
        [HttpPost]
        public async Task<IActionResult> GenerarAlertas()
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId == null)
                    return Json(new { success = false, message = "No autorizado" });

                int alertasGeneradas = 0;

                var sensores = await _context.Sensores
                    .Include(s => s.Finca)
                    .Where(s => s.Finca != null &&
                               s.Finca.IdUsuario == userId &&
                               s.Estado == "activo")
                    .ToListAsync();

                if (!sensores.Any())
                {
                    return Json(new
                    {
                        success = true,
                        message = "No tienes sensores activos para generar alertas",
                        count = 0
                    });
                }

                foreach (var sensor in sensores)
                {
                    var ultimaLectura = await _context.LecturasSensor
                        .Where(l => l.IdSensor == sensor.IdSensor)
                        .OrderByDescending(l => l.FechaHora)
                        .FirstOrDefaultAsync();

                    if (ultimaLectura == null)
                        continue;

                    bool generarAlerta = false;
                    string tipoAlerta = "";
                    string descripcion = "";
                    string prioridad = "media";

                    // Evaluar condiciones según tipo de sensor
                    var tipoSensor = sensor.TipoSensor?.ToLower() ?? "";

                    if (tipoSensor.Contains("humedad") && ultimaLectura.Valor < 30)
                    {
                        generarAlerta = true;
                        tipoAlerta = "humedad_baja";
                        descripcion = $"Humedad crítica en {sensor.Finca.NombreFinca}: {ultimaLectura.Valor:0.##}%";
                        prioridad = "alta";
                    }
                    else if (tipoSensor.Contains("temperatura") && ultimaLectura.Valor > 35)
                    {
                        generarAlerta = true;
                        tipoAlerta = "temperatura_critica";
                        descripcion = $"Temperatura elevada en {sensor.Finca.NombreFinca}: {ultimaLectura.Valor:0.##}°C";
                        prioridad = "alta";
                    }
                    else if (tipoSensor.Contains("ph") && (ultimaLectura.Valor < 5.5m || ultimaLectura.Valor > 7.5m))
                    {
                        generarAlerta = true;
                        tipoAlerta = "ph_inadecuado";
                        descripcion = $"pH fuera de rango en {sensor.Finca.NombreFinca}: {ultimaLectura.Valor:0.##}";
                        prioridad = "media";
                    }
                    else if (tipoSensor.Contains("humedad") && ultimaLectura.Valor < 40)
                    {
                        generarAlerta = true;
                        tipoAlerta = "humedad_baja";
                        descripcion = $"Humedad baja en {sensor.Finca.NombreFinca}: {ultimaLectura.Valor:0.##}%";
                        prioridad = "media";
                    }

                    if (generarAlerta)
                    {
                        // Verificar si ya existe una alerta similar reciente
                        var alertaExistente = await _context.Alertas
                            .Where(a => a.IdFinca == sensor.IdFinca &&
                                        a.TipoAlerta == tipoAlerta &&
                                        a.Estado == "pendiente" &&
                                        a.FechaGenerada >= DateTime.Now.AddHours(-24))
                            .FirstOrDefaultAsync();

                        if (alertaExistente == null)
                        {
                            // Buscar cultivo activo en la finca
                            var cultivo = await _context.Cultivos
                                .Where(c => c.IdFinca == sensor.IdFinca && c.Estado == "activo")
                                .FirstOrDefaultAsync();

                            if (cultivo != null)
                            {
                                var alerta = new Alerta
                                {
                                    IdCultivo = cultivo.IdCultivo,
                                    IdFinca = sensor.IdFinca,
                                    TipoAlerta = tipoAlerta,
                                    Descripcion = descripcion,
                                    NivelPrioridad = prioridad,
                                    FechaGenerada = DateTime.Now,
                                    Estado = "pendiente"
                                };

                                _context.Alertas.Add(alerta);
                                alertasGeneradas++;
                            }
                        }
                    }
                }

                await _context.SaveChangesAsync();

                if (alertasGeneradas == 0)
                {
                    return Json(new
                    {
                        success = true,
                        message = "No se generaron nuevas alertas. Todas las lecturas están en rangos normales o ya existen alertas recientes.",
                        count = 0
                    });
                }

                return Json(new
                {
                    success = true,
                    message = $"{alertasGeneradas} alerta(s) generada(s) exitosamente",
                    count = alertasGeneradas
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = $"Error al generar alertas: {ex.Message}"
                });
            }
        }

        // POST: Alerta/Eliminar
        [HttpPost]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId == null)
                    return Json(new { success = false, message = "No autorizado" });

                var alerta = await _context.Alertas
                    .Include(a => a.Finca)
                    .FirstOrDefaultAsync(a => a.IdAlerta == id &&
                                            a.Finca != null &&
                                            a.Finca.IdUsuario == userId);

                if (alerta == null)
                    return Json(new { success = false, message = "Alerta no encontrada" });

                _context.Alertas.Remove(alerta);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Alerta eliminada exitosamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error al eliminar: {ex.Message}" });
            }
        }

        // GET: Alerta/Estadisticas (Opcional - para Dashboard)
        [HttpGet]
        public async Task<IActionResult> ObtenerEstadisticas()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return Json(new { success = false, message = "No autorizado" });

            var alertas = await _context.Alertas
                .Include(a => a.Finca)
                .Where(a => a.Finca != null && a.Finca.IdUsuario == userId)
                .ToListAsync();

            var stats = new
            {
                totalPendientes = alertas.Count(a => a.Estado == "pendiente"),
                alta = alertas.Count(a => a.NivelPrioridad == "alta" && a.Estado == "pendiente"),
                media = alertas.Count(a => a.NivelPrioridad == "media" && a.Estado == "pendiente"),
                baja = alertas.Count(a => a.NivelPrioridad == "baja" && a.Estado == "pendiente"),
                atendidas = alertas.Count(a => a.Estado == "atendida")
            };

            return Json(new { success = true, data = stats });
        }
    }
}