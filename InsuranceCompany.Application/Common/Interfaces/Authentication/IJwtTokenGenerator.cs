namespace InsuranceCompany.Application.Common.Interfaces.Authentication;

public interface IJwtTokenGenerator
{
    string GenerateToken(Guid userid, string firstName, string lastName );
}