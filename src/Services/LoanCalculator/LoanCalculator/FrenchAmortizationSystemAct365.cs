using BuildingBlocks.GoalSeek.Abstractions;
using LoanCalculator.Models;

namespace LoanCalculator;

public class FrenchAmortizationSystemAct365 : IGoalSeek
{
    public decimal LoanRequestAmount { get; set; }
    public decimal NominalAnnualInterestRate { get; set; }
    public int NumberOfPayments { get; set; }
    public decimal Principal { get; set; }
    public decimal AdministrativeExpenses { get; set; } = decimal.Zero;
    public decimal AdministrativeExpensesTax { get; set; } = decimal.Zero;
    public decimal LoanInsurance { get; set; } = decimal.Zero;
    public decimal LoanInsuranceTax { get; set; } = decimal.Zero;
    public decimal InterestTaxRate { get; set; } = decimal.Zero;
    public decimal? MonthlyPayment { get; set; } = null;

    public FrenchAmortizationSystemAct365(
        decimal loanRequestAmount,
        decimal nominalAnnualInterestRate,
        int numberOfPayments,
        decimal administrativeExpenses = 0,
        decimal administrativeExpensesTax = 0,
        decimal loanInsurance = 0,
        decimal loanInsuranceTax = 0,
        decimal interestTaxRate = 0,
        decimal? monthlyPayment = null)
    {
        LoanRequestAmount = loanRequestAmount;
        NominalAnnualInterestRate = nominalAnnualInterestRate;
        NumberOfPayments = numberOfPayments;
        AdministrativeExpenses = administrativeExpenses;
        AdministrativeExpensesTax = administrativeExpensesTax;
        LoanInsurance = loanInsurance;
        LoanInsuranceTax = loanInsuranceTax;
        InterestTaxRate = interestTaxRate;
        MonthlyPayment = monthlyPayment;
        Principal = LoanRequestAmount +
            AdministrativeExpenses +
            AdministrativeExpensesTax +
            LoanInsurance +
            LoanInsuranceTax;
    }

    public LoanSummary CalculateLoanPayments(decimal? monthlyPayment = null)
    {
        List<AmortizationPayment> amortizationTable = [];

        double monthlyInterestRate = (double)NominalAnnualInterestRate / 12 / 100;
        MonthlyPayment = monthlyPayment != null
            ? (decimal)monthlyPayment
            : Principal * (decimal)(monthlyInterestRate / (1 - Math.Pow(1 + monthlyInterestRate, -NumberOfPayments)));

        decimal balance = Principal;

        DateTime currentDate = DateTime.Now; // Start with the current date

        for (int i = 1; i <= NumberOfPayments; i++)
        {
            // Calculate the days in the current month
            int daysInMonth = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);

            // Calculate the monthly interest payment based on the days between payments
            decimal interestPayment =balance * NominalAnnualInterestRate / 100 * daysInMonth / 365;
            decimal interestTaxPayment = interestPayment * InterestTaxRate / 100;

            // Calculate the principal payment for the current month
            decimal principalPayment =(decimal)MonthlyPayment - (interestPayment + interestTaxPayment);

            // Update the balance
            //Console.WriteLine(principalPayment);
            balance -= principalPayment;

            amortizationTable.Add(new AmortizationPayment()
            {
                PaymentNumber = i,
                MonthlyPayment = (decimal)MonthlyPayment,
                PrincipalPayment = principalPayment,
                InterestPayment = interestPayment,
                InterestTaxPayment = interestTaxPayment,
                Balance = balance,
                PaymentDate = currentDate
            });

            // Move to the next month
            currentDate = currentDate.AddMonths(1);
        }

        return new LoanSummary()
        {
            LoanRequestAmount = LoanRequestAmount,
            Principal = Principal,
            MonthlyPayment = (decimal)MonthlyPayment,
            NumberOfPayments = NumberOfPayments,
            NominalAnnualInterestRate = NominalAnnualInterestRate,
            AdministrativeExpenses = AdministrativeExpenses,
            AdministrativeExpensesTax = AdministrativeExpensesTax,
            LoanInsurance = LoanInsurance,
            LoanInsuranceTax = LoanInsuranceTax,
            AmortizationPayments = amortizationTable
        };
    }



    public decimal Calculate(decimal input)
    {
        LoanSummary loanSummary = CalculateLoanPayments(input);

        decimal lastBalance = loanSummary.AmortizationPayments.Last().Balance;

        return lastBalance;
    }
}