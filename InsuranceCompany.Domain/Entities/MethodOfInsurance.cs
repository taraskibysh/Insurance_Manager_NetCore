using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace InsuranceCompany.Domain.Entities
{
    public enum MethodOfInsurance
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        FullInsurance,
        HalfInsurance,
        QuarterInsurance
    }
}
