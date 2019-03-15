using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ZadatakNeki.DTO;
using ZadatakNeki.Models;
using ZadatakNeki.Repositorys.IRepository;
using ZadatakNeki.Repositorys.Repository;

namespace ZadatakNeki.Controllers
{
    [Route("api/[controller]/[action]")]
    public class KancelarijaController : BaseController<Kancelarija, KancelarijaDTO>
    {
        private readonly IMapper _mapper;
        private readonly IKancelarijaRepository _repository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IOsobaRepository _osobaRepository;

        public KancelarijaController(IMapper mapper, IKancelarijaRepository repository,
            IUnitOfWork unitOfWork, IOsobaRepository osobaRepository) : base(mapper, repository, unitOfWork)
        {
            _mapper = mapper;
            _repository = repository;
            this.unitOfWork = unitOfWork;
            _osobaRepository = osobaRepository;
        }

        /// <summary>
        /// Akcija koja vraca sve kancelarije
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult SveKancelarije()
        {
            return base.DajSve();
        }

        /// <summary>
        /// Akcija koja vraca samo entitet koji ima dati ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}:int")]
        public IActionResult PoId(long id)
        {
            return base.PoId(id);
        }

        /// <summary>
        /// Akcija koja upisuje novi entitet u bazu
        /// </summary>
        /// <param name="kancelarija"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Upisivanje(KancelarijaDTO kancelarija)
        {
            return base.Upisivanje(kancelarija);
        }

        /// <summary>
        /// Akcija koja menja postojeci entitet koji ima dati ID
        /// </summary>
        /// <param name="id"></param>
        /// <param name="kDTO"></param>
        /// <returns></returns>
        [HttpPut("{id}:int")]
        public IActionResult IzmenaPodataka(long id, KancelarijaDTO kDTO)
        {
            return base.Izmena(id, kDTO);
        }

        /// <summary>
        /// Akcija koja brise entitet koji ima dati ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public IActionResult Brisanje(long id)
        {
            try
            {
                Kancelarija kancelarija = _repository.EntitetPoId(id);

                if (kancelarija == null)
                {
                    return NotFound("Nema takve kancelarije.");
                }

                if (kancelarija.Opis.Equals("kantina"))
                {
                    return BadRequest("Necu obrisat kantinu.");
                }
                unitOfWork.PocniTransakciju();
                
                List<Osoba> beziIzKancelarije = _osobaRepository.DajSveEntitete().Where(o => o.Kancelarija == kancelarija).ToList();

                for (int i = 0; i < beziIzKancelarije.Count(); i++)
                {
                    Osoba osoba = beziIzKancelarije[i];
                    osoba.Kancelarija = new Kancelarija() { Opis = "kantina" };

                    var mozdaIma = _repository.DajSveEntitete().Where(k => k.Opis == osoba.Kancelarija.Opis)
                        .FirstOrDefault();
                    if (mozdaIma != null)
                    {
                        osoba.Kancelarija = mozdaIma;
                    }
                    unitOfWork.Sacuvaj();
                    unitOfWork.ZavrsiTransakciju();
                }

                _repository.ObrisiEntitet(kancelarija);
                unitOfWork.Sacuvaj();

                return Ok("Kancelarija je izbrisana iz baze podataka.");
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

        /// <summary>
        /// Akcija koja pretrazuje entitet po nazivu kancelarije
        /// </summary>
        /// <param name="opis"></param>
        /// <returns></returns>
        [HttpGet("{opis}")]
        public IActionResult PretragaPoNazivu(string opis)
        {
            var kancelarija = _repository.PretragaPoNazivu(opis);
            if (kancelarija == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<KancelarijaDTO>(kancelarija));
        }
    }

}
