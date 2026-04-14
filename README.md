### Test Description

In the 'PaymentService.cs' file you will find a method for making a payment. At a high level the steps for making a payment are:

- Lookup the account the payment is being made from
- Check the account is in a valid state to make the payment
- Deduct the payment amount from the account's balance and update the account in the database

What we’d like you to do is refactor the code with the following things in mind:  

- Adherence to SOLID principals
- Testability  
- Readability

We’d also like you to add some unit tests to the ClearBank.DeveloperTest.Tests project to show how you would test the code that you’ve produced. The only specific ‘rules’ are:  

- The solution should build.
- The tests should all pass.
- You should not change the method signature of the MakePayment method.

You are free to use any frameworks/NuGet packages that you see fit.  

You should plan to spend around 1 to 3 hours to complete the exercise.

---

## Approach

I used Claude Code to assist with this task. I told it to identify refactoring opportunities, improve the code structure, and add tests using TUnit. I reviewed each change and made my own additions along the way.

### What I Changed and Why

#### Introduced IAccountDataStore interface

The two data store classes `AccountDataStore` and `BackupAccountDataStore` had no shared interface. I added `IAccountDataStore` so `PaymentService` depends on an abstraction rather than a concrete implementation, making it easier to swap implementations and test in isolation.

#### Refactored `PaymentService`

The original had all payment validation crammed into one big switch statement, and was creating its own data store inside the method. Claude extracted each scheme's rules into its own validator class and changed the data store and validators to be passed in via the constructor. So it makes the code cleaner and more maintainable.

#### Added unit tests

Used TUnit and Moq to write tests covering all three payment schemes — valid accounts, invalid accounts, insufficient balance, and checking the balance gets deducted correctly.

I had not used TUnit before but it came up as a newer, more modern .NET testing framework so I wanted to try it. It is said to be better than xUnit and NUnit. I have not felt significant improvement yet but it is built async-first unlike the other test suites, has faster test discovery, and has more expressive assertions so makes it simpler.

---

### What I Would Do With More Time

- **Add a Program.cs** — there is no entry point where dependencies are wired up.
- **BackupAccountDataStore not implemented** - Implements IAccountDataStore but is never used anywhere.
- **Improve test coverage** — add tests for insufficient balance edge cases, boundary conditions. For e.g DebtorAccountNumber is never set in tests so GetAccount is always called with null.
- **Credit the creditor account** — currently only the debtor is debited. The creditor account is never credited.
- **Input validation** — guard against null requests, negative amounts, and invalid account number formats.
- **Exception handling** — no exceptions are caught anywhere, so any failure in the data store or validation will crash the whole payment flow.
- **Logging and error handling** — add structured logging and handle data store failures gracefully.
- **Tests for AccountDataStore and BackupAccountDataStore** — these classes have no unit tests of their own yet.
