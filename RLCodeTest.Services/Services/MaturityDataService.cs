using RLCodeTest.DAL.Interfaces;
using RLCodeTest.Models;
using RLCodeTest.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace RLCodeTest.Services.Services
{
    // Implement IMaturityDataService interface
    public class MaturityDataService : IMaturityDataService
    {
        // Using dependency injection to inject IMaturityDataRepository - this provides the flexibility to either -
        // Swap to a different type of data repository, or
        // Use a Mock repository for unit testing
        IMaturityDataRepository _repo;
        public MaturityDataService(IMaturityDataRepository repo)
        {
            _repo = repo;
        }

        public IEnumerable<MaturityDataModel> GetMaturityDataAndCalculateValues()
        {
            // Get base Maturity Data from Data Repository 
            IEnumerable<MaturityDataBaseModel> maturityDataBase = _repo.GetMaturityData();

            // Create a list to use for MaturityDataModel's with additional derived fields/calculations
            List<MaturityDataModel> maturityData = new List<MaturityDataModel>();

            // Check that we have results
            if (maturityDataBase != null && maturityDataBase.Any())
            {
                // Create List from IEnumerable 
                List<MaturityDataBaseModel> maturityDataList = maturityDataBase.ToList();

                // Iterate through MaturityDataModel list
                foreach (MaturityDataBaseModel item in maturityDataBase)
                {
                    // Convert from base model to model containing additional properties
                    MaturityDataModel model = CreateModelFromBaseModel(item);

                    // Calculate values for additional properties for each item
                    model = CalculateMaturityDataValues(model);

                    // Add to maturityData list
                    maturityData.Add(model);
                }
            }

            return maturityData;
        }

        public MaturityDataModel CreateModelFromBaseModel(MaturityDataBaseModel item)
        {
            // Convert to model containing additional derived/calculated fields
            return new MaturityDataModel
            {
                PolicyNumber = item.PolicyNumber,
                PolicyStartDate = item.PolicyStartDate,
                Premiums = item.Premiums,
                Membership = item.Membership,
                DiscretionaryBonus = item.DiscretionaryBonus,
                UpliftPercentage = item.UpliftPercentage
            };
        }

        public MaturityDataModel CalculateMaturityDataValues(MaturityDataModel model)
        {
            // Get Policy Type, Management Fee, and Discretionary Bonus Eligibility - before proceeding to final Maturity Value Calculation
            model.PolicyType = GetPolicyType(model);
            model.ManagementFeePercentage = GetManagementFeePercentage(model);
            model.DiscretionaryBonusEligibility = GetDiscretionaryBonusEligibility(model);

            // Calculate Maturity Value
            model.MaturityValue = CalculateMaturityValue(model);

            return model;
        }

        private string GetPolicyType(MaturityDataModel model)
        {
            // Derive Policy Type from first character of Policy Number 
            if(model.PolicyNumber != null && model.PolicyNumber.Length > 1)
                return model.PolicyNumber.Substring(0,1);

            return null;
        }

        private decimal GetManagementFeePercentage(MaturityDataModel model)
        {
            decimal managementFeePercentage = 0;

            if (model.PolicyType != null)
            {
                // Derive Management Fee % from Policy Type 
                switch (model.PolicyType)
                {
                    case "A":
                        managementFeePercentage = 3;
                        break;
                    case "B":
                        managementFeePercentage = 5;
                        break;
                    case "C":
                        managementFeePercentage = 7;
                        break;
                    default:
                        managementFeePercentage = 0;
                        break;
                }
            }

            return managementFeePercentage;
        }

        private bool GetDiscretionaryBonusEligibility(MaturityDataModel model)
        {
            // Derive Discretionary Bonus Eligibility from Policy Type and Discretionary Bonus Criteria

            if (model.PolicyType != null && model.PolicyType == "A")
            {
                if (model.PolicyStartDate < DateTime.Parse("1990-01-01"))
                    return true;
            }
            else if (model.PolicyType != null && model.PolicyType == "B")
            {
                if (model.Membership == true)
                    return true;
            }
            else if ((model.PolicyType != null && model.PolicyType == "C"))
            {
                if (model.PolicyStartDate >= DateTime.Parse("1990-01-01") && model.Membership == true)
                    return true;
            }

            return false;
        }

        private decimal CalculateMaturityValue(MaturityDataModel model)
        {
            // Calculate Management Fee Value by using the number of Premiums and the Management Fee Percentage
            decimal managementFeeValue = model.Premiums * (model.ManagementFeePercentage / 100);

            // Calculate the Uplift Value by adding the Uplift Percentage to 1 - e.g. Uplift Percentage of 25% will equal an uplift value of 1.25
            decimal upliftValue = 1 + (model.UpliftPercentage / 100);

            // Calculate Discretionary Bonus Value using the Discretionary Bonus Eligibility
            decimal discretionaryBonusValue = model.DiscretionaryBonusEligibility == true ? model.DiscretionaryBonus : 0;

            // Carry out final Maturity Value calculation
            decimal maturityValue = ((model.Premiums - managementFeeValue) + discretionaryBonusValue) * upliftValue;
            
            // Round to 2 decimal places
            return Math.Round(maturityValue, 2);
        }

        public bool GenerateXMLFile(IEnumerable<MaturityDataModel> maturityData)
        {
            // Use Linq to XML to create XML file containing Policy Number and Maturity Value
            XDocument doc = new XDocument();

            // Create the root element for the document - MaturityDataResults
            XElement maturityDataResultsElement = new XElement("MaturityDataResults");

            // Iterate through Maturity Data items 
            foreach (var item in maturityData)
            {
                // Create Maturity Data element for each policy
                XElement maturityDataElement =
                    new XElement("MaturityData",
                        new XElement("PolicyNumber", item.PolicyNumber),
                        new XElement("MaturityValue", item.MaturityValue));

                // Add element to the root element
                maturityDataResultsElement.Add(maturityDataElement);
            }

            // Add root element to XML Document
            doc.Add(maturityDataResultsElement);

            string xmlFilename = ConfigurationManager.AppSettings["XMLFileName"];
            string xmlFolder = ConfigurationManager.AppSettings["xmlFolder"];

            // Define file save path
            string saveFilePath = AppDomain.CurrentDomain.BaseDirectory + Path.Combine(xmlFolder, xmlFilename);

            // Call save method to generate XML file and save to file system
            doc.Save(saveFilePath);

            return true;
        }
    }
}