namespace InsuranceCompany.Application.Services.Authentication;

using System.Reflection.Metadata;
using InsuranceCompany.Application.Common.Interfaces.Authentication;
using InsuranceCompany.Application.Common.Interfaces.Persistance;
using InsuranceCompany.Domain.Entities;


public class AuthenticationService : IAuthenticationService
{

    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IUserRepository _userRepository;

    public AuthenticationService(IJwtTokenGenerator jwtTokenGenerator, IUserRepository userRepository)
    {
        _jwtTokenGenerator = jwtTokenGenerator ?? throw new ArgumentNullException(nameof(jwtTokenGenerator));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    public async Task<AuthenticationResult> Register(string firstName, string lastName, string email, string password)
    {
        
        var user = await _userRepository.GetUserByEmail(email);
        if (user != null)
        {
            throw new Exception("User with given Email already exists.");
        }

   
        user = new User
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Password = password
        };

        
        await _userRepository.Add(user);

       
        var token = _jwtTokenGenerator.GenerateToken(user.Id, firstName, lastName);

        return new AuthenticationResult(
            user.Id,
            firstName,
            lastName,
            email,
            token
        );
    }


    public async Task<AuthenticationResult> Login(string email, string password)
    {
        
        var user = await _userRepository.GetUserByEmail(email);
        if (user == null)
        {
            throw new Exception("User with given email doesn't exist.");
        }

        
        if (user.Password != password)
        {
            throw new Exception("Invalid password.");
        }

        
        var token = _jwtTokenGenerator.GenerateToken(user.Id, user.FirstName, user.LastName);

        return new AuthenticationResult(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email,
            token
        );
    }

}