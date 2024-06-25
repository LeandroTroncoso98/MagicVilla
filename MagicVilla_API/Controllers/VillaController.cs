using AutoMapper;
using MagicVilla_API.Datos;
using MagicVilla_API.Modelos;
using MagicVilla_API.Modelos.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace MagicVilla_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaController : ControllerBase
    {
        //Inyectar el servicio de logger
        //implica la recopilación y almacenamiento de información sobre eventos y actividades en una aplicación.
        public VillaController(ILogger<VillaController> logger, ApplicationDbContext db, IMapper mapper)
        {
            _logger = logger;
            _db = db;
            _mapper = mapper;
        }
        private readonly ILogger<VillaController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<VillaDto>>> GetVillas()
        {
            _logger.LogInformation("Obetener las villas");
            IEnumerable<Villa> villaList = await _db.Villa.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<VillaDto>>(villaList));
        }



        [HttpGet("id:int", Name = "GetVilla")]
        /*Documentamos los diferentes tipos de codigos de estado que vamos a manejar con el atributo ProducesResponseType*/
        [ProducesResponseType(200)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VillaDto>> GetVilla(int id)
        {
            if (id == 0)
            {
                _logger.LogError($"Error al traer Villa con ID: {id}");
                return BadRequest();
            }
            //var villa = VillaStore.villaList.FirstOrDefault(m => m.Id == id);
            var villa = await _db.Villa.FirstOrDefaultAsync(m => m.Id == id);
            if (villa == null) return NotFound();
            return Ok(_mapper.Map<VillaDto>(villa));    
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        /* [FromBody] Indica con el tipo de dato que vamos a trabajar*/
        public async Task<ActionResult<VillaDto>> CrearVilla([FromBody] VillaCreateDto createDto)
        {
            if(!ModelState.IsValid) return BadRequest();

            if(await _db.Villa.FirstOrDefaultAsync(m => m.Nombre.ToLower() == createDto.Nombre.ToLower()) != null)
            {
                ModelState.AddModelError("NameExist", "Ese nombre ya se encuentra en uso");
                //Enviamos la validacion de modelstate personalizada
                return BadRequest(ModelState);
            }
            if(createDto == null) return BadRequest();
            
            Villa modelo = _mapper.Map<Villa>(createDto);
            await _db.Villa.AddAsync(modelo);
            await _db.SaveChangesAsync();
            return CreatedAtRoute("GetVilla", new {id = modelo.Id}, modelo);
        }


        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteVilla(int id)
        {
            if (id == 0) return BadRequest();
            var villa =await _db.Villa.FirstOrDefaultAsync(m => m.Id == id);
            if(villa == null) return NotFound();
            _db.Villa.Remove(villa);
            await _db.SaveChangesAsync();
            return NoContent();
        }



        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateVilla(int id, [FromBody] VillaUpdateDto updateDto)
        {
            if (updateDto == null||id!= updateDto.Id) return BadRequest();
            //Villa villaEdit = new()
            //{
            //    Id = villaDto.Id,
            //    Nombre = villaDto.Nombre,
            //    Detalle = villaDto.Detalle,
            //    ImagenUrl = villaDto.ImageUrl,
            //    Ocupantes = villaDto.Ocupantes,
            //    Tarifa = villaDto.Tarifa,
            //    MetrosCuadrados = villaDto.MetrosCuadrados,
            //    Amenidad = villaDto.Amenidad
            //};
            //Con automapper omitimos todo el paso anterior
            Villa villa = _mapper.Map<Villa>(updateDto);
            _db.Villa.Update(villa);
            await _db.SaveChangesAsync();
            return NoContent();
        }




        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        /*
         Previo a Path hay que instalar las siguientes dependencias:
            Microsoft.AspNetCore.JsonPatch
            Microsoft.aspnetcore.mvc.newtonsoftjson
        y agregar el servicio de newtonSoftJson en program
         */
        public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDto> pathDto)
        {
            if (pathDto == null || id != 0) return BadRequest();
            var villa = await _db.Villa.AsNoTracking().FirstOrDefaultAsync(m =>m.Id == id);
            VillaUpdateDto villaDto = _mapper.Map<VillaUpdateDto>(villa);

            if (villa == null) return BadRequest();

            pathDto.ApplyTo(villaDto, ModelState);

            if (!ModelState.IsValid) return BadRequest(ModelState);

            Villa modelo = _mapper.Map<Villa>(villaDto);

            _db.Villa.Update(modelo);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}
