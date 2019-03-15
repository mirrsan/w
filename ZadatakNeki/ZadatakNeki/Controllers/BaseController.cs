using System;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZadatakNeki.Models;
using ZadatakNeki.Repositorys.IRepository;
using ZadatakNeki.Repositorys.Repository;

namespace ZadatakNeki.Controllers
{
    [Route("api/[controller]/[action]")]
    public abstract class BaseController<T, Tdto> : Controller where T : class  where Tdto : class 
    {
        private readonly IMapper _mapper;
        private readonly IRepository<T> _repository;
        private readonly IUnitOfWork _unitOfWork;
        
        public BaseController(IMapper mapper, IRepository<T> repository, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        // akcija koja vraca sve entitete
        [HttpGet]
        protected IActionResult DajSve()
        {
            return Ok(_repository.DajSveEntitete().Select(w => _mapper.Map<Tdto>(w)));
        }

        // akcija koja vraca entitet koji ima dati ID
        [HttpGet("{id}")]
        protected IActionResult PoId(long id)
        {
            try
            {
                var entitet = _mapper.Map<Tdto>(_repository.EntitetPoId(id));

                if (entitet == null)
                {
                    return BadRequest("Nema ti toga vamo.");
                }

                return Ok(entitet);
            }
            catch (Exception ex)
            {
                Error greska = new Error()
                {
                    Exception = ex.Message,
                    StackTrace = ex.StackTrace
                };
                return BadRequest(greska);
            }
        }

        // akcija koja upisuje entitet u bazu
        [HttpPost]
        protected IActionResult Upisivanje(Tdto koga)
        {
            if (koga == null)
            {
                return BadRequest("Upisi fino to.");
            }

            _unitOfWork.PocniTransakciju();

            T entitet = _mapper.Map<T>(koga);
            _repository.DodajEntitet(entitet);
            _unitOfWork.Sacuvaj();

            _unitOfWork.ZavrsiTransakciju();

            return Ok("Nije uspelo, ahahah salim se sacuvao sam.");
        }

        // akcija za izmenu podataka entiteta
        [HttpPut("{id}")]
        protected IActionResult Izmena(long id, Tdto entitetDTO)
        {
            T entitet = _repository.EntitetPoId(id);
            if (entitet == null)
            {
                return BadRequest("Ne mogu naci entitet koji zelis da menjas.");
            }
            T map = _mapper.Map<Tdto, T>(entitetDTO, entitet);
            _repository.Izmeni(map);
            _unitOfWork.Sacuvaj();

            return Ok("Vase izmene su sacuvane!");
        }

        // akcija za brisanje entiteta 
        [HttpDelete("{id}")]
        protected IActionResult Brisanje(long id)
        {
            T entitet = _repository.EntitetPoId(id);
            if (entitet == null)
            {
                return NotFound("Sta ces brisat, njega nako nije ni bilo.");
            }
            _repository.ObrisiEntitet(entitet);
            _unitOfWork.Sacuvaj();

            return Ok("Obrisao sam.");
        }
    }
}