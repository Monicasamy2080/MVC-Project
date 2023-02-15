using AutoMapper;
using Demo.BLL.Interfaces;
using Demo.DAL.Entities;
using Demo.PL.Helper;
using Demo.PL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Demo.PL.Controllers
{
    public class EmployeeController : Controller
    {
        private IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<DepartmentController> _logger;

        public EmployeeController(IUnitOfWork unitOfWork,IMapper mapper, ILogger<DepartmentController> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public IActionResult Index(string SearchValue="")
        {
            if (string.IsNullOrEmpty(SearchValue))
            {
                var employees = _unitOfWork.EmployeeRepository.GetAll();
                //foreach(var item in employees)//used to show departmentname in table of employe in view
                //{
                //    var department = _unitOfWork.DepartmentRepository.GetById(item.DepartmentId);
                //    ViewBag.DepartmentName=department.Name;
                //}
                //EmployeeViewModel employeeViewModels = new EmployeeViewModel()
                //{
                //    Id = employees.Id,
                //    Address = employees.Address,
                //    DateOfCreation = employees.DateOfCreation,
                //    Email = employees.Email,
                //    HireDate = employees.HireDate,
                //    IsActive = employees.IsActive,
                //    Name = employees.Name,
                //    Salary = employees.Salary,
                //};
                var mappedEmployees = _mapper.Map<IEnumerable<EmployeeViewModel>>(employees);
                return View(mappedEmployees);

            }
            else
            {
                var employees = _unitOfWork.EmployeeRepository.Search(SearchValue);
                var mappedEmployees = _mapper.Map<IEnumerable<EmployeeViewModel>>(employees);
                return View(mappedEmployees);
            }
          
        }
        public IActionResult Create()
        {
            ViewBag.Departments=_unitOfWork.DepartmentRepository.GetAll();
            return View();
        }
        [HttpPost]
        public IActionResult Create(EmployeeViewModel model)
        {
            if(ModelState.IsValid)
            {
                model.ImageName = DocumentSettings.UploadFile(model.Image, "imgs");
                var employee=_mapper.Map<Employee>(model);
                _unitOfWork.EmployeeRepository.Add(employee);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Departments = _unitOfWork.DepartmentRepository.GetAll();

            return View(model);
        }
        public IActionResult Delete(int id, EmployeeViewModel model)
        {
            if (id != model.Id)
                return RedirectToAction("Error", "Home");
            

            try
            {
                var employee = _mapper.Map<Employee>(model);
                _unitOfWork.EmployeeRepository.Delete(employee);
                return RedirectToAction(nameof(Index));
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction("Error", "Home");
            }
        }
        public IActionResult Details(int? id, EmployeeViewModel model)
        {
            if (id is null)
                return RedirectToAction("Error", "Home");
               var employee= _unitOfWork.EmployeeRepository.GetById(id);
            if (employee is null)
                return RedirectToAction("Error", "Home");
            model = _mapper.Map<EmployeeViewModel>(employee);
            ViewBag.Department = _unitOfWork.DepartmentRepository.GetById(employee.DepartmentId);
                //ViewBag.DepName = ViewBag.Department.Name;
                return View(model);

        }

        public  IActionResult Update(int?id,EmployeeViewModel model)
        {
            if (id is null)
                return RedirectToAction("Error", "Home");
            var employee = _unitOfWork.EmployeeRepository.GetById(id);
            if (employee is null)
                return RedirectToAction("Error", "Home");
            model = _mapper.Map<EmployeeViewModel>(employee);
            ViewBag.Departments = _unitOfWork.DepartmentRepository.GetAll();
            return View(model);
        }
        [HttpPost]
        public IActionResult Update(int  id, EmployeeViewModel model)
        {
            if (id != model.Id)
                return BadRequest();
            if (ModelState.IsValid)
            {
                model.ImageName = DocumentSettings.UploadFile(model.Image, "imgs");

                var employee = _mapper.Map<Employee>(model);
               _unitOfWork.EmployeeRepository.Update(employee);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Departments = _unitOfWork.DepartmentRepository.GetAll();

            return View(model);
        }
  
    }
}
