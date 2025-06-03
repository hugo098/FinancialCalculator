using BuildingBlocks.GoalSeek;
using LoanCalculator;
using LoanCalculator.Models;
using Testing;

decimal startValue = 1000000m;
decimal endValue = 58000000000m;
//decimal endValue = 1000000m;

decimal increment = 1000000m;

int arraySize = (int)((endValue - startValue) / increment) + 1;
decimal[] decimalArray = new decimal[arraySize];

for (int i = 0; i < arraySize; i++)
{
    decimalArray[i] = startValue + (i * increment);
}


for (int i = 120; i <= 120; i++)
{
    foreach (decimal p in decimalArray)
    {
        Console.WriteLine($"Numero de cuotas {i} Valor prestado {p}");
        decimal principal = p;
        decimal annualInterestRate = 4;
        int numberOfPayments = i;

        FrenchAmortizationSystemAct365 frenchAmortizationSystemAct365 = new(
            loanRequestAmount: principal,
            nominalAnnualInterestRate: annualInterestRate,
            numberOfPayments: numberOfPayments,
            interestTaxRate: 10);

        LoanSummary firstGuess = frenchAmortizationSystemAct365.CalculateLoanPayments();

        //Console.WriteLine(firstGuess.First().MonthlyPayment);

        //GoalSeek goalSeek = new(frenchAmortizationSystemAct365);
        GoalSeekResult? goalSeekResult = GoalSeek.TrySeek(
            func: frenchAmortizationSystemAct365.Calculate,
            targetValue: 0,
            initialGuess: firstGuess.AmortizationPayments.First().MonthlyPayment
            //accuracyLevel: 0.0m,
            //maxIterations: 5000,
            //resultRoundOff: false
            );

        if (goalSeekResult is null)
            throw new Exception("Could not find the target value");

        //Console.WriteLine(JsonSerializer.Serialize(goalSeekResult));

        LoanSummary finalCalculation = frenchAmortizationSystemAct365.CalculateLoanPayments(goalSeekResult?.ClosestValue);

        //Console.WriteLine(JsonSerializer.Serialize(table));
        //Console.WriteLine("Payment Number\tMonthly Payment\tPrincipal Payment\tInterest Payment\tBalance\tPayment Date");
        /*foreach (var payment in finalCalculation.AmortizationPayments)
        {
            Console.WriteLine($"{payment.PaymentNumber}\t\t{payment.MonthlyPayment:C}\t\t{payment.PrincipalPayment:C}\t\t{payment.InterestPayment+payment.InterestTaxPayment:C}\t\t{payment.Balance:C}\t\t{payment.PaymentDate:MM/dd/yyyy}");
        }*/

        Console.WriteLine($"LoanRequestAmount: {finalCalculation.LoanRequestAmount:C}");
        Console.WriteLine($"Principal: {finalCalculation.Principal:C}");
        Console.WriteLine($"Principal: {finalCalculation.AmortizationPayments.Sum(x => x.PrincipalPayment):C}");
        Console.WriteLine($"Total interest: {finalCalculation.AmortizationPayments.Sum(x => x.InterestPayment):C}");
        Console.WriteLine($"Total interest tax: {finalCalculation.AmortizationPayments.Sum(x => x.InterestTaxPayment):C}");
        Console.WriteLine($"Loan total: {finalCalculation.MonthlyPayment * finalCalculation.NumberOfPayments:C}");
        Console.WriteLine($"Loan total: {finalCalculation.AmortizationPayments.Sum(x => x.MonthlyPayment):C}");
        Console.WriteLine($"Loan total: {(finalCalculation.AmortizationPayments.Sum(x => x.InterestPayment) +
            finalCalculation.AmortizationPayments.Sum(x => x.InterestTaxPayment) +
            finalCalculation.Principal):C}");


        TestConsoleTable
            .From(finalCalculation.AmortizationPayments)
            .Configure(o => o.NumberAlignment = Alignment.Right)
            .Write(Testing.Format.Minimal);

        //Console.WriteLine(JsonSerializer.Serialize(table));
        /*Console.WriteLine("Payment Number\tMonthly Payment\tPrincipal Payment\tInterest Payment\tBalance\tPayment Date");
        foreach (var payment in finalCalculation.AmortizationPayments)
        {
            Console.WriteLine($"{payment.PaymentNumber}\t\t{payment.MonthlyPayment:C}\t\t{payment.PrincipalPayment:C}\t\t{payment.InterestPayment+payment.InterestTaxPayment:C}\t\t{payment.Balance:C}\t\t{payment.PaymentDate:MM/dd/yyyy}");
        }*/

        AmortizationPayment lastPayment = finalCalculation.AmortizationPayments[finalCalculation.AmortizationPayments.Count - 1];
        Console.WriteLine($"\nLast Balance: {lastPayment.Balance:C}");
        Console.ReadLine();
    }
}