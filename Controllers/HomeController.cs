using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgroMonitor.Data;
using AgroMonitor.Models;
using System.Diagnostics;

namespace AgroMonitor.Controllers
{
    public class HomeController : Controller
    {
        private readonly AgroMonitorDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(AgroMonitorDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Home/Index - Login
        public IActionResult Index()
        {
            return View();
        }

        // POST: Home/Login
        [HttpPost]
        public async Task<IActionResult> Login(string correo, string contraseña)
        {
            if (string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(contraseña))
            {
                ViewBag.Error = "Por favor ingrese correo y contraseña";
                return View("Index");
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Correo == correo && u.ContraseñaHash == contraseña);

            if (usuario == null)
            {
                ViewBag.Error = "Credenciales incorrectas";
                return View("Index");
            }

            // Guardar sesión - NOMBRES ESTANDARIZADOS
            HttpContext.Session.SetInt32("UserId", usuario.IdUsuario);
            HttpContext.Session.SetString("UserName", usuario.Nombre);
            HttpContext.Session.SetString("UserRole", usuario.Rol);

            return RedirectToAction("Dashboard");
        }

        // GET: Home/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: Home/Register
        [HttpPost]
        public async Task<IActionResult> Register(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                usuario.CreadoEn = DateTime.Now;
                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(usuario);
        }

        // GET: Home/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Index");
            }

            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.TotalFincas = await _context.Fincas.CountAsync(f => f.IdUsuario == userId);
            ViewBag.TotalCultivos = await _context.Cultivos
                .CountAsync(c => c.Finca != null && c.Finca.IdUsuario == userId && c.Estado == "activo");
            ViewBag.TotalSensores = await _context.Sensores
                .CountAsync(s => s.Finca != null && s.Finca.IdUsuario == userId && s.Estado == "activo");
            ViewBag.TotalAlertas = await _context.Alertas
                .CountAsync(a => a.Finca != null && a.Finca.IdUsuario == userId && a.Estado == "pendiente");

            return View();
        }

        // GET: Home/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}