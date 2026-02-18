using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgroMonitor.Data;
using AgroMonitor.Models;

namespace AgroMonitor.Controllers
{
    public class FincaController : Controller
    {
        private readonly AgroMonitorDbContext _context;

        public FincaController(AgroMonitorDbContext context)
        {
            _context = context;
        }

        // GET: Finca/Index
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Index", "Home");

            ViewBag.UserName = HttpContext.Session.GetString("UserName");

            var fincas = await _context.Fincas
                .Include(f => f.Usuario)
                .Where(f => f.IdUsuario == userId)
                .ToListAsync();

            return View(fincas);
        }

        // GET: Finca/Crear
        public IActionResult Crear()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Index", "Home");

            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            return View(new Finca()); // Pasar un modelo vacío
        }

        // POST: Finca/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Finca finca)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Index", "Home");

            // Remover validaciones que no necesitamos
            ModelState.Remove("Usuario");
            ModelState.Remove("IdUsuario");
            ModelState.Remove("CreadoEn");

            if (!ModelState.IsValid)
            {
                ViewBag.UserName = HttpContext.Session.GetString("UserName");
                return View(finca);
            }

            // Asignar datos del usuario
            finca.IdUsuario = userId.Value;
            finca.CreadoEn = DateTime.Now;

            try
            {
                _context.Fincas.Add(finca);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Finca creada exitosamente";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al guardar: {ex.Message}");
                ViewBag.UserName = HttpContext.Session.GetString("UserName");
                return View(finca);
            }
        }

        // GET: Finca/Editar/5
        public async Task<IActionResult> Editar(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Index", "Home");

            ViewBag.UserName = HttpContext.Session.GetString("UserName");

            var finca = await _context.Fincas
                .FirstOrDefaultAsync(f => f.IdFinca == id && f.IdUsuario == userId);

            if (finca == null)
                return NotFound();

            return View(finca);
        }

        // POST: Finca/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, Finca finca)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Index", "Home");

            if (id != finca.IdFinca)
                return NotFound();

            // Remover validaciones que no necesitamos
            ModelState.Remove("Usuario");

            if (!ModelState.IsValid)
            {
                ViewBag.UserName = HttpContext.Session.GetString("UserName");
                return View(finca);
            }

            var fincaExistente = await _context.Fincas
                .FirstOrDefaultAsync(f => f.IdFinca == id && f.IdUsuario == userId);

            if (fincaExistente == null)
                return NotFound();

            try
            {
                fincaExistente.NombreFinca = finca.NombreFinca;
                fincaExistente.UbicacionText = finca.UbicacionText;
                fincaExistente.Latitud = finca.Latitud;
                fincaExistente.Longitud = finca.Longitud;
                fincaExistente.AreaTareas = finca.AreaTareas;

                await _context.SaveChangesAsync();

                TempData["Success"] = "Finca actualizada exitosamente";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al actualizar: {ex.Message}");
                ViewBag.UserName = HttpContext.Session.GetString("UserName");
                return View(finca);
            }
        }

        // POST: Finca/Eliminar/5
        [HttpPost]
        public async Task<IActionResult> Eliminar(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return Json(new { success = false, message = "No autorizado" });

            var finca = await _context.Fincas
                .FirstOrDefaultAsync(f => f.IdFinca == id && f.IdUsuario == userId);

            if (finca == null)
                return Json(new { success = false, message = "Finca no encontrada" });

            try
            {
                _context.Fincas.Remove(finca);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Finca eliminada" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }
    }
}