// ============================================
// SensorController.cs - CORREGIDO
// ============================================
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AgroMonitor.Data;
using AgroMonitor.Models;

namespace AgroMonitor.Controllers
{
    public class SensorController : Controller
    {
        private readonly AgroMonitorDbContext _context;

        public SensorController(AgroMonitorDbContext context)
        {
            _context = context;
        }

        // GET: Sensor/Index
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.UserName = HttpContext.Session.GetString("UserName");

            var sensores = await _context.Sensores
                .Include(s => s.Finca)
                .Where(s => s.Finca != null && s.Finca.IdUsuario == userId)
                .ToListAsync();

            return View(sensores);
        }

        // GET: Sensor/Crear
        public async Task<IActionResult> Crear()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.UserName = HttpContext.Session.GetString("UserName");

            var fincas = await _context.Fincas
                .Where(f => f.IdUsuario == userId)
                .ToListAsync();

            ViewBag.Fincas = new SelectList(fincas, "IdFinca", "NombreFinca");
            return View();
        }

        // POST: Sensor/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Sensor sensor)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                sensor.InstaladoEn = DateTime.Now;
                sensor.UltimoCheck = DateTime.Now;
                _context.Sensores.Add(sensor);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Sensor creado exitosamente";
                return RedirectToAction(nameof(Index));
            }

            var fincas = await _context.Fincas
                .Where(f => f.IdUsuario == userId)
                .ToListAsync();
            ViewBag.Fincas = new SelectList(fincas, "IdFinca", "NombreFinca", sensor.IdFinca);

            return View(sensor);
        }

        // GET: Sensor/Editar/5
        public async Task<IActionResult> Editar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.UserName = HttpContext.Session.GetString("UserName");

            var sensor = await _context.Sensores
                .Include(s => s.Finca)
                .FirstOrDefaultAsync(s => s.IdSensor == id && s.Finca != null && s.Finca.IdUsuario == userId);

            if (sensor == null)
            {
                return NotFound();
            }

            var fincas = await _context.Fincas
                .Where(f => f.IdUsuario == userId)
                .ToListAsync();
            ViewBag.Fincas = new SelectList(fincas, "IdFinca", "NombreFinca", sensor.IdFinca);

            return View(sensor);
        }

        // POST: Sensor/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, Sensor sensor)
        {
            if (id != sensor.IdSensor)
            {
                return NotFound();
            }

            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sensor);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Sensor actualizado exitosamente";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SensorExists(sensor.IdSensor))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            var fincas = await _context.Fincas
                .Where(f => f.IdUsuario == userId)
                .ToListAsync();
            ViewBag.Fincas = new SelectList(fincas, "IdFinca", "NombreFinca", sensor.IdFinca);

            return View(sensor);
        }

        // GET: Sensor/Detalle/5
        public async Task<IActionResult> Detalle(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.UserName = HttpContext.Session.GetString("UserName");

            var sensor = await _context.Sensores
                .Include(s => s.Finca)
                .Include(s => s.Lecturas.OrderByDescending(l => l.FechaHora).Take(50))
                .FirstOrDefaultAsync(s => s.IdSensor == id && s.Finca != null && s.Finca.IdUsuario == userId);

            if (sensor == null)
            {
                return NotFound();
            }

            return View(sensor);
        }

        private bool SensorExists(int id)
        {
            return _context.Sensores.Any(e => e.IdSensor == id);
        }
    }
}
