using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeDepartmentMVC.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }

        [Required]
        [StringLength(50, MinimumLength =3,ErrorMessage ="Name must be between 3 and 50 characters")]
        public string Name { get; set; }

        public string Email { get; set; }

        public decimal Salary { get; set; }

        [ForeignKey("Department")]
        public int DepartmentId { get; set; }

        public Department? Department { get; set; }
    }
}