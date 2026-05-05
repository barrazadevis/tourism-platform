using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourismPlatform.Core.DTOs.Company;
using TourismPlatform.Data.Interfaces;

namespace TourismPlatform.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CompanyController : BaseController
    {
        private readonly ICompanyService _companyService;

        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCompany([FromBody] CreateCompanyRequestDto request)
        {
            try
            {
                var result = await _companyService.CreateCompanyAsync(request);
                return CreatedAtAction(nameof(GetCompany), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating company: {ex.Message}");
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetCompany(Guid id)
        {
            try
            {
                var result = await _companyService.GetCompanyByIdAsync(id);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving company: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCompanies()
        {
            try
            {
                var result = await _companyService.GetAllCompaniesAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving companies: {ex.Message}");
            }
        }

        [HttpGet("by-application/{applicationId:int}")]
        public async Task<IActionResult> GetCompaniesByApplication(int applicationId)
        {
            try
            {
                var result = await _companyService.GetCompaniesByApplicationAsync(applicationId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving companies: {ex.Message}");
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateCompany(Guid id, [FromBody] UpdateCompanyRequestDto request)
        {
            try
            {
                var result = await _companyService.UpdateCompanyAsync(id, request);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error updating company: {ex.Message}");
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteCompany(Guid id)
        {
            try
            {
                var result = await _companyService.DeleteCompanyAsync(id);
                if (!result)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error deleting company: {ex.Message}");
            }
        }

        [HttpPost("{id:guid}/users")]
        public async Task<IActionResult> AssignUserToCompany(Guid id, [FromBody] AssignUserToCompanyRequestDto request)
        {
            try
            {
                var result = await _companyService.AssignUserToCompanyAsync(id, request.UserId);
                if (!result)
                    return BadRequest("Unable to assign user to company");

                return Ok(new { message = "User assigned to company successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error assigning user: {ex.Message}");
            }
        }

        [HttpDelete("{companyId:guid}/users/{userId:int}")]
        public async Task<IActionResult> RemoveUserFromCompany(Guid companyId, int userId)
        {
            try
            {
                var result = await _companyService.RemoveUserFromCompanyAsync(companyId, userId);
                if (!result)
                    return NotFound("User not found in company");

                return Ok(new { message = "User removed from company successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error removing user: {ex.Message}");
            }
        }

        [HttpGet("{id:guid}/users")]
        public async Task<IActionResult> GetCompanyUsers(Guid id)
        {
            try
            {
                var result = await _companyService.GetCompanyUsersAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving company users: {ex.Message}");
            }
        }

        [HttpPatch("{id:guid}/deactivate")]
        public async Task<IActionResult> DeactivateCompany(Guid id)
        {
            try
            {
                var result = await _companyService.DeactivateCompanyAsync(id);
                if (!result)
                    return NotFound();

                return Ok(new { message = "Company deactivated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error deactivating company: {ex.Message}");
            }
        }

        [HttpPatch("{id:guid}/activate")]
        public async Task<IActionResult> ActivateCompany(Guid id)
        {
            try
            {
                var result = await _companyService.ActivateCompanyAsync(id);
                if (!result)
                    return NotFound();

                return Ok(new { message = "Company activated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error activating company: {ex.Message}");
            }
        }
    }
}