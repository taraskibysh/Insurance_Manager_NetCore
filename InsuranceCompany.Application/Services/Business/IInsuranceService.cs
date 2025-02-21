using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InsuranceCompany.Domain.Entities;

namespace InsuranceCompany.Application.Services.Business
{
    public interface IInsuranceService
    {
        public Task<List<Insurance>> GetAllInsurances(Guid UserId);

        public Task<Insurance> GetInsurance(Guid id);

        public Task<Insurance> AddInsurance(TypeOfInsurance typeOfInsurance, MethodOfInsurance methodOfInsurance, InsuranceStatus status, decimal price, Guid UserId);

        public Task DeleteInsurance(Guid Id);

        public Task UpdateInsurance(Guid Id, TypeOfInsurance typeOfInsurance, MethodOfInsurance methodOfInsurance, InsuranceStatus status, Decimal price, Guid UserId);

        public Task<string> GetFullName(Guid UserId);



    }
}


