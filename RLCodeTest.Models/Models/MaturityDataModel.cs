namespace RLCodeTest.Models
{
    // Inherit from base model and add additional fields to use for Maturity Value calculation
    public class MaturityDataModel : MaturityDataBaseModel
    {
        public string PolicyType { get; set; }
        public decimal ManagementFeePercentage { get; set; }
        public bool DiscretionaryBonusEligibility{ get; set; }
        public decimal MaturityValue { get; set; }
    }
}
