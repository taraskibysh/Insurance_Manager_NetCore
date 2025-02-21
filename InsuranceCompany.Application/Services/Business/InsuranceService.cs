using InsuranceCompany.Application.Common.Interfaces.Services;
using InsuranceCompany.Application.Common.Interfaces.Persistance;
using InsuranceCompany.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;

namespace InsuranceCompany.Application.Services.Business
{
    public class InsuranceService : IInsuranceService
    {
        private IPriceCalculator _priceCalc;
        private IInsuranceRepository _Repository;

        public InsuranceService(IPriceCalculator priceCalculator, IInsuranceRepository repository)
        {
            _priceCalc = priceCalculator;
            _Repository = repository;
        }

        public async Task<Insurance> AddInsurance(TypeOfInsurance typeOfInsurance, MethodOfInsurance methodOfInsurance, InsuranceStatus status, decimal Price, Guid userId)
        {
            if (Price < 0)
            {
                throw new ArgumentException("price cannot be negative");
            }
            decimal pay = await _priceCalc.PriceCalc(typeOfInsurance, methodOfInsurance, status, Price);

            Insurance insurance = new Insurance
            {
                Id = Guid.NewGuid(),
                Pay = pay,
                Price = Price,
                TypeOfInsurance = typeOfInsurance,
                Status = status,
                MethodOfInsurance = methodOfInsurance,
                UserId = userId,
            };

            await _Repository.AddInsurance(insurance);

            return insurance;
        }

        public async Task DeleteInsurance(Guid Id)
        {

            await _Repository.DeleteInsurance(Id);
        }

        public async Task<List<Insurance>> GetAllInsurances(Guid userId)
        {
            var list = await _Repository.GetAllInsurance(userId);

            if (list is not null)
            {
                return list;
            }
            else
            {
                throw new Exception("You dont have any insurance");
            }
        }

        public async Task<Insurance> GetInsurance(Guid id)
        {
            return await _Repository.GetInsurance(id);
        }

        public async Task UpdateInsurance(Guid id, TypeOfInsurance typeOfInsurance, MethodOfInsurance methodOfInsurance, InsuranceStatus status, Decimal price, Guid userId)
        {

             var pay = await _priceCalc.PriceCalc(typeOfInsurance,methodOfInsurance,status, price);
            Insurance insurance = new Insurance
            {
                Id = id,
                Pay = pay,
                Price = price,
                TypeOfInsurance = typeOfInsurance,
                MethodOfInsurance = methodOfInsurance,
                Status = status,
                UserId = userId,
            };

            await _Repository.UpdateInsurance(id, insurance);
        }

        public async Task<string> GetFullName(Guid UserId)
        {
            return await _Repository.GetUserFullName(UserId);
        }
    }
}
