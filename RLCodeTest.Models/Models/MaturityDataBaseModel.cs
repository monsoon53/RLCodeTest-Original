using System;

namespace RLCodeTest.Models
{
    public class MaturityDataBaseModel
    {
        public string PolicyNumber { get; set; }
        public DateTime PolicyStartDate { get; set; }
        public int Premiums { get; set; }
        public bool Membership { get; set; }
        public decimal DiscretionaryBonus { get; set; }
        public decimal UpliftPercentage { get; set; }
    }
}
