using System.Collections.Generic;
using RLCodeTest.Models;

namespace RLCodeTest.Services.Interfaces
{
    public interface IMaturityDataService
    {
        IEnumerable<MaturityDataModel> GetMaturityDataAndCalculateValues();
        bool GenerateXMLFile(IEnumerable<MaturityDataModel> maturityData);
        MaturityDataModel CreateModelFromBaseModel(MaturityDataBaseModel item);
        MaturityDataModel CalculateMaturityDataValues(MaturityDataModel maturityData);
    }
}