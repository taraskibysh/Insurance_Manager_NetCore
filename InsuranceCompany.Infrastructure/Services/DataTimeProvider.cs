namespace InsuranceCompany.Infrastructure.Services;
using InsuranceCompany.Application.Common.Interfaces.Services;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}