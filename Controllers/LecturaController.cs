using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgroMonitor.Data;
using AgroMonitor.Models;

namespace AgroMonitor.Controllers
{
    public class LecturaController : Controller
    {
        private readonly AgroMonitorDbContext _context;

        public LecturaController(AgroMonitorDbContext context)
        {
            _context = context;
        }

        // GET: Lectura/Index
        public async Task<IActionResult> Index(int? sensorId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Index", "Home");

            ViewBag.UserName = HttpContext.Session.GetString("UserName");

            // Obtener todos los sensores del usuario para el filtro
            var sensores = await _context.Sensores
                .Include(s => s.Finca)
                .Where(s => s.Finca != null && s.Finca.IdUsuario == userId)
                .ToListAsync();

            ViewBag.Sensores = sensores;
            ViewBag.SensorSeleccionado = sensorId;

            // Query base de lecturas
            var query = _context.LecturasSensor
                .Include(l => l.Sensor)
                    .ThenInclude(s => s.Finca)
                .Where(l => l.Sensor != null &&
                           l.Sensor.Finca != null &&
                           l.Sensor.Finca.IdUsuario == userId);

            // Filtrar por sensor si se especifica
            if (sensorId.HasValue)
            {
                query = query.Where(l => l.IdSensor == sensorId.Value);
            }

            // Obtener las últimas 50 lecturas
            var lecturas = await query
                .OrderByDescending(l => l.FechaHora)
                .Take(50)
                .ToListAsync();

            return View(lecturas);
        }

        // POST: Lectura/SimularLectura
        [HttpPost]
        public async Task<IActionResult> SimularLectura(int sensorId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Index", "Home");

            // Verificar que el sensor pertenece al usuario
            var sensor = await _context.Sensores
                .Include(s => s.Finca)
                .FirstOrDefaultAsync(s => s.IdSensor == sensorId &&
                                        s.Finca != null &&
                                        s.Finca.IdUsuario == userId);

            if (sensor == null)
            {
                TempData["Error"] = "Sensor no encontrado";
                return RedirectToAction(nameof(Index));
            }

            // Generar valor simulado según el tipo de sensor
            var random = new Random();
            decimal valor = 0;
            string unidad = "";

            switch (sensor.TipoSensor.ToLower())
            {
                case "humedad":
                    valor = (decimal)(random.NextDouble() * 60 + 20); // 20-80%
                    unidad = "%";
                    break;
                case "temperatura":
                    valor = (decimal)(random.NextDouble() * 20 + 15); // 15-35°C
                    unidad = "°C";
                    break;
                case "ph":
                    valor = (decimal)(random.NextDouble() * 3 + 5); // 5.0-8.0
                    unidad = "pH";
                    break;
                case "humedad_suelo":
                    valor = (decimal)(random.NextDouble() * 50 + 30); // 30-80%
                    unidad = "%";
                    break;
                case "luz":
                    valor = (decimal)(random.NextDouble() * 80000 + 10000); // 10k-90k lux
                    unidad = "lux";
                    break;
                default:
                    valor = (decimal)(random.NextDouble() * 100);
                    unidad = "unidad";
                    break;
            }

            // Crear la lectura
            var lectura = new LecturaSensor
            {
                IdSensor = sensorId,
                Valor = valor,
                Unidad = unidad,
                FechaHora = DateTime.Now,
                ProcesadoML = true
            };

            _context.LecturasSensor.Add(lectura);
            await _context.SaveChangesAsync();

            // Actualizar último check del sensor
            sensor.UltimoCheck = DateTime.Now;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Lectura simulada: {valor:0.##} {unidad}";
            return RedirectToAction(nameof(Index), new { sensorId = sensorId });
        }

        // GET: Lectura/ObtenerDatosGrafico
        [HttpGet]
        public async Task<IActionResult> ObtenerDatosGrafico(int sensorId, int horas = 24)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return Json(new { success = false, message = "No autorizado" });

            var fechaLimite = DateTime.Now.AddHours(-horas);

            var lecturas = await _context.LecturasSensor
                .Include(l => l.Sensor)
                    .ThenInclude(s => s.Finca)
                .Where(l => l.IdSensor == sensorId &&
                           l.FechaHora >= fechaLimite &&
                           l.Sensor != null &&
                           l.Sensor.Finca != null &&
                           l.Sensor.Finca.IdUsuario == userId)
                .OrderBy(l => l.FechaHora)
                .Select(l => new
                {
                    fecha = l.FechaHora.ToString("yyyy-MM-dd HH:mm:ss"),
                    valor = l.Valor,
                    unidad = l.Unidad
                })
                .ToListAsync();

            return Json(new
            {
                success = true,
                data = lecturas
            });
        }
    }
}