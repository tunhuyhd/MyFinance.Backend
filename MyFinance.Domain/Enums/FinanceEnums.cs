namespace MyFinance.Domain.Enums;

public enum TransactionType
{
    Income = 1,
    Expense = 2,
    Transfer = 3
}

public enum AccountType
{
    Checking = 1,
    Savings = 2,
    Cash = 3,
    CreditCard = 4,
    Investment = 5
}

public enum CategoryType
{
    Income = 1,
    Expense = 2
}

public enum SavingsGoalStatus
{
    Active = 1,
    Completed = 2,
    Cancelled = 3
}
