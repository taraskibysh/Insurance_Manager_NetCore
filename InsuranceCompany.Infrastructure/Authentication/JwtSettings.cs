using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceCompany.Infrastructure.Authentication
{
    public class JwtSettings
    {
        public const string SectionName = "JwtSettings";
        public string? Secret {  get; init; }

        public int ExpiryMinutes {  get; init; }

        public string? Issuer { get; init; }

        public string? Audience {  get; init; }

       
    }
}
