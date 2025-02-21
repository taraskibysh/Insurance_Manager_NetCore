using InsuranceCompany.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceCompany.Application.Common.Interfaces.Services
{
    public interface IPriceCalculator
    {

        public Task<decimal> PriceCalc(TypeOfInsurance typeOfInsurance, MethodOfInsurance methodOfInsurance, InsuranceStatus status, decimal price);

    }
}
