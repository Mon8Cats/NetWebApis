using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkyApi.Models;
using ParkyApi.Models.Dtos;
using ParkyApi.Repository.IRepository;

namespace ParkyApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class NationalParksController : ControllerBase
    {
        private readonly INationalParkRepository _npRepo;
        private readonly IMapper _mapper;

        public NationalParksController(INationalParkRepository npRepo, IMapper mapper)
        {
            _npRepo = npRepo;
            _mapper = mapper;
        }

        /// <summary>
        /// Get list of national parks
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<NationalParkDto>))]
        //[ProducesResponseType(400)]
        public IActionResult GetNationalParks()
        {
            var parks =  _npRepo.GetNationalParks();
            var parkDtos = _mapper.Map<List<NationalParkDto>>(parks);
            return Ok(parkDtos);
        }

        /// <summary>
        /// Get individual national park
        /// </summary>
        /// <param name="id">The id of the national park</param>
        /// <returns></returns>
        [HttpGet("{id:int}", Name = "GetNationalPark")]
        [ProducesResponseType(200, Type = typeof(NationalParkDto))]
        //[ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public IActionResult GetNationalPark(int id)
        {
            var park = _npRepo.GetNationalPark(id);
            if(park == null)
            {
                return NotFound();
            }
            var parkDto = _mapper.Map<NationalParkDto>(park);
            return Ok(parkDto);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(NationalParkDto))]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[ProducesDefaultResponseType]
        public IActionResult CreateNationalPark([FromBody]  NationalParkDto parkDto)
        {
            if(parkDto == null)
            {
                return BadRequest(ModelState);
            }

            if (_npRepo.NationalParkExists(parkDto.Name))
            {
                ModelState.AddModelError("", "National park exists!");
                return StatusCode(404, ModelState);

            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var park = _mapper.Map<NationalPark>(parkDto);
            if (!_npRepo.CreateNationalPark(park))
            {
                ModelState.AddModelError("", $"Something went wrong when saving the record {park.Name}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetNationalPark", new { id = park.Id }, park); //201 Created
        }

        [HttpPatch("{id:int}", Name = "UpdateNationalPark")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateNationalPark(int id, [FromBody] NationalParkDto parkDto)
        {
            if (parkDto == null || id != parkDto.Id)
            {
                return BadRequest(ModelState);
            }

            var park = _mapper.Map<NationalPark>(parkDto);
            if (!_npRepo.UpdateNationalPark(park))
            {
                ModelState.AddModelError("", $"Something went wrong when updating the record {park.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{id:int}", Name = "DeleteNationalPark")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteNationalPark(int id)
        {
            if (!_npRepo.NationalParkExists(id))
            {
                return NotFound();
            }

            var park = _npRepo.GetNationalPark(id);

            if (!_npRepo.DeleteNationalPark(park))
            {
                ModelState.AddModelError("", $"Something went wrong when deleting the record {park.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

    }
}
