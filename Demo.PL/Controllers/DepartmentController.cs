using Demo.BLL.Interfaces;
using Demo.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace Demo.PL.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly IDepartmentRepository _repository;
        private readonly ILogger<DepartmentController> _logger;

        public DepartmentController(IDepartmentRepository repository, ILogger<DepartmentController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var departments = _repository.GetAll();
            //ViewData["Message"] = "Hello From View Data";
            //ViewBag.Message= "Hello From View Bag";//dynamic property and slower than view data and can using in casting 
            TempData.Keep();//save data when send from create to index
            return View(departments);
        }

        public IActionResult Details(int? id)
        {
            if (id is null)
                return RedirectToAction("Error", "Home");
            try
            {
                var department = _repository.GetById(id);

                if (department is null)
                    throw new Exception();

                return View(department);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Department department)
        {
            if (ModelState.IsValid)
            {
                _repository.Add(department);
                TempData["Message"] = "Department created successfully";
                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        public IActionResult Edit(int? id)
        {
            if (id is null)
                return RedirectToAction("Error", "Home");
            var department = _repository.GetById(id);

            if (department is null)
                return RedirectToAction("Error", "Home");

            return View(department);
        }

        [HttpPost]
        public IActionResult Edit(int id, Department department)
        {
            if(id != department.Id)
                return RedirectToAction("Error", "Home");

            if (ModelState.IsValid)
            {
                _repository.Update(department);
                return RedirectToAction(nameof(Index));
            }

            return View(department);
        }
        [Authorize(Roles ="Admin")]
        public IActionResult Delete(int id, Department department)
        {
            if (id != department.Id)
                return RedirectToAction("Error", "Home");

            try
            {
                _repository.Delete(department);
                return RedirectToAction(nameof(Index));
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction("Error", "Home");
            }
        }

    }
}
