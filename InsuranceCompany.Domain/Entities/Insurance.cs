using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceCompany.Domain.Entities
{
  


    public class Insurance
    {
        public Guid Id { get; set; }
        public TypeOfInsurance TypeOfInsurance { get; set; }

        public decimal Pay {  get; set; }
        public decimal Price { get; set; }

        public InsuranceStatus Status { get; set; }

        public MethodOfInsurance MethodOfInsurance { get; set; }

        public Guid UserId { get; set; }  

        public User? User { get; set; }
    }

}
