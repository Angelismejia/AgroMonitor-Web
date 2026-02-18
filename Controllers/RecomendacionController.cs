using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgroMonitor.Data;
using AgroMonitor.Models;

namespace AgroMonitor.Controllers
{
    public class RecomendacionController : Controller
    {
        private readonly AgroMonitorDbContext _context;

        public RecomendacionController(AgroMonitorDbContext context)
        {
            _context = context;
        }

        // GET: Recomendacion/Index
        public async Task<IActionResult> Index()
        {
            // 
            var usuarioId = HttpContext.Session.GetInt32("UserId");
            if (usuarioId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // ✅ Agregar ViewBag para el layout
            ViewBag.UserName = HttpContext.Session.GetString("UserName");

            var recomendaciones = await _context.RecomendacionesML
                .Include(r => r.Cultivo)
                    .ThenInclude(c => c!.Finca)
                .Where(r => r.Cultivo!.Finca!.IdUsuario == usuarioId)
                .OrderByDescending(r => r.FechaGenerada)
                .Take(50)
                .ToListAsync();

            return View(recomendaciones);
        }

        // GET: Recomendacion/GenerarAuto - Simulación ML
        public async Task<IActionResult> GenerarAuto()
        {
            
            var usuarioId = HttpContext.Session.GetInt32("UserId");
            if (usuarioId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var cultivos = await _context.Cultivos
                .Include(c => c.Finca)
                .Where(c => c.Finca!.IdUsuario == usuarioId && c.Estado == "activo")
                .ToListAsync();

            if (cultivos.Any())
            {
                var cultivo = cultivos.First();
                var random = new Random();

                var tiposRecomendacion = new[] { "riego", "fertilizante", "plaga" };
                var tipo = tiposRecomendacion[random.Next(tiposRecomendacion.Length)];

                var descripciones = new Dictionary<string, string>
                {
                    { "riego", "Aplicar riego ligero en las próximas 6 horas. Humedad del suelo está en nivel bajo." },
                    { "fertilizante", "Ajustar fertilización con NPK balanceado. Déficit de nitrógeno detectado." },
                    { "plaga", "Monitorear presencia de plagas comunes. Condiciones climáticas favorables para su aparición." }
                };

                var recomendacion = new RecomendacionML
                {
                    IdCultivo = cultivo.IdCultivo,
                    TipoRecomendacion = tipo,
                    Descripcion = descripciones[tipo],
                    Confianza = (decimal)(random.NextDouble() * 30 + 70),
                    FechaGenerada = DateTime.Now
                };

                _context.RecomendacionesML.Add(recomendacion);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Recomendación ML generada exitosamente";
            }
            else
            {
                TempData["Info"] = "No tienes cultivos activos para generar recomendaciones";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}