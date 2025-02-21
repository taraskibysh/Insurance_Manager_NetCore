using InsuranceCompany.Application.Common.Interfaces.Services;
using InsuranceCompany.Application.Services.Business;
using InsuranceCompany.Contracts.Model;  
using InsuranceCompany.Contracts.Models;
using InsuranceCompany.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;


namespace InsuranceCompany.Api.Controllers
{

    [Route("controller")]
    [Authorize]
    public class InsuranceController : ControllerBase
    {



        private readonly IInsuranceService _insuranceService;

        public InsuranceController(IInsuranceService insuranceService)
        {
            _insuranceService = insuranceService;
        }

        
        [HttpGet("TokenCheck")]
        public IActionResult TokenCheck()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
            if (userIdClaim == null)
            {
                return Unauthorized("User ID not found in token.");
            }

            var userId = Guid.Parse(userIdClaim.Value);
            return Ok(userId);

        }

        [HttpDelete("Id")]
        public async Task<IActionResult> Delete(Guid Id)
        {
            try
            {
                await _insuranceService.DeleteInsurance(Id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpGet("Id")]
        public async Task<IActionResult> GetInsurances(Guid Id)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
            if (userIdClaim == null)
            {
                return Unauthorized("User ID not found in token.");
            }

            var userId = Guid.Parse(userIdClaim.Value);
            try
            {
                var insurance = await _insuranceService.GetInsurance(Id);
                var fullName = await _insuranceService.GetFullName(insurance.UserId);

                var result = new InsuranceResponceModel
                {
                    Id = insurance.Id,

                    TypeOfInsurance = insurance.TypeOfInsurance,

                    Pay = insurance.Pay,
                    Price = insurance.Price,

                    Status = insurance.Status,

                    MethodOfInsurance = insurance.MethodOfInsurance,

                    Owner = fullName
                };
                return Ok(result);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
            


       
        [HttpGet("all")]
        public async Task<IActionResult> GetAllInsurances()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
            if (userIdClaim == null)
            {
                return Unauthorized("User ID not found in token.");
            }

            var userId = Guid.Parse(userIdClaim.Value);
            var insurances = await _insuranceService.GetAllInsurances(userId); 

            if (insurances != null)
            {
                return Ok(insurances);
            }
            else
            {
                return NotFound("No insurances found.");
            }
        }


        
        [HttpPost("create")]
        public async Task<IActionResult> AddInsurance([FromBody] InsuranceRequestModel model)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
            if (userIdClaim == null)
            {
                return Unauthorized("User ID not found in token.");
            }

            var userId = Guid.Parse(userIdClaim.Value);

            try
            {
                await _insuranceService.AddInsurance(model.Type, model.Method, model.Status, model.Price, userId);
                return Ok("Insurance added successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        

        [HttpPut("Id")]

        public async Task<IActionResult> UpdateInsurance([FromBody] InsuranceRequestModel model, Guid id)
        {

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
            if (userIdClaim == null)
            {
                return Unauthorized("User ID not found in token.");
            }

            var userId = Guid.Parse(userIdClaim.Value);

            try
            {
                await _insuranceService.UpdateInsurance(id,model.Type, model.Method, model.Status, model.Price, userId);
                return Ok("Insurance updated successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}
