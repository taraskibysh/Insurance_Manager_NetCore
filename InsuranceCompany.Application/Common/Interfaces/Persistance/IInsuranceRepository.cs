using InsuranceCompany.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceCompany.Application.Common.Interfaces.Persistance
{
    public interface IInsuranceRepository
    {
        Task AddInsurance(Insurance insurance);
        Task<Insurance> GetInsurance(Guid id);
        Task<List<Insurance>> GetAllInsurance(Guid userId);
        Task DeleteInsurance(Guid id);
        Task UpdateInsurance(Guid id, Insurance insurance);

        Task<string> GetUserFullName(Guid userId);
    }

}


