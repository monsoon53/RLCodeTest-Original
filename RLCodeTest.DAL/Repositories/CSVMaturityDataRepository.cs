using RLCodeTest.DAL.Interfaces;
using RLCodeTest.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace RLCodeTest.DAL.Repositories
{
    public class CSVMaturityDataRepository : IMaturityDataRepository
    {
        string csvFilename;
        string csvFolder;
        string csvPath;

        public CSVMaturityDataRepository()
        {
            // Get CSV filename from config file
            csvFilename = ConfigurationManager.AppSettings["CSVFileName"];

            // Get CSV folder from config file
            csvFolder = ConfigurationManager.AppSettings["CSVFolder"];

            // Set path to CSV as csvFolder/csvFilename
            csvPath = AppDomain.CurrentDomain.BaseDirectory + Path.Combine(csvFolder, csvFilename);
        }

        public IEnumerable<MaturityDataBaseModel> GetMaturityData()
        {
            // Check CSV file exists
            if (!File.Exists(csvPath))
                throw new Exception(string.Format("Please ensure that {0} is in the {1} folder", csvFilename, csvFolder));

            List<MaturityDataBaseModel> maturityDataList = new List<MaturityDataBaseModel>();

            // Load CSV file into StreamReader
            using (var sr = new StreamReader(csvPath))
            {
                int count = 1;
                // Iterate through lines of CSV
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();

                    // Skip the first line containing the column headers
                    if (count > 1)
                    {
                        // Split CSV line into string array
                        var values = line.Split(',');

                        // Convert CSV values to MaturityDataBaseModel
                        MaturityDataBaseModel maturityData = ConvertCsvValuesToModel(values);

                        // Add to list
                        maturityDataList.Add(maturityData);
                    }
                    count++;
                }
            }
            
            return maturityDataList;
        }

        private MaturityDataBaseModel ConvertCsvValuesToModel(string[] values)
        {
            string policy_number = values[0];
            string policy_start_date = values[1];
            string premiums = values[2];
            string membership = values[3];
            string discretionary_bonus = values[4];
            string uplift_percentage = values[5];

            MaturityDataBaseModel maturityData = new MaturityDataBaseModel
            {
                PolicyNumber = policy_number,
                PolicyStartDate = DateTime.Parse(policy_start_date),
                Premiums = int.Parse(premiums),
                Membership = membership.ToUpper() == "Y" ? true : false,
                DiscretionaryBonus = decimal.Parse(discretionary_bonus),
                UpliftPercentage = decimal.Parse(uplift_percentage)
            };

            return maturityData;
        }
    }
}
