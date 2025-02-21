using InsuranceCompany.Application.Common.Interfaces.Persistance;
using InsuranceCompany.Domain.Entities;
using InsuranceCompany.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InsuranceCompany.Infrastructure.Persistance
{
    public class InsuranceRepository : IInsuranceRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<InsuranceRepository> _logger;

        public InsuranceRepository(ApplicationDbContext dbContext, ILogger<InsuranceRepository> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task AddInsurance(Insurance insurance)
        {
            if (insurance == null)
            {
                throw new ArgumentNullException(nameof(insurance));
            }

            await _dbContext.insurances.AddAsync(insurance);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteInsurance(Guid id)
        {
            var insurance = await _dbContext.insurances.FirstOrDefaultAsync(i => i.Id == id);
            if (insurance != null)
            {
                _dbContext.insurances.Remove(insurance);
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation($"Insurance with ID {id} deleted successfully.");
            }
            else
            {
                _logger.LogWarning($"Insurance with ID {id} not found.");
                throw new KeyNotFoundException("Insurance not found");
            }
        }

        public async Task<List<Insurance>> GetAllInsurance(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("Invalid user ID");
            }

            return await _dbContext.insurances
                .AsNoTracking()
                .Where(i => i.UserId == userId)
                .ToListAsync();
        }

        public async Task<Insurance> GetInsurance(Guid id)
        {
            var insurance = await _dbContext.insurances.AsNoTracking()
                .FirstOrDefaultAsync(i => i.Id == id);

            return insurance ?? throw new KeyNotFoundException("Insurance not found");
        }

        public async Task UpdateInsurance(Guid id, Insurance insurance)
        {
            if (insurance == null)
            {
                throw new ArgumentNullException(nameof(insurance));
            }

            var existingInsurance = await _dbContext.insurances.FirstOrDefaultAsync(i => i.Id == id);
            if (existingInsurance != null)
            {
                _dbContext.Entry(existingInsurance).CurrentValues.SetValues(insurance);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException("Insurance doesn't exist");
            }
        }


        public async Task<string> GetUserFullName(Guid Id)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == Id);
            if (user == null)
            {
                throw new InvalidOperationException("user not found");
            }
            else
            {
                string FullName = user.FirstName + " " + user.LastName;
                return FullName;

            }
        }
    }

}
