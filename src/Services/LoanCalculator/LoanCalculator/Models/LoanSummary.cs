namespace LoanCalculator.Models;
public class LoanSummary
{
    public decimal LoanRequestAmount { get; set; }
    public decimal NominalAnnualInterestRate { get; set; }
    public decimal EffectiveAnnualInterestRate { get; set; }
    public int NumberOfPayments { get; set; }
    public decimal Principal { get; set; }
    public decimal AdministrativeExpenses { get; set; } = decimal.Zero;
    public decimal AdministrativeExpensesTax { get; set; } = decimal.Zero;
    public decimal LoanInsurance { get; set; } = decimal.Zero;
    public decimal LoanInsuranceTax { get; set; } = decimal.Zero;
    public decimal InterestTaxRate { get; set; } = decimal.Zero;
    public decimal MonthlyPayment { get; set; }
    public required List<AmortizationPayment> AmortizationPayments { get; set;}
}