using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Redis.OM;
using Redis.OM.Searching;
using Report_MS.Models;
using Report_MS.Repository;
using Xunit;
using Profile = Report_MS.Models.Profile;

namespace Report_MS.Controllers
{
    public class ProfileController : ControllerBase
    {
        private readonly RedisCollection<Profile> _profiles;
        private readonly RedisConnectionProvider _provider;


        private readonly IMongoRepository<Report> _reportRepository;

        public ProfileController(RedisConnectionProvider provider)
        {
            _provider = provider;
            _profiles = (RedisCollection<Profile>)provider.RedisCollection<Profile>();
        }

        [HttpPost("addProfile")]
        public async Task<Profile> AddProfile([FromBody] Profile profile)
        {
            
            await _profiles.InsertAsync(profile);
            return profile;
        }

        [HttpGet("filterAge")]
        public IList<Profile> FilterByAge([FromQuery] int minAge, [FromQuery] int maxAge)
        {
            return _profiles.Where<Profile>(x => x.Age >= minAge && x.Age <= maxAge).ToList<Profile>();
        }

        [HttpGet("filterGeo")]
        public IList<Profile> FilterByGeo([FromQuery] double lat, [FromQuery] double lon, [FromQuery] double radius, [FromQuery] string unit)
        {
            return _profiles.GeoFilter(x => x.Address!.Location, lon, lat, radius, Enum.Parse<GeoLocDistanceUnit>(unit)).ToList<Profile>();
        }

        [HttpGet("filterName")]
        public IList<Profile> FilterByName([FromQuery] string firstName, [FromQuery] string lastName)
        {
            return _profiles.Where(x => x.FirstName == firstName && x.LastName == lastName).ToList();
        }

        [HttpGet("postalCode")]
        public IList<Profile> FilterByPostalCode([FromQuery] string postalCode)
        {
            return _profiles.Where(x => x.Address!.PostalCode == postalCode).ToList();
        }

        [HttpGet("something")]
        public IList<Profile> FilterByProfileStatement([FromQuery] string text)
        {
            return _profiles.Where(x => x.Something == text).ToList();
        }

        [HttpGet("streetName")]
        public IList<Profile> FilterByStreetName([FromQuery] string streetName)
        {
            return _profiles.Where(x => x.Address!.StreetName == streetName).ToList();
        }

        [HttpGet("skill")]
        public IList<Profile> FilterBySkill([FromQuery] string skill)
        {
            return _profiles.Where(x => x.Skills.Contains(skill)).ToList();
        }

       

        private string GetAgeGroup(int age)
        {
            if (age <= 20)
                return "Below 20";
            else if (age <= 30)
                return "20-30";
            else if (age <= 40)
                return "31-40";
            else
                return "Above 40";
        }

        #region Update and Delete methods (Just in case)
        #region Update
        [HttpPatch("updateAge/{id}")]
        public IActionResult UpdateAge([FromRoute] string id, [FromBody] int newAge)
        {
            foreach (var profile in _profiles.Where(x => x.Id == id))
            {
                profile.Age = newAge;
            }
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

        #region reporting
        [HttpGet("report/ageGroup")]
        public IActionResult AgeGroupReport()
        {
            var result = _profiles.GroupBy(x => GetAgeGroup(x.Age))
                .Select(group => new
                {
                    AgeGroup = group.Key,
                    Count = group.Count()
                })
                .ToList();

            return Ok(result);
        }

        [HttpGet("report/skillFrequency")]
        public IActionResult SkillFrequencyReport()
        {
            var result = _profiles.SelectMany(x => x.Skills)
                .GroupBy(skill => skill)
                .Select(group => new
                {
                    Skill = group.Key,
                    Frequency = group.Count()
                })
                .ToList();

            return Ok(result);
        }

        [HttpPost("report/skill")]
        public IActionResult GenerateSkillReport([FromBody] string[] skills)
        {
            var profiles = _profiles.Where(x => skills.All(skill => x.Skills.Contains(skill))).ToList();

            var report = new Report
            {
                Id = Guid.NewGuid().ToString(),
                Timestamp = DateTime.UtcNow,
                Data = profiles
            };

            _reportRepository.Create(report);

            return Ok(report);
        }


        [HttpPost("report/skills-details")]
        public IActionResult GenerateSkillsDetailsReport([FromBody] ProfileFilter filter)
        {
            IList<Profile> profiles = new List<Profile>();

            if (filter.MinAge.HasValue && filter.MaxAge.HasValue)
            {
                profiles.Concat(this.FilterByAge(filter.MinAge.Value, filter.MaxAge.Value));
            }

            var skillsDetails = profiles.SelectMany(x => x.Skills).Distinct().ToList();

            var report = new Report
            {
                Id = Guid.NewGuid().ToString(),
                Timestamp = DateTime.UtcNow,
                Data = skillsDetails
            };

            _reportRepository.Create(report);

            return Ok(report);
        }

        #endregion
    }

}