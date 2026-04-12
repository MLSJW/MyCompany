using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using MyCompany.Domain.Entities;

namespace MyCompany.Controllers.Admin
{
    public partial class AdminController
    {
        public async Task<IActionResult> ServiceCategoriesEdit (int id)
        {
            //Если есть ID 
            ServiceCategory? entity = id == default 
                ? new ServiceCategory() 
                : await _dataManager.ServiceCategories.GetServiceCategoryByIdAsync(id);
            return View(entity);
        }
        [HttpPost]
        public async Task<IActionResult> ServiceCategoriesEdit(ServiceCategory entity)
        {
            if (!ModelState.IsValid)
                return View(entity);
            await _dataManager.ServiceCategories.SaveServiceCategoryAsync(entity);
            _logger.LogInformation($"Добавлена/обновлена категория услуги с ID:{entity.Id}");
            return RedirectToAction("Index");
        }
        [HttpPost]
        public  async Task<IActionResult>  ServiceCategoriesDelete(int id)
        {
            await _dataManager.ServiceCategories.DeleteServiceCategoryAsync(id);
            _logger.LogInformation($"Удалена категория услуги с ID:{id}");
            return RedirectToAction("Index");
        }
    }
}
