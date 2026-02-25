using System.ComponentModel.DataAnnotations;

namespace EmployeeDepartmentMVC.Models
{
    public class Department
    {
        public int DepartmentId { get; set; }

        [Required]
        public string DepartmentName { get; set; }

        public string Location { get; set; }

        public ICollection<Employee>? Employees { get; set; }
    }
}