using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InsuranceCompany.Domain.Entities;

namespace InsuranceCompany.Contracts.Model;

public class InsuranceRequestModel
{
    public decimal Price { get; set; }
    public MethodOfInsurance Method { get; set; }
    public InsuranceStatus Status { get; set; }
    public TypeOfInsurance Type { get; set; }

}
