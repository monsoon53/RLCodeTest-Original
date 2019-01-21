using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RLCodeTest.DAL.Interfaces;
using RLCodeTest.Models;
using RLCodeTest.Services.Interfaces;
using RLCodeTest.Services.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RLCodeTest.Tests.Services
{
    [TestClass]
    public class MaturityDataServiceTest
    {
        IMaturityDataService _serv;
        Mock<IMaturityDataRepository> _repoMock;
        List<MaturityDataBaseModel> maturityDataModelList;
      
        // Initialise properties to be shared between different tests
        [TestInitialize]
        public void Setup()
        {
            // Create hard coded list of Maturity Data
            maturityDataModelList = new List<MaturityDataBaseModel>();

            maturityDataModelList.Add(new MaturityDataBaseModel {
                PolicyNumber = "A100001", PolicyStartDate = DateTime.Parse("01/06/1986"), Premiums = 10000, Membership = true, DiscretionaryBonus = 1000, UpliftPercentage = 40 });
            maturityDataModelList.Add(new MaturityDataBaseModel {
                PolicyNumber = "A100002", PolicyStartDate = DateTime.Parse("01/01/1990"), Premiums = 12500, Membership = false, DiscretionaryBonus = 1350, UpliftPercentage = (decimal)37.5 });
            maturityDataModelList.Add(new MaturityDataBaseModel {
                PolicyNumber = "A100003", PolicyStartDate = DateTime.Parse("31/12/1989"), Premiums = 15250, Membership = false, DiscretionaryBonus = 1600, UpliftPercentage = 42 });
            maturityDataModelList.Add(new MaturityDataBaseModel {
                PolicyNumber = "B100001", PolicyStartDate = DateTime.Parse("01/01/1995"), Premiums = 12000, Membership = true, DiscretionaryBonus = 2000, UpliftPercentage = 41 });
            maturityDataModelList.Add(new MaturityDataBaseModel {
                PolicyNumber = "B100002", PolicyStartDate = DateTime.Parse("01/01/1970"), Premiums = 18000, Membership = false, DiscretionaryBonus = 3000, UpliftPercentage = 43 });
            maturityDataModelList.Add(new MaturityDataBaseModel {
                PolicyNumber = "B100003", PolicyStartDate = DateTime.Parse("20/07/1969"), Premiums = 20000, Membership = true, DiscretionaryBonus = 4000, UpliftPercentage = 45 });
            maturityDataModelList.Add(new MaturityDataBaseModel {
                PolicyNumber = "C100001", PolicyStartDate = DateTime.Parse("01/01/1992"), Premiums = 13000, Membership = false, DiscretionaryBonus = 1000, UpliftPercentage = 42 });
            maturityDataModelList.Add(new MaturityDataBaseModel {
                PolicyNumber = "C100002", PolicyStartDate = DateTime.Parse("31/12/1989"), Premiums = 15000, Membership = true, DiscretionaryBonus = 2000, UpliftPercentage = 44 });
            maturityDataModelList.Add(new MaturityDataBaseModel {
                PolicyNumber = "C100003", PolicyStartDate = DateTime.Parse("01/01/1990"), Premiums = 17000, Membership = true, DiscretionaryBonus = 3000, UpliftPercentage = 46 });

            // Create a Mock IMaturityDataRepository - GetMaturityData() will return hard coded maturityDataModelList
            _repoMock = new Mock<IMaturityDataRepository>();
            _repoMock.Setup(x => x.GetMaturityData()).Returns(maturityDataModelList);

            // Instantiate MaturityDataService and pass Mock IMaturityDataRepository to constructor 
            _serv = new MaturityDataService(_repoMock.Object);
        }

        // Unit Tests to ensure the correct values are being calculated for each combination of Policy Type and Discretionary Bonus Eligibility
        [TestMethod]
        public void PolicyTypeA_DiscretionaryBonusEligible_MaturityValue()
        {
            // Arrange
            MaturityDataModel maturityDataModel = _serv.CreateModelFromBaseModel(maturityDataModelList.FirstOrDefault(x => x.PolicyNumber == "A100001"));

            // Act
            maturityDataModel = _serv.CalculateMaturityDataValues(maturityDataModel);

            // Assert
            Assert.AreEqual((decimal)14980.00, maturityDataModel.MaturityValue);
        }

        [TestMethod]
        public void PolicyTypeA_DiscretionaryBonusNotEligible_MaturityValue()
        {
            // Arrange
            MaturityDataModel maturityDataModel = _serv.CreateModelFromBaseModel(maturityDataModelList.FirstOrDefault(x => x.PolicyNumber == "A100002"));

            // Act
            maturityDataModel = _serv.CalculateMaturityDataValues(maturityDataModel);

            // Assert
            Assert.AreEqual((decimal)16671.88, maturityDataModel.MaturityValue);
        }

        [TestMethod]
        public void PolicyTypeB_DiscretionaryBonusEligible_MaturityValue()
        {
            // Arrange
            MaturityDataModel maturityDataModel = _serv.CreateModelFromBaseModel(maturityDataModelList.FirstOrDefault(x => x.PolicyNumber == "B100001"));

            // Act
            maturityDataModel = _serv.CalculateMaturityDataValues(maturityDataModel);

            // Assert
            Assert.AreEqual((decimal)18894.00, maturityDataModel.MaturityValue);
        }

        [TestMethod]
        public void PolicyTypeB_DiscretionaryBonusNotEligible_MaturityValue()
        {
            // Arrange
            MaturityDataModel maturityDataModel = _serv.CreateModelFromBaseModel(maturityDataModelList.FirstOrDefault(x => x.PolicyNumber == "B100002"));

            // Act
            maturityDataModel = _serv.CalculateMaturityDataValues(maturityDataModel);

            // Assert
            Assert.AreEqual((decimal)24453.00, maturityDataModel.MaturityValue);
        }

        [TestMethod]
        public void PolicyTypeC_DiscretionaryBonusEligible_MaturityValue()
        {
            // Arrange
            MaturityDataModel maturityDataModel = _serv.CreateModelFromBaseModel(maturityDataModelList.FirstOrDefault(x => x.PolicyNumber == "C100003"));

            // Act
            maturityDataModel = _serv.CalculateMaturityDataValues(maturityDataModel);

            // Assert
            Assert.AreEqual((decimal)27462.60, maturityDataModel.MaturityValue);
        }

        [TestMethod]
        public void PolicyTypeC_DiscretionaryBonusNotEligible_MaturityValue()
        {
            // Arrange
            MaturityDataModel maturityDataModel = _serv.CreateModelFromBaseModel(maturityDataModelList.FirstOrDefault(x => x.PolicyNumber == "C100001"));

            // Act
            maturityDataModel = _serv.CalculateMaturityDataValues(maturityDataModel);

            // Assert
            Assert.AreEqual((decimal)17167.80, maturityDataModel.MaturityValue);
        }
    }
}
