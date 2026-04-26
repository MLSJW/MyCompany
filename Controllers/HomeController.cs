using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyCompany.Domain;
using MyCompany.Domain.Entities;
using MyCompany.Infrastructure;
using MyCompany.Models;

namespace MyCompany.Controllers
{
    public class HomeController : Controller
    {
        // GET: HomeController
        private readonly DataManager _dataManager;
        public HomeController (DataManager dataManager)
        {
            _dataManager = dataManager;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Service> list = await _dataManager.Services.GetServicesAsync();
            IEnumerable<ServiceDTO> listDTO = HelperDTO.TransformServices(list).Take(3).ToList();
            return View(listDTO);
        }

        //public ActionResult Index()
        //{
        //    return View();
        //}
        public IActionResult Contacts()
        { 
            return View();
        }
    }
}
