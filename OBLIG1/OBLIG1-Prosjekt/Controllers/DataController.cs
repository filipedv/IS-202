using Microsoft.AspNetCore.Mvc;
using OBLIG1.Repository;

namespace OBLIG1.Controllers
{
    public class DataController : Controller
    {
        private readonly IDataRepository _iDataRepository;
        
        public DataController(IDataRepository idataRepository)
        {
            iDataRepository = idataRepository;
        }
        [HttpGet]
        public async Task<IActionResult> ShowData()
        {
            return View();
        }
        
        [HttpPost] //Det som sendes til brukeren?
        public async Task<IActionResult> ShowData()
        {
            return View();
        }
        
        //lager ViewModel under model for å skille detm an sender til brukeren?
    }
}