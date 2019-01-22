# RLCodeTest-Original #

I’ve used the test as an opportunity to show how I would lay out the structure of the project as if it were the beginning of a real-world application. The solution is split out into layers which form the basis of a multi-tiered application – essentially the layers are UI, Service Layer, and Data Access Layer but there are a couple of additional simple projects to aid separation of concerns.

Although I’m aware that the brief states that it isn’t looking for a front end, I have incorporated an MVC web application as a UI just to give me a simple entry point; this is just a single controller that does a couple of calls to a service, it also renders a simple razor view to display the results as an additional visual element to complement the generated XML file. 

The application doesn’t require any user interaction, if you just run it from visual studio a web page will render with the results for each policy and the XML file will be generated each time the application runs, or if the page is refreshed.

In order to create loosely coupled code, and add some unit tests to the solution, I’ve used Ninject to implement dependency injection. 

Additional Nuget Packages
* RLCodeTest
  * Ninject.MVC5
* RLCodeTest.Tests
  * Ninject.Moq

  
Solution Setup

The solution is split into separate projects to keep things logically separated:

* RLCodeTest
* RLCodeTest.Services
* RLCodeTest.DAL
  
Additional projects 

* RLCodeTest.Models
* RLCodeTest.Library


Unit test project

* RLCodeTest.Tests

## RLCodeTest ##

Global.asax 
* Ninject dependency injection setup

MaturityDataController
* Inherits from the BaseController which implements a CustomErrorHandler attribute
  * This is for simplicity of error handling; any exceptions can be caught in one place and logged accordingly. A more granular approach to error handling may be more suitable in different scenarios using try/catch blocks in the various methods. 
* IMaturityDataService is injected into the constructor 
* Index ActionResult – no real work is done here; just a couple of calls to the service layer and sending a very simple view model to the Index view to display the results.

## RLCodeTest.Services ##
* Interfaces folder – containing IMaturityDataService
* Services folder - containing concrete implementation MaturityDataService

MaturityDataService – this is where most of the work is done by the application.

* IMaturityDataRepository is injected into the constructor
  * Implementing the CSVMaturityDataRepository in this way gives us the flexibility to switch it out for a different type of data repository if required, and/or use a Mock version of the repository in unit testing.
* GetMaturityDataAndCalculateValues() - public method that calls the data repository to get a collection of IEnumerable<MaturityDataBaseModel> before iterating through each item and carrying out the following for each:
  * CreateModelFromBaseModel() – creates model with additional properties; explained in more detail in the RLCodeTest.Models section.
  * CalculateMaturityDataValues() - explained next:
* CalculateMaturityDataValues() - public method that calls the following private methods to populate the additional properties and carry out the final Maturity Value calculation 
  * GetPolicyType() 
    * Derives the Policy Type from the first character of the Policy Number
  * GetManagementFeePercentage()
    * Derived from Policy Type to set a value of 3, 5, or 7%
  * GetDiscretionarytBonusEligibility()
    * Takes various criteria into account for each Policy Type and returns a boolean value
  * CalculateMaturityValue()
    * Final calculation using all the required properties/values
* GenerateXMLFile() public method which uses Linq to XML to  generate the xml file and save to the file system (folder and filename are defined in Web.config).


## RLCodeTest.DAL ##

* Interfaces folder – containing IMaturityDataRepository
* Repositories folder - containing concrete implementation CSVMaturityDataRepository

CSVMaturityDataRepository

* The repository contains one public method GetMaturityData() which returns a collection of IEnumerable<MaturityDataBaseModel>
* GetMaturityData() uses a StreamReader to get the data from the MaturityData.csv file (folder and filename are defined in Web.config). A while loop is then used to iterate through each line of the CSV:
  * Each iteration calls private method ConvertCsvValuesToModel() which converts each line into an instance of MaturityDataBaseModel
  * Each newly created model is returned to the GetMaturityData() public method and added to the collection.

## RLCodeTest.Models ##

* Models folder 
  * MaturityDataBaseModel – this represents the base data returned from the data repository.
  * MaturityDataModel – inherits from MaturityDataBaseModel and extends it to add the additional properties for calculating the final figure for Maturity Value: 
    * PolicyType
    * ManagementFeePercentage
    * DiscretionaryBonusEligibility
    * MaturityValue
* ViewModels folder
  * MaturityDataViewModel - simple view model containing results for MVC front end.
  
## RLCodeTest.Library ##

* Attributes folder - CustomHandleErrorAttribute - implemented in BaseContoller in MVC front end. This allows us to catch errors globally and handle in one place i.e. log them etc
* The Library project is where I would also put any shared code for the solution such as helper classes etc.

## RLCodeTest.Tests ##

MaturityDataServiceTest
* Uses a Setup method with a TestInitialize attribute to configure properties and test data which is shared between the unit tests:
  * Collection of hard coded test data - List<MaturityDataBaseModel>
  * Mock IMaturityDataRespository that uses functionality provided by the Moq Nuget package to return test data from the GetMaturityData() method.
* Instantiates an instance of IMaturityDataService by passing the Mock IMaturityDataRespository to the constructor.
* TestMethod’s to ensure the correct values are being calculated for each combination of Policy Type and Discretionary Bonus Eligibility.
