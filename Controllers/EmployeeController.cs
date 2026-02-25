using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EmployeeDepartmentMVC.Data;
using EmployeeDepartmentMVC.Models;

namespace EmployeeDepartmentMVC.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EmployeeController> _logger;

        public EmployeeController(ApplicationDbContext context, ILogger<EmployeeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Employee
        public async Task<IActionResult> Index(string searchString)
        {
            _logger.LogInformation("Employee Index page accessed.");

            var employees = from e in _context.Employees
                            select e;

            if (!string.IsNullOrEmpty(searchString))
            {
                _logger.LogInformation("Searching employees with keyword: {SearchKeyword}", searchString);

                employees = employees.Where(e =>
                    e.Name.ToLower().Contains(searchString.ToLower()));
            }

            return View(await employees.ToListAsync());
        }

        // GET: Employee/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Details requested with null ID.");
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Department)
                .FirstOrDefaultAsync(m => m.EmployeeId == id);

            if (employee == null)
            {
                _logger.LogWarning("Employee not found for Details. ID: {EmployeeId}", id);
                return NotFound();
            }

            _logger.LogInformation("Viewing details for Employee ID: {EmployeeId}", id);

            return View(employee);
        }

        // GET: Employee/Create
        public IActionResult Create()
        {
            _logger.LogInformation("Employee Create page opened.");

            ViewData["DepartmentId"] =
                new SelectList(_context.Departments, "DepartmentId", "DepartmentName");

            return View();
        }

        // POST: Employee/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee employee)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(employee);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("New Employee Created: {EmployeeName}", employee.Name);

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while creating employee.");
                    throw;
                }
            }

            _logger.LogWarning("Employee creation failed due to invalid model state.");

            ViewData["DepartmentId"] =
                new SelectList(_context.Departments, "DepartmentId", "DepartmentName", employee.DepartmentId);

            return View(employee);
        }

        // GET: Employee/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Edit requested with null ID.");
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                _logger.LogWarning("Employee not found for Edit. ID: {EmployeeId}", id);
                return NotFound();
            }

            _logger.LogInformation("Edit page opened for Employee ID: {EmployeeId}", id);

            ViewData["DepartmentId"] =
                new SelectList(_context.Departments, "DepartmentId", "DepartmentName", employee.DepartmentId);

            return View(employee);
        }

        // POST: Employee/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("EmployeeId,Name,Email,Salary,DepartmentId")] Employee employee)
        {
            if (id != employee.EmployeeId)
            {
                _logger.LogWarning("Edit mismatch ID. Route ID: {RouteId}, Employee ID: {EmployeeId}",
                    id, employee.EmployeeId);

                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Employee Updated Successfully. ID: {EmployeeId}",
                        employee.EmployeeId);
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!EmployeeExists(employee.EmployeeId))
                    {
                        _logger.LogError("Concurrency error. Employee not found. ID: {EmployeeId}",
                            employee.EmployeeId);

                        return NotFound();
                    }
                    else
                    {
                        _logger.LogError(ex, "Concurrency exception occurred while updating employee.");
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            _logger.LogWarning("Employee update failed due to invalid model state.");

            ViewData["DepartmentId"] =
                new SelectList(_context.Departments, "DepartmentId", "DepartmentName", employee.DepartmentId);

            return View(employee);
        }

        // GET: Employee/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Delete requested with null ID.");
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Department)
                .FirstOrDefaultAsync(m => m.EmployeeId == id);

            if (employee == null)
            {
                _logger.LogWarning("Employee not found for Delete. ID: {EmployeeId}", id);
                return NotFound();
            }

            _logger.LogInformation("Delete confirmation opened for Employee ID: {EmployeeId}", id);

            return View(employee);
        }

        // POST: Employee/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee != null)
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();

                _logger.LogWarning("Employee Deleted Successfully. ID: {EmployeeId}", id);
            }
            else
            {
                _logger.LogWarning("Attempted to delete non-existing employee. ID: {EmployeeId}", id);
            }

            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.EmployeeId == id);
        }
    }
}