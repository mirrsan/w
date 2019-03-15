using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ZadatakNeki.DTO;
using ZadatakNeki.Models;
using ZadatakNeki.Repository;
using ZadatakNeki.Repositorys.IRepository;

namespace ZadatakNeki.Controllers
{
    [Route("[controller]/[action]")]
    public class UredjajController : BaseController<Uredjaj, UredjajDTO>
    {
        private readonly IMapper _mapper;
        private readonly IUredjajRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOsobaUredjajRepository _osobaUredjajRepository;

        public UredjajController(IMapper mapper, IUredjajRepository repository, IUnitOfWork unitOfWork, IOsobaUredjajRepository osobaUredjajRepository) : base(mapper, repository, unitOfWork)
        {
            _mapper = mapper;
            _repository = repository;
            _unitOfWork = unitOfWork;
            _osobaUredjajRepository = osobaUredjajRepository;
        }

        /// <summary>
        /// Akcija koja vraca sve uredjaje
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Svi()
        {
            return base.DajSve();
        }

        /// <summary>
        /// akcija koja vraca samo entitet koji ima dati ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}:int")]
        public IActionResult PoId(long id)
        {
            return base.PoId(id);
        }

        /// <summary>
        /// akcija koja upisuje novi entitet u bazu
        /// </summary>
        /// <param name="uredjajNovi"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Upisivanje(UredjajDTO uredjajNovi)
        {
            return base.Upisivanje(uredjajNovi);
        }

        /// <summary>
        /// akcija koja menja postojeci entitet koji ima dati ID
        /// </summary>
        /// <param name="id"></param>
        /// <param name="uredjaj"></param>
        /// <returns></returns>
        [HttpPut("{id}:int")]
        public IActionResult IzmenaPodataka(long id, UredjajDTO uredjaj)
        {
            return base.Izmena(id, uredjaj);
        }

        /// <summary>
        /// akcija koja brise entitet koji ima dati ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public IActionResult Brisanje(long id)
        {
            try
            {
                Uredjaj uredjaj = _repository.EntitetPoId(id);

                if (uredjaj == null)
                {
                    return NotFound("Nema uredjaj sa datim ID.");
                }

                var mozda = (from nn in _osobaUredjajRepository.DajSveEntitete()
                    where nn.UredjajId == id
                    select new {Uredjaj = nn.Uredjaj.Naziv}).FirstOrDefault();

                if (mozda != null)
                {
                    return BadRequest("Uredjaj nije izbrisan, zato sto se koristi ili je nekada vec bio koriscen.");
                }

                _repository.ObrisiEntitet(uredjaj);
                _unitOfWork.Sacuvaj();

                return Ok("Uredjaj je izbrisan iz baze podataka.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// akcija koja pretrazuje entitet po imenu osobe
        /// </summary>
        /// <param name="naziv"></param>
        /// <returns></returns>
        [HttpGet("{naziv}")]
        public IActionResult PretragaPoImenu(string naziv)
        {
            var uredjaj = _repository.PretragaPoImenu(naziv);
            if (uredjaj == null)
            {
                return NotFound("Nista");
            }
            return Ok(uredjaj);
        }

        /// <summary>
        /// akcija koja vraca uredjaje koji se koriste
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult PretragaUredjajiKojiSeKoriste()
        {
            var koristeSe = from nn in _osobaUredjajRepository.DajSveEntitete() select new {Uredjaj = nn.Uredjaj.Naziv};

            return Ok(koristeSe.ToList());
        }
        
    }
}
