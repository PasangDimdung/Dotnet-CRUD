using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    /* Reason for using IEmployeeRepository interface is: Without it we cannot use Dependency Injection as a result 
    our application will be extremely difficult to change and maintain. Unit Testing is also becomes difficult. */
    public class MockEmployeeRepository : IEmployeeRepository
    {
        private List<Employee> _employeeList;
        public MockEmployeeRepository()
        {
            _employeeList = new List<Employee>()
            {
                new Employee() { Id= 1, Name = "Hari", Email = "hari@gmail.com", Department = "IT"},
                new Employee() { Id= 2, Name = "Ram", Email = "ram@gmail.com", Department = "HR" },
                new Employee() { Id= 3, Name = "Shyam", Email = "shyam@gmail.com", Department = "IT" },
                new Employee() { Id= 4, Name = "Test", Email = "test@gmail.com", Department = "IT" }
            };
        }

        public IEnumerable<Employee> GetAllEmployee()
        {
            return _employeeList;
        }

        public Employee GetEmployee(int Id)
        {
            return _employeeList.FirstOrDefault(e => e.Id == Id);
        }
    }
}
