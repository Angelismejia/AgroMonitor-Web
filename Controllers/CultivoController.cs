using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AgroMonitor.Data;
using AgroMonitor.Models;

namespace AgroMonitor.Controllers
{
    public class CultivoController : Controller
    {
        private readonly AgroMonitorDbContext _context;

        public CultivoController(AgroMonitorDbContext context)
        {
            _context = context;
        }

        // GET: Cultivo/Index
        public async Task<IActionResult> Index()
        {
            var usuarioId = HttpContext.Session.GetInt32("UserId");
            if (usuarioId == null)
                return RedirectToAction("Index", "Home");

            // ✅ Enviar nombre del usuario
            ViewBag.UserName = HttpContext.Session.GetString("UserName");

            var cultivos = await _context.Cultivos
                .Include(c => c.Finca)
                .Where(c => c.Finca!.IdUsuario == usuarioId)
                .ToListAsync();

            return View(cultivos);
        }

        // GET: Cultivo/Crear
        public async Task<IActionResult> Crear()
        {
            var usuarioId = HttpContext.Session.GetInt32("UserId");
            if (usuarioId == null)
                return RedirectToAction("Index", "Home");

            // ✅ Mostrar nombre
            ViewBag.UserName = HttpContext.Session.GetString("UserName");

            var fincas = await _context.Fincas
                .Where(f => f.IdUsuario == usuarioId)
                .ToListAsync();

            ViewBag.Fincas = new SelectList(fincas, "IdFinca", "NombreFinca");
            return View();
        }

        // POST: Cultivo/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Cultivo cultivo)
        {
            var usuarioId = HttpContext.Session.GetInt32("UserId");
            if (usuarioId == null)
                return RedirectToAction("Index", "Home");

            if (ModelState.IsValid)
            {
                _context.Cultivos.Add(cultivo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var fincas = await _context.Fincas
                .Where(f => f.IdUsuario == usuarioId)
                .ToListAsync();

            ViewBag.Fincas = new SelectList(fincas, "IdFinca", "NombreFinca", cultivo.IdFinca);
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            return View(cultivo);
        }

        // GET: Cultivo/Editar/5
        public async Task<IActionResult> Editar(int? id)
        {
            if (id == null)
                return NotFound();

            var usuarioId = HttpContext.Session.GetInt32("UserId");
            if (usuarioId == null)
                return RedirectToAction("Index", "Home");

            // ✅ Mostrar nombre
            ViewBag.UserName = HttpContext.Session.GetString("UserName");

            var cultivo = await _context.Cultivos
                .Include(c => c.Finca)
                .FirstOrDefaultAsync(c => c.IdCultivo == id && c.Finca!.IdUsuario == usuarioId);

            if (cultivo == null)
                return NotFound();

            var fincas = await _context.Fincas
                .Where(f => f.IdUsuario == usuarioId)
                .ToListAsync();

            ViewBag.Fincas = new SelectList(fincas, "IdFinca", "NombreFinca", cultivo.IdFinca);

            return View(cultivo);
        }

        // POST: Cultivo/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, Cultivo cultivo)
        {
            if (id != cultivo.IdCultivo)
                return NotFound();

            var usuarioId = HttpContext.Session.GetInt32("UserId");
            if (usuarioId == null)
                return RedirectToAction("Index", "Home");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cultivo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Cultivos.Any(e => e.IdCultivo == cultivo.IdCultivo))
                        return NotFound();

                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            var fincas = await _context.Fincas
                .Where(f => f.IdUsuario == usuarioId)
                .ToListAsync();

            ViewBag.Fincas = new SelectList(fincas, "IdFinca", "NombreFinca", cultivo.IdFinca);
            ViewBag.UserName = HttpContext.Session.GetString("UserName");

            return View(cultivo);
        }

        // GET: Cultivo/Eliminar/5
        public async Task<IActionResult> Eliminar(int? id)
        {
            if (id == null)
                return NotFound();

            var usuarioId = HttpContext.Session.GetInt32("UserId");
            if (usuarioId == null)
                return RedirectToAction("Index", "Home");

            // ✅ Mostrar nombre
            ViewBag.UserName = HttpContext.Session.GetString("UserName");

            var cultivo = await _context.Cultivos
                .Include(c => c.Finca)
                .FirstOrDefaultAsync(c => c.IdCultivo == id && c.Finca!.IdUsuario == usuarioId);

            if (cultivo == null)
                return NotFound();

            return View(cultivo);
        }

        // POST: Cultivo/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            var cultivo = await _context.Cultivos.FindAsync(id);
            if (cultivo != null)
            {
                _context.Cultivos.Remove(cultivo);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
