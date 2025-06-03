namespace LoanCalculator.Models;
public class AmortizationPayment
{
    public int PaymentNumber { get; set; }
    public DateTime PaymentDate { get; set; }
    public decimal MonthlyPayment { get; set; }
    public decimal PrincipalPayment { get; set; }
    public decimal InterestPayment { get; set; }
    public decimal InterestTaxPayment { get; set; }
    public decimal Balance { get; set; }
}
