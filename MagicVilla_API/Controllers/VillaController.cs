using MagicVilla_API.Datos;
using MagicVilla_API.Modelos;
using MagicVilla_API.Modelos.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace MagicVilla_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaController : ControllerBase
    {
        //Inyectar el servicio de logger
        //implica la recopilación y almacenamiento de información sobre eventos y actividades en una aplicación.
        public VillaController(ILogger<VillaController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }
        private readonly ILogger<VillaController> _logger;
        private readonly ApplicationDbContext _db;

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<VillaDto>> GetVillas()
        {
            _logger.LogInformation("Obetener las villas");
            return Ok(_db.Villa.ToList());
        }
        [HttpGet("id:int", Name = "GetVilla")]
        /*Documentamos los diferentes tipos de codigos de estado que vamos a manejar con el atributo ProducesResponseType*/
        [ProducesResponseType(200)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<VillaDto> GetVilla(int id)
        {
            if (id == 0)
            {
                _logger.LogError($"Error al traer Villa con ID: {id}");
                return BadRequest();
            }
            //var villa = VillaStore.villaList.FirstOrDefault(m => m.Id == id);
            var villa = _db.Villa.FirstOrDefault(m => m.Id == id);
            if (villa == null) return NotFound();
            return Ok(villa);    
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        /* [FromBody] Indica con el tipo de dato que vamos a trabajar*/
        public ActionResult<VillaDto> CrearVilla([FromBody] VillaDto villaDto)
        {
            if(!ModelState.IsValid) return BadRequest();
            if(VillaStore.villaList.FirstOrDefault(m => m.Nombre.ToLower() == villaDto.Nombre.ToLower()) != null)
            {
                ModelState.AddModelError("NameExist", "Ese nombre ya se encuentra en uso");
                //Enviamos la validacion de modelstate personalizada
                return BadRequest(ModelState);
            }
            if(villaDto == null) return BadRequest();
            if(villaDto.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            Villa modelo = new()
            {
                Nombre = villaDto.Nombre,
                Detalle = villaDto.Detalle,
                ImagenUrl = villaDto.ImageUrl,
                Ocupantes = villaDto.Ocupantes,
                Tarifa = villaDto.Tarifa,
                MetrosCuadrados = villaDto.MetrosCuadrados,
                Amenidad = villaDto.Amenidad,
            };
            _db.Villa.Add(modelo);
            _db.SaveChanges();
            return CreatedAtRoute("GetVilla", new {id = villaDto.Id}, villaDto);
        }


        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult DeleteVilla(int id)
        {
            if (id == 0) return BadRequest();
            var villa = _db.Villa.FirstOrDefault(m => m.Id == id);
            if(villa == null) return NotFound();
            _db.Villa.Remove(villa);
            _db.SaveChanges();
            return NoContent();
        }



        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdateVilla(int id, [FromBody] VillaDto villaDto)
        {
            if (villaDto == null||id!= villaDto.Id) return BadRequest();
            Villa villaEdit = new()
            {
                Id = villaDto.Id,
                Nombre = villaDto.Nombre,
                Detalle = villaDto.Detalle,
                ImagenUrl = villaDto.ImageUrl,
                Ocupantes = villaDto.Ocupantes,
                Tarifa = villaDto.Tarifa,
                MetrosCuadrados = villaDto.MetrosCuadrados,
                Amenidad = villaDto.Amenidad
            };
            _db.Villa.Update(villaEdit);
            _db.SaveChanges();
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
        public IActionResult UpdatePartialVilla(int id, JsonPatchDocument<VillaDto> pathDto)
        {
            if (pathDto == null || id != 0) return BadRequest();
            var villa = _db.Villa.FirstOrDefault(m =>m.Id == id);
            VillaDto villaEdit = new()
            {
                Id = villa.Id,
                Nombre = villa.Nombre,
                Detalle = villa.Detalle,
                ImageUrl = villa.ImagenUrl,
                Ocupantes = villa.Ocupantes,
                Tarifa = villa.Tarifa,
                MetrosCuadrados = villa.MetrosCuadrados,
                Amenidad = villa.Amenidad
            };
            if (villa == null) return BadRequest();
            pathDto.ApplyTo(villaEdit, ModelState);
            if (!ModelState.IsValid) return BadRequest(ModelState);
            Villa modelo = new()
            {
                Id = villaEdit.Id,
                Nombre = villaEdit.Nombre,
                Detalle = villaEdit.Detalle,
                ImagenUrl = villaEdit.ImageUrl,
                Ocupantes = villaEdit.Ocupantes,
                Tarifa = villaEdit.Tarifa,
                MetrosCuadrados = villaEdit.MetrosCuadrados,
                Amenidad = villaEdit.Amenidad
            };
            _db.Villa.Update(modelo);
            _db.SaveChanges();

            return NoContent();
        }
    }
}
