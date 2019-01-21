using System.Collections.Generic;

namespace RLCodeTest.Models.ViewModels
{
    public class MaturityDataViewModel
    {
        public IEnumerable<MaturityDataModel> MaturityDataResults { get; set; }
        public bool XMLFileGenerated { get; set; }
    }
}
