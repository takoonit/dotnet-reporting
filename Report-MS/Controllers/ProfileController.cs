using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Redis.OM;
using Redis.OM.Searching;
using Report_MS.Models;
using Report_MS.Models.Repository;
using Report_MS.Repository;
using Report_MS.Utils;

namespace Report_MS.Controllers;

public class ProfileController : ControllerBase
{
    private readonly RedisCollection<Profile> _profiles;
    private readonly RedisConnectionProvider _provider;


    private readonly IMongoRepository<Report> _reportRepository;

    public ProfileController(RedisConnectionProvider provider,  IMongoRepository<Report> reportRepository)
    {
        _provider = provider;
        _profiles = (RedisCollection<Profile>)provider.RedisCollection<Profile>();

        _reportRepository = reportRepository;
    }

    [HttpPost("addProfile")]
    public async Task<IActionResult> AddProfile([FromBody] Profile profile)
    {
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
    public IList<Profile> FilterByName([FromQuery] string firstName, [FromQuery] string lastName)
    {
        return _profiles.Where(x => x.FirstName == firstName && x.LastName == lastName).ToList();
    }

    [HttpGet("status")]
    public IList<Profile> FilterByProfileStatus([FromQuery] string text)
    {
        return _profiles.Where(x => x.Status == text).ToList();
    }

    private string GetAgeGroup(int age)
    {
        if (age <= 20)
            return "Below 20";
        if (age <= 30)
            return "20-30";
        if (age <= 40)
            return "31-40";
        return "Above 40";
    }

    #region reporting

    [HttpPost("report/profiles")]
    public IActionResult GenerateSkillsDetailsReport([FromBody] ProfileFilter filter)
    {
        IList<Profile> profiles = new List<Profile>();

        //if (string.IsNullOrWhiteSpace(filter.Status))
        //{
        //    return Forbid("Status can't be empty");
        //}

        //profiles = _profiles.Where<Profile>(x => x.Status == filter.Status).ToList();
        profiles.Add(new Profile());

        var jsonProfiles = JsonConvert.SerializeObject(profiles, Formatting.Indented);
        var csvProfiles = Converter.ListToCsv<Profile, ProfileMap>(profiles);

        var report = new Report
        {
            Id = Guid.NewGuid().ToString(),
            Timestamp = DateTime.UtcNow,
            Data = jsonProfiles
        };

        _reportRepository.Create(report);

        return File(Encoding.UTF8.GetBytes(csvProfiles), "text/csv", "file.csv");
    }

    #endregion

    #region Update and Delete methods (Just in case)

    #region Update

    [HttpPatch("updateFirstName/{id}")]
    public IActionResult UpdateFirstName([FromRoute] string id, [FromBody] string newFirstName)
    {
        foreach (var profile in _profiles.Where(x => x.Id == id)) profile.FirstName = newFirstName;
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