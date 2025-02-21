namespace InsuranceCompany.Infrastructure.Persistance;
using InsuranceCompany.Application.Common.Interfaces.Persistance;
using InsuranceCompany.Domain.Entities;
using Microsoft.EntityFrameworkCore.Diagnostics;
using InsuranceCompany.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _dbContext;


    public UserRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User?> GetUserByEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            return null;
        }

        if (_dbContext.Users == null)
        {
            return null; 
        }

        return await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    }



    public async Task Add(User user)
    {
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();
    }

}


