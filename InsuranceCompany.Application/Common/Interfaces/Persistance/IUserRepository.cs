namespace InsuranceCompany.Application.Common.Interfaces.Persistance;
using InsuranceCompany.Domain.Entities;

public interface IUserRepository
{
    Task<User?> GetUserByEmail(string email); 
    Task Add(User user);



}