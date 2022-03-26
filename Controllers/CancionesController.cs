using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; //ToListAsync
using WebApiCanciones.Filtros;
using WebApiCanciones.Entidades;
using WebApiCanciones.DTOs; //Sirve para no filtrar entidades directas

namespace WebApiCanciones.Controllers
{
    [ApiController]
    [Route("api/canciones")]
    public class CancionesController : Microsoft.AspNetCore.Mvc.ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;
        

        /*private readonly IService service;
        private readonly ServiceTransient serviceTransient;
        private readonly ServiceScoped serviceScoped;
        private readonly ServiceSingleton serviceSingleton;
        private readonly ILogger<CancionesController> logger;*/

        public CancionesController(ApplicationDbContext context, IMapper mapper /*,IService service, //Se utiliza la interfaz completa IService
                                    ServiceTransient serviceTransient, ServiceScoped serviceScoped,
                                    ServiceSingleton serviceSingleton, ILogger<CancionesController> logger*/)
        {
            this.dbContext = context;
            this.mapper = mapper;
          
            /*this.service = service;
            this.serviceTransient = serviceTransient;
            this.serviceScoped = serviceScoped;
            this.serviceSingleton = serviceSingleton;
            this.logger = logger;*/
        }

        /*[HttpGet("GUID")]
        [ResponseCache(Duration = 10)] //Tiempo definido en segundos
        [ServiceFilter(typeof(FiltroDeAccion))]

        public ActionResult ObtenerGuid()

        {
            logger.LogInformation("Durante la ejecución"); //Verifica que si se siga un orden con los logs

            return Ok(new
            {
                
                CancionesControllerTransient = serviceTransient.guid,
                ServiceA_Transient = service.GetTransient(),
                CancionesControllerScoped = serviceScoped.guid,
                ServiceA_Scoped = service.GetScoped(),
                CancionesControllerSingleton = serviceSingleton.guid,
                ServiceA_Singleton = service.GetSingleton()

            }) ;
            
        }*/

        [HttpGet]
        //[HttpGet("listado")] //api/canciones/listado
        //[HttpGet("/listado")] // /listado
        //[ResponseCache(Duration = 15)] //Tiempo definido en segundos
        //[Authorize]
        //[ServiceFilter(typeof(FiltroDeAccion))]

        public async Task<ActionResult<List<GetCancionDTO>>> Get()
        {
            //* Niveles de logs
            //Critical
            //Error
            //Warning
            //Information
            //Debug
            //Trace
            // *//

            //throw new NotImplementedException(); marca el error en color rojo
            //logger.LogInformation("Se obtiene el listado de canciones");
            //logger.LogWarning("Mensaje de prueba warning");
            //service.EjecutarJob();
            var canciones = await dbContext.Canciones.ToListAsync();
            return mapper.Map<List<GetCancionDTO>>(canciones);
            //return await dbContext.Canciones.Include(x => x.albumes).ToListAsync();

        }

        /*[HttpGet("primero")] //api/canciones/primero?
        public async Task<ActionResult<Cancion>> PrimeraCancion([FromHeader] int valor, [FromQuery] string cancion, [FromQuery] int cancionId)
        {
            return await dbContext.Canciones.FirstOrDefaultAsync();
        }*/

        [HttpGet("{id:int}")]
        public async Task<ActionResult<GetCancionDTO>> Get(int id)
        {
            var cancion = await dbContext.Canciones.FirstOrDefaultAsync(cancionBD => cancionBD.Id == id);

            if (cancion == null)
            {
                return NotFound();
            }
            return mapper.Map<GetCancionDTO>(cancion);
        }

 
        

        /*[HttpGet("{id:int}/{param?}")] //? ahorra escribir el nombre de la canción a buscar
        public async Task<ActionResult<Cancion>> Get(int id, string param)
        {
            var cancion = await dbContext.Canciones.FirstOrDefaultAsync(x => x.Id == id);
            if (param == null)
            {
                return NotFound();
            }
            return cancion;
        }*/

        /*[HttpGet("primero2")]
        public ActionResult<Cancion> PrimeraCancion() //Canción es el tipo de objeto
        {
            return new Cancion() { Nombre = "DOS" }; //Entonces regresa un objeto de ese tipo
        }*/

        /*[HttpGet("primero2")]
        public ActionResult<int> PrimeraCancion() //Int es el tipo de objeto
        {
            return 13; //Entonces regresa un objeto de ese tipo
        }*/

        /*[HttpGet("primero2")]
        public ActionResult<string> PrimeraCancion() //string es el tipo de objeto
        {
            return "Una cadena"; //Entonces regresa un objeto de ese tipo
        }*/

        /*[HttpGet("primero2")]
        public IActionResult PrimeraCancion() //IActionResult 
        {
            return Ok("Una cadena"); //Entonces regresa una acción 
        }*/

        /*[HttpGet("primero2")]
        public string PrimeraCancion() // Cadena
        {
            return "Una cadena"; //Entonces regresa una cadena
        }*/

        [HttpGet("{nombre}")] //Busca el primer registro que contenga el texto ingresado
        public async Task<ActionResult<List<GetCancionDTO>>> Get([FromRoute] string nombre)
        {
            var canciones = await dbContext.Canciones.Where(cancionBD => cancionBD.Nombre.Contains(nombre)).ToListAsync();

            var ruta = "C:/Users/alice/source/repos/WebApiCanciones/WebApiCanciones/wwwroot/registroConsultado.txt";

            foreach (var cancion in canciones)
            {
                using (StreamWriter writer = new StreamWriter(ruta, append: true)) { writer.WriteLine(cancion.Nombre); }

            }
            

            return mapper.Map<List<GetCancionDTO>>(canciones);
        }

   


        [HttpPost]
        public async Task<ActionResult> Post([FromBody] CancionDTO cancionDto)
        {
            //Ejemplo para validar desde el controlador con la BD con ayuda del dbContext

            var existeCancionMismoNombre = await dbContext.Canciones.AnyAsync(x => x.Nombre == cancionDto.Nombre);

            if (existeCancionMismoNombre)
            {
                return BadRequest($"Ya existe una canción con el nombre {cancionDto.Nombre}");
            }

            //var cancion = new Cancion()
            //{
            //    Nombre = cancionDto.Nombre
            //};

            var cancion = mapper.Map<Cancion>(cancionDto);

            dbContext.Add(cancion);
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id:int}")] // api/cancion/1
        public async Task<ActionResult> Put(Cancion cancion, int id)
        {
            var exist = await dbContext.Canciones.AnyAsync(x => x.Id == id);
            if (!exist)
            {
                return NotFound();
            }

            if (cancion.Id != id)
            {
                return BadRequest("El id de la canción no coincide con el establecido en la url.");
            }

            dbContext.Update(cancion);
            await dbContext.SaveChangesAsync();

            var ruta = "C:/Users/alice/source/repos/WebApiCanciones/WebApiCanciones/wwwroot/nuevosRegistros.txt";
            using (StreamWriter writer = new StreamWriter(ruta, append: true)) { writer.WriteLine(cancion.Nombre); }


            return Ok();
        }

        

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var exist = await dbContext.Canciones.AnyAsync(x => x.Id == id);
            if (!exist)
            {
                return NotFound("El Recurso no fue encontrado.");
            }

            dbContext.Remove(new Cancion()
            {
                Id = id
            });
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        

        /*[HttpPost]
        public async Task<ActionResult> Post([FromBody] Cancion cancion) //FromBody, se manda desde el jason
        //async: programación asíncrona, pueden ejecutarse tareas en segundo espacio
        //task: métodos asíncronos devuelven task
        {
            dbContext.Add(cancion);
            await dbContext.SaveChangesAsync();
            return Ok();
        }
        

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(Cancion cancion, int id)
        {
            if(cancion.Id != id)
            {
                return BadRequest("El id de la canción no coincide con el establecido en la url.");
            }

            dbContext.Update(cancion);
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var exist = await dbContext.Canciones.AnyAsync(x => x.Id == id);
            if (!exist)
            {
                return NotFound("El recurso no fue encontrado.");
           
            }

            dbContext.Remove(new Cancion()
            {
                Id = id
            });
            await dbContext.SaveChangesAsync();
            return Ok();
        }*/





    }
}
