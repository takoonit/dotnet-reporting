using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Redis.OM;
using Redis.OM.Searching;
using Report_MS.Common;
using Report_MS.Dto;
using Report_MS.Models;
using Report_MS.Repository;
using Report_MS.Utils;

namespace Report_MS.Controllers;

public class ProfileController : ControllerBase
{
    private readonly IRedisCollection<Profile> _profiles;
    private readonly RedisConnectionProvider _provider;
    private readonly IMongoRepository<Report> _reportRepository;

    public ProfileController(RedisConnectionProvider provider, IMongoRepository<Report> reportRepository)
    {
        _provider = provider;
        _profiles = (RedisCollection<Profile>)provider.RedisCollection<Profile>();
        _reportRepository = reportRepository;
    }

    [HttpPost("addProfile")]
    public async Task<IActionResult> AddProfile([FromBody] ProfileDto profileDto)
    {
        if (string.IsNullOrWhiteSpace(profileDto.FirstName)) return Forbid("First name can't be empty");

        var profile = new Profile
        {
            Id = Guid.NewGuid().ToString(),
            FirstName = profileDto.FirstName,
            LastName = profileDto.LastName,
            Status = ProfileStatus.Active
        };

        await _profiles.InsertAsync(profile);
        return CreatedAtAction(nameof(GetProfile), new { id = profile.Id }, profile);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Profile>> GetProfile([FromRoute] string id)
    {
        var profile = await _profiles.FindByIdAsync(id);
        if (profile == null) return NotFound();

        return profile;
    }

    [HttpGet("filterName")]
    public IList<Profile> FilterByName([FromQuery] string firstName)
    {
        return _profiles.Where(x => x.FirstName == firstName).ToList();
    }

    [HttpGet("status")]
    public IList<Profile> FilterByProfileStatus([FromQuery] string status)
    {
        return _profiles.Where(c => c.Status == status).ToList();
    }

    #region reporting

    [HttpPost("report/profiles/{fileType}")]
    public async Task<IActionResult> GenerateSkillsDetailsReport([FromRoute] string fileType)
    {
        var profiles = await _profiles.Where(p => p.Status == ProfileStatus.Active).ToListAsync();
        var data = fileType switch
        {
            "json" => JsonConvert.SerializeObject(profiles, Formatting.Indented),
            "csv" => Converter.ListToCsv<Profile, ProfileMap>(profiles),
            _ => null
        };

        var contentType = fileType switch
        {
            "json" => "application/json",
            "csv" => "text/csv",
            _ => ""
        };

        if (data != null)
        {
            var fileName = $"{DateTime.Now.ToShortDateString()}.{fileType}";
            var report = new Report(data);

            await _reportRepository.Create(report).WaitAsync(new CancellationToken());

            return File(Encoding.UTF8.GetBytes(data), contentType, fileName);
        }

        return BadRequest("File type not supported, Please use json or csv");
    }

    #endregion

    #region Update and Delete methods (Just in case)

    #region Update

    [HttpPatch("updateStatus/{id}")]
    public IActionResult UpdateFirstName([FromRoute] string id, [FromBody] string newStatus)
    {
        foreach (var profile in _profiles.Where(x => x.Id == id)) profile.Status = newStatus;

        _profiles.Save();
        return Accepted();
    }

    #endregion

    #region Delete

    [HttpDelete("{id}")]
    public IActionResult DeletePerson([FromRoute] string id)
    {
        _provider.Connection.Unlink($"Profile:{id}");
        return NoContent();
    }

    #endregion

    #endregion
}