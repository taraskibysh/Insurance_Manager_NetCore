using AutoMapper;
using InsuranceCompany.Contracts.Authentication;
using InsuranceCompany.Application.Services.Authentication;

namespace InsuranceCompany.Contracts.Mapping.Authentication
{
    public class AuthResponseProfile : Profile
    {
        public AuthResponseProfile()
        {
            CreateMap<AuthenticationResult, AuthenticationResponce>();
        }
    }
}