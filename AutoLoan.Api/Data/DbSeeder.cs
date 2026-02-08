using AutoLoan.Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace AutoLoan.Api.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext db)
    {
        await db.Database.ExecuteSqlRawAsync("DELETE FROM financial_infos"); await db.Database.ExecuteSqlRawAsync("DELETE FROM addresses"); await db.Database.ExecuteSqlRawAsync("DELETE FROM vehicles"); await db.Database.ExecuteSqlRawAsync("DELETE FROM documents"); await db.Database.ExecuteSqlRawAsync("DELETE FROM application_notes"); await db.Database.ExecuteSqlRawAsync("DELETE FROM status_histories"); await db.Database.ExecuteSqlRawAsync("DELETE FROM applications"); await db.Database.ExecuteSqlRawAsync("DELETE FROM jwt_denylists"); await db.Database.ExecuteSqlRawAsync("DELETE FROM security_audit_logs"); await db.Database.ExecuteSqlRawAsync("DELETE FROM api_keys"); await db.Database.ExecuteSqlRawAsync("DELETE FROM users");

        var now = DateTime.UtcNow;
        User MakeUser(string email, string first, string last, string phone, UserRole role) => new()
        {
            Email = email, EncryptedPassword = BCrypt.Net.BCrypt.HashPassword("password123"),
            FirstName = first, LastName = last, Phone = phone, Role = role,
            Jti = Guid.NewGuid().ToString(), ConfirmedAt = now, CreatedAt = now, UpdatedAt = now
        };

        var tiffany = MakeUser("tiffany.chen@example.com", "Tiffany", "Chen", "714-555-1001", UserRole.Customer);
        var joseph = MakeUser("joseph.nguyen@example.com", "Joseph", "Nguyen", "714-555-1002", UserRole.Customer);
        var hai = MakeUser("hai.pham@example.com", "Hai", "Pham", "714-555-1003", UserRole.Customer);
        var vivian = MakeUser("vivian.nguyen@example.com", "Vivian", "Nguyen", "714-555-1004", UserRole.Customer);
        var jason = MakeUser("jason.hart@example.com", "Jason", "Hart", "714-555-1005", UserRole.Customer);
        var phong = MakeUser("ltphongssvn@gmail.com", "Phong", "Le", "714-555-9999", UserRole.Customer);
        var dijali = MakeUser("dijali@gmail.com", "Dijali", "Test", "714-555-9998", UserRole.Customer);
        var elena = MakeUser("elena.bychenkova@gmail.com", "Elena", "Bychenkova", "714-555-9997", UserRole.Customer);
        var shuveksha = MakeUser("tuladhar.shuveksha@gmail.com", "Shuveksha", "Tuladhar", "714-555-9996", UserRole.Customer);
        var vera = MakeUser("verafes@gmail.com", "Vera", "Fes", "714-555-9995", UserRole.Customer);
        var gab = MakeUser("gabhalley@gmail.com", "Gab", "Halley", "714-555-9994", UserRole.Customer);
        var officer = MakeUser("officer@example.com", "Thuy", "Nguyen", "714-555-2001", UserRole.LoanOfficer);
        var underwriter = MakeUser("underwriter@example.com", "Loi", "Luu", "714-555-3001", UserRole.Underwriter);

        db.Users.AddRange(tiffany, joseph, hai, vivian, jason, phong, dijali, elena, shuveksha, vera, gab, officer, underwriter);
        await db.SaveChangesAsync();

        // Tiffany - Draft
        var app1 = new Application { User = tiffany, Status = ApplicationStatus.Draft, CurrentStep = 2, Dob = new DateTime(1988,3,15, 0,0,0, DateTimeKind.Utc), SsnEncrypted = "123-45-6789", CreatedAt = now, UpdatedAt = now };
        db.Applications.Add(app1);
        await db.SaveChangesAsync();
        db.Addresses.Add(new Address { ApplicationId = app1.Id, AddressType = "residential", StreetAddress = "15464 Goldenwest St", City = "Westminster", State = "CA", ZipCode = "92683", YearsAtAddress = 5, MonthsAtAddress = 3, CreatedAt = now, UpdatedAt = now });
        db.Vehicles.Add(new Vehicle { ApplicationId = app1.Id, Make = "Toyota", Model = "Camry", Year = 2024, Condition = "new", Vin = "4T1BF1FK5GU123456", EstimatedValue = 32000m, CreatedAt = now, UpdatedAt = now });

        // Joseph - Submitted
        var app2 = new Application { User = joseph, Status = ApplicationStatus.Submitted, CurrentStep = 5, Dob = new DateTime(1985,7,22, 0,0,0, DateTimeKind.Utc), SsnEncrypted = "234-56-7890", LoanAmount = 30000m, DownPayment = 5000m, SubmittedAt = now, LoanTerm = 48, InterestRate = 5.9m, MonthlyPayment = 573.62m, CreatedAt = now, UpdatedAt = now };
        db.Applications.Add(app2);
        await db.SaveChangesAsync();
        db.Addresses.Add(new Address { ApplicationId = app2.Id, AddressType = "residential", StreetAddress = "14571 Magnolia St, Suite 105", City = "Westminster", State = "CA", ZipCode = "92683", YearsAtAddress = 4, MonthsAtAddress = 6, CreatedAt = now, UpdatedAt = now });
        db.Vehicles.Add(new Vehicle { ApplicationId = app2.Id, Make = "Honda", Model = "Accord", Year = 2024, Condition = "new", Vin = "1HGCV1F34PA012345", EstimatedValue = 34000m, CreatedAt = now, UpdatedAt = now });
        db.FinancialInfos.Add(new FinancialInfo { ApplicationId = app2.Id, IncomeType = "primary", EmployerName = "Kindred Hospital Westminster", JobTitle = "Pharmacist", EmploymentStatus = "full_time", YearsEmployed = 6, MonthsEmployed = 8, AnnualIncome = 125000m, MonthlyIncome = 10416.67m, MonthlyExpenses = 3500m, CreditScore = 780, CreatedAt = now, UpdatedAt = now });

        // Hai - Under Review
        var app3 = new Application { User = hai, Status = ApplicationStatus.UnderReview, CurrentStep = 5, Dob = new DateTime(1990,11,8, 0,0,0, DateTimeKind.Utc), SsnEncrypted = "345-67-8901", LoanAmount = 42000m, DownPayment = 6000m, SubmittedAt = now.AddDays(-2), LoanTerm = 60, InterestRate = 6.5m, MonthlyPayment = 702.35m, CreatedAt = now, UpdatedAt = now };
        db.Applications.Add(app3);
        await db.SaveChangesAsync();
        db.Addresses.Add(new Address { ApplicationId = app3.Id, AddressType = "residential", StreetAddress = "9600 Bolsa Ave", City = "Westminster", State = "CA", ZipCode = "92683", YearsAtAddress = 2, MonthsAtAddress = 9, CreatedAt = now, UpdatedAt = now });
        db.Vehicles.Add(new Vehicle { ApplicationId = app3.Id, Make = "Ford", Model = "F-150", Year = 2024, Condition = "new", Vin = "1FTFW1E50PFA98765", EstimatedValue = 48000m, CreatedAt = now, UpdatedAt = now });
        db.FinancialInfos.Add(new FinancialInfo { ApplicationId = app3.Id, IncomeType = "primary", EmployerName = "Westminster Police Department", JobTitle = "Police Officer I", EmploymentStatus = "full_time", YearsEmployed = 4, MonthsEmployed = 3, AnnualIncome = 102000m, MonthlyIncome = 8500m, MonthlyExpenses = 2800m, CreditScore = 745, CreatedAt = now, UpdatedAt = now });

        // Vivian - Under Review
        var app4 = new Application { User = vivian, Status = ApplicationStatus.UnderReview, CurrentStep = 5, Dob = new DateTime(1995,4,12, 0,0,0, DateTimeKind.Utc), SsnEncrypted = "456-78-9012", LoanAmount = 25000m, DownPayment = 3000m, SubmittedAt = now.AddDays(-1), LoanTerm = 60, InterestRate = 7.9m, MonthlyPayment = 449.18m, CreatedAt = now, UpdatedAt = now };
        db.Applications.Add(app4);
        await db.SaveChangesAsync();
        db.Addresses.Add(new Address { ApplicationId = app4.Id, AddressType = "residential", StreetAddress = "8419 Westminster Blvd", City = "Westminster", State = "CA", ZipCode = "92683", YearsAtAddress = 1, MonthsAtAddress = 6, CreatedAt = now, UpdatedAt = now });
        db.Vehicles.Add(new Vehicle { ApplicationId = app4.Id, Make = "Honda", Model = "Civic", Year = 2024, Condition = "new", Vin = "2HGFC2F59PH567890", EstimatedValue = 28000m, CreatedAt = now, UpdatedAt = now });
        db.FinancialInfos.Add(new FinancialInfo { ApplicationId = app4.Id, IncomeType = "primary", EmployerName = "Extended Care Hospital of Westminster", JobTitle = "Certified Nursing Assistant", EmploymentStatus = "full_time", YearsEmployed = 2, MonthsEmployed = 4, AnnualIncome = 48000m, MonthlyIncome = 4000m, MonthlyExpenses = 1800m, CreditScore = 680, CreatedAt = now, UpdatedAt = now });

        // Jason - Under Review
        var app5 = new Application { User = jason, Status = ApplicationStatus.UnderReview, CurrentStep = 5, Dob = new DateTime(1992,9,30, 0,0,0, DateTimeKind.Utc), SsnEncrypted = "567-89-0123", LoanAmount = 38000m, DownPayment = 4000m, SubmittedAt = now.AddDays(-3), LoanTerm = 60, InterestRate = 7.5m, MonthlyPayment = 693.21m, CreatedAt = now, UpdatedAt = now };
        db.Applications.Add(app5);
        await db.SaveChangesAsync();
        db.Addresses.Add(new Address { ApplicationId = app5.Id, AddressType = "residential", StreetAddress = "15464 Goldenwest St", City = "Westminster", State = "CA", ZipCode = "92683", YearsAtAddress = 3, MonthsAtAddress = 0, CreatedAt = now, UpdatedAt = now });
        db.Vehicles.Add(new Vehicle { ApplicationId = app5.Id, Make = "Tesla", Model = "Model 3", Year = 2024, Condition = "new", Vin = "5YJ3E1EA5PF234567", EstimatedValue = 42000m, CreatedAt = now, UpdatedAt = now });
        db.FinancialInfos.Add(new FinancialInfo { ApplicationId = app5.Id, IncomeType = "primary", EmployerName = "Westminster School District", JobTitle = "Substitute Teacher", EmploymentStatus = "part_time", YearsEmployed = 3, MonthsEmployed = 2, AnnualIncome = 42000m, MonthlyIncome = 3500m, MonthlyExpenses = 2000m, CreditScore = 710, CreatedAt = now, UpdatedAt = now });

        // Joseph - Pending Documents
        var app6 = new Application { User = joseph, Status = ApplicationStatus.PendingDocuments, CurrentStep = 5, Dob = new DateTime(1985,7,22, 0,0,0, DateTimeKind.Utc), SsnEncrypted = "234-56-7890", LoanAmount = 45000m, DownPayment = 7000m, SubmittedAt = now.AddDays(-5), LoanTerm = 48, InterestRate = 6.9m, MonthlyPayment = 913.18m, CreatedAt = now, UpdatedAt = now };
        db.Applications.Add(app6);
        await db.SaveChangesAsync();
        db.Addresses.Add(new Address { ApplicationId = app6.Id, AddressType = "residential", StreetAddress = "14571 Magnolia St, Suite 105", City = "Westminster", State = "CA", ZipCode = "92683", YearsAtAddress = 4, MonthsAtAddress = 6, CreatedAt = now, UpdatedAt = now });
        db.Vehicles.Add(new Vehicle { ApplicationId = app6.Id, Make = "BMW", Model = "X3", Year = 2023, Condition = "certified", Vin = "5UXTY5C05N9B12345", EstimatedValue = 52000m, CreatedAt = now, UpdatedAt = now });
        db.FinancialInfos.Add(new FinancialInfo { ApplicationId = app6.Id, IncomeType = "primary", EmployerName = "Kindred Hospital Westminster", JobTitle = "Pharmacist", EmploymentStatus = "full_time", YearsEmployed = 6, MonthsEmployed = 8, AnnualIncome = 125000m, MonthlyIncome = 10416.67m, MonthlyExpenses = 3500m, CreditScore = 780, CreatedAt = now, UpdatedAt = now });

        // Hai - Approved
        var app7 = new Application { User = hai, Status = ApplicationStatus.Approved, CurrentStep = 5, Dob = new DateTime(1990,11,8, 0,0,0, DateTimeKind.Utc), SsnEncrypted = "345-67-8901", LoanAmount = 32000m, DownPayment = 6000m, SubmittedAt = now.AddDays(-7), DecidedAt = now.AddDays(-4), LoanTerm = 60, InterestRate = 6.5m, MonthlyPayment = 508.44m, CreatedAt = now, UpdatedAt = now };
        db.Applications.Add(app7);
        await db.SaveChangesAsync();
        db.Addresses.Add(new Address { ApplicationId = app7.Id, AddressType = "residential", StreetAddress = "9600 Bolsa Ave", City = "Westminster", State = "CA", ZipCode = "92683", YearsAtAddress = 2, MonthsAtAddress = 9, CreatedAt = now, UpdatedAt = now });
        db.Vehicles.Add(new Vehicle { ApplicationId = app7.Id, Make = "Toyota", Model = "Tacoma", Year = 2024, Condition = "new", Vin = "3TMCZ5AN5PM123456", EstimatedValue = 38000m, CreatedAt = now, UpdatedAt = now });
        db.FinancialInfos.Add(new FinancialInfo { ApplicationId = app7.Id, IncomeType = "primary", EmployerName = "Westminster Police Department", JobTitle = "Police Officer I", EmploymentStatus = "full_time", YearsEmployed = 4, MonthsEmployed = 3, AnnualIncome = 102000m, MonthlyIncome = 8500m, MonthlyExpenses = 2800m, CreditScore = 745, CreatedAt = now, UpdatedAt = now });

        // Tiffany - Approved
        var app8 = new Application { User = tiffany, Status = ApplicationStatus.Approved, CurrentStep = 5, Dob = new DateTime(1988,3,15, 0,0,0, DateTimeKind.Utc), SsnEncrypted = "123-45-6789", LoanAmount = 45000m, DownPayment = 7000m, SubmittedAt = now.AddDays(-6), DecidedAt = now.AddDays(-3), LoanTerm = 48, InterestRate = 5.9m, MonthlyPayment = 871.25m, CreatedAt = now, UpdatedAt = now };
        db.Applications.Add(app8);
        await db.SaveChangesAsync();
        db.Addresses.Add(new Address { ApplicationId = app8.Id, AddressType = "residential", StreetAddress = "15464 Goldenwest St", City = "Westminster", State = "CA", ZipCode = "92683", YearsAtAddress = 5, MonthsAtAddress = 3, CreatedAt = now, UpdatedAt = now });
        db.Vehicles.Add(new Vehicle { ApplicationId = app8.Id, Make = "Lexus", Model = "RX 350", Year = 2024, Condition = "new", Vin = "2T2HZMDA5PC123456", EstimatedValue = 52000m, CreatedAt = now, UpdatedAt = now });
        db.FinancialInfos.Add(new FinancialInfo { ApplicationId = app8.Id, IncomeType = "primary", EmployerName = "Extended Care Hospital of Westminster", JobTitle = "Registered Nurse", EmploymentStatus = "full_time", YearsEmployed = 5, MonthsEmployed = 7, AnnualIncome = 95000m, MonthlyIncome = 7916.67m, MonthlyExpenses = 3200m, CreditScore = 760, CreatedAt = now, UpdatedAt = now });

        // Phong - Draft
        var app9 = new Application { User = phong, Status = ApplicationStatus.Draft, CurrentStep = 1, Dob = new DateTime(1990,1,15, 0,0,0, DateTimeKind.Utc), SsnEncrypted = "999-88-7777", CreatedAt = now, UpdatedAt = now };
        db.Applications.Add(app9);
        await db.SaveChangesAsync();
        db.Addresses.Add(new Address { ApplicationId = app9.Id, AddressType = "residential", StreetAddress = "10000 Bolsa Ave", City = "Westminster", State = "CA", ZipCode = "92683", YearsAtAddress = 2, MonthsAtAddress = 0, CreatedAt = now, UpdatedAt = now });
        db.Vehicles.Add(new Vehicle { ApplicationId = app9.Id, Make = "Honda", Model = "CR-V", Year = 2024, Condition = "new", Vin = "7FARW2H59PE000001", EstimatedValue = 35000m, CreatedAt = now, UpdatedAt = now });

        await db.SaveChangesAsync();
    }
}
