namespace InsuranceCompany.Application.Common.Interfaces.Services;

public interface IDateTimeProvider
{
	DateTime UtcNow { get; }

}