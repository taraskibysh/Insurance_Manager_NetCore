namespace InsuranceCompany.Contracts.Authentication;

public record AuthenticationResponce(
    Guid id,
    string FirstName, 
    string LastName,
    string Email, 
    string token);
