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
                new Employee() { Id= 1, Name = "Amano Hina", Email = "amanohina@gmail.com", Department = Department.IT},
                new Employee() { Id= 2, Name = "Nishimiya", Email = "nishimiya@gmail.com", Department = Department.HR },
                new Employee() { Id= 3, Name = "Hodaka Morishima", Email = "hodakamorishima@gmail.com", Department = Department.Payroll },
                new Employee() { Id= 4, Name = "Test", Email = "test@gmail.com", Department = Department.IT }
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

        public Employee Add(Employee employee)
        {
            employee.Id = _employeeList.Max(e => e.Id) + 1;
            _employeeList.Add(employee);
            return employee;
        }

        public Employee Update(Employee employeeChanges)
        {
            Employee employee = _employeeList.FirstOrDefault(e => e.Id == employeeChanges.Id);
            if (employee != null)
            {
                employee.Name = employeeChanges.Name;
                employee.Email = employeeChanges.Email;
                employee.Department = employeeChanges.Department;
            }
            return employee;
        }

        public Employee Delete(int Id)
        {
            Employee employee = _employeeList.FirstOrDefault(e => e.Id == Id);
            if (employee!= null)
            {
                _employeeList.Remove(employee);
            }
            return employee;
        }
    }
}
