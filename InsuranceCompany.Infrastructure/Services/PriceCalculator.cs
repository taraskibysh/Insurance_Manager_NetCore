using InsuranceCompany.Application.Common.Interfaces.Services;
using InsuranceCompany.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceCompany.Infrastructure.Services
{
    public class PriceCalculator : IPriceCalculator
    {
        async Task<decimal> IPriceCalculator.PriceCalc(TypeOfInsurance typeOfInsurance, MethodOfInsurance methodOfInsurance, InsuranceStatus status, decimal price)
        {
            decimal pay = price /10;
            if (status == InsuranceStatus.Active)
            {
                switch (typeOfInsurance)
                {
                    case TypeOfInsurance.CarInsurance:
                        pay *= 0.9m;
                        break;

                    case TypeOfInsurance.HealthInsurance:
                        pay *= 0.7m;
                        break;

                    case TypeOfInsurance.HouseInsurance:
                        pay *= 0.2m;
                        break;

                    case TypeOfInsurance.AnotherInsurance:
                        price *= 1;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(typeOfInsurance), typeOfInsurance, "Invalid insurance type.");
                }

                switch (methodOfInsurance)
                {

                    case MethodOfInsurance.FullInsurance:
                        pay *=  1;
                        break;

                    case MethodOfInsurance.HalfInsurance:
                        pay *= 0.5m;
                        break;

                    case MethodOfInsurance.QuarterInsurance:
                        pay *= 0.25m;
                        break;


                }

                return await Task.FromResult((decimal)pay);

            }
            else
            {
                return await Task.FromResult((decimal)0);
            }

        }
    }
}
