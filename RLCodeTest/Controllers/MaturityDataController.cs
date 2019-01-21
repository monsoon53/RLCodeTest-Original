using RLCodeTest.Models;
using RLCodeTest.Models.ViewModels;
using RLCodeTest.Services.Interfaces;
using System.Collections.Generic;
using System.Web.Mvc;

namespace RLCodeTest.Controllers
{
    // Inherit from BaseController so we can implement CustomHandleError Attribute
    public class MaturityDataController : BaseController
    {
        // Use Dependency Injection to inject IMaturityDataService
        IMaturityDataService _serv;
        public MaturityDataController(IMaturityDataService serv)
        {
            _serv = serv;
        }

        public ActionResult Index()
        {
            // Call Service to get Maturity Data and Calculate Values
            IEnumerable<MaturityDataModel> maturityData = _serv.GetMaturityDataAndCalculateValues();

            // Call Service to Generate XML file
            bool xmlFileGenerated = _serv.GenerateXMLFile(maturityData);

            // Populate ViewModel so we can put the results on the page
            MaturityDataViewModel model = new MaturityDataViewModel { MaturityDataResults = maturityData, XMLFileGenerated = xmlFileGenerated }; 

            return View(model);
        }
    }
}