using System;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ZadatakNeki.DTO;
using ZadatakNeki.Models;
using ZadatakNeki.Repository;
using ZadatakNeki.Repositorys.IRepository;

namespace ZadatakNeki.Controllers
{
    [Route("api/[controller]/[action]")]
    public class OsobaController : BaseController<Osoba, OsobaDTO>
    {
        private readonly IMapper _mapper;
        private readonly IOsobaRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IKancelarijaRepository _kancelarijaRepository;
        private readonly IOsobaUredjajRepository _osobaUredjajRepository;

        public OsobaController(IMapper mapper, IOsobaRepository repository, IUnitOfWork unitOfWork, IKancelarijaRepository kancelarijaRepository, IOsobaUredjajRepository osobaUredjajRepository) : base(mapper, repository, unitOfWork)
        {
            _mapper = mapper;
            _repository = repository;
            _unitOfWork = unitOfWork;
            _kancelarijaRepository = kancelarijaRepository;
            _osobaUredjajRepository = osobaUredjajRepository;
        }

        /// <summary>
        /// metod koji vraca sve osobe
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult SveOsobe()
        {
            return base.DajSve();
        }

        /// <summary>
        /// Akcija koja vraca samo entitet koji ima dati ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult OsobaPoId(long id)
        {
            return base.PoId(id);
        }

        /// <summary>
        /// Akcija koja upisuje novi entitet u bazu
        /// </summary>
        /// <param name="osobaInfo"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult UpisivanjeOsobe(OsobaDTO osobaInfo)
        {
            if(osobaInfo == null)
            {
                return BadRequest("Niste upisali podatke da valja!");
            }
            Osoba osoba = _mapper.Map<Osoba>(osobaInfo);

            var kancelarija = _kancelarijaRepository.PretragaPoNazivu(osobaInfo.Kancelarija.Opis);

            if (kancelarija != null)
            {
                osoba.Kancelarija = kancelarija;
            }

            _repository.DodajEntitet(osoba);
            _unitOfWork.Sacuvaj();
            return Ok("Osoba je sacuvana u bazi.");
        }

        /// <summary>
        /// Akcija koja menja postojeci entitet koji ima dati ID
        /// </summary>
        /// <param name="id"></param>
        /// <param name="noviInfo"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public IActionResult IzmenaOsobe(long id, OsobaDTO noviInfo)
        {
            try
            {
                _unitOfWork.PocniTransakciju();

                Osoba stariInfo = _repository.EntitetPoId(id);
                if (stariInfo == null)
                {
                    return BadRequest("Nije pronadjena osoba sa datim ID.");
                }

                stariInfo.Ime = noviInfo.Ime;
                stariInfo.Prezime = noviInfo.Prezime;

                var kancelarija = _kancelarijaRepository.PretragaPoNazivu(noviInfo.Kancelarija.Opis);

                if (kancelarija != null)
                {
                    stariInfo.Kancelarija = kancelarija;
                }
                else
                {
                    stariInfo.Kancelarija = _mapper.Map<Kancelarija>(noviInfo.Kancelarija);
                }
                _unitOfWork.Sacuvaj();
                _unitOfWork.ZavrsiTransakciju();

                return Ok("Sacuvane su izmene.");
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
        /// Akcija koja brise entitet koji ima dati ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public IActionResult BrisanjeOsobe(long id)
        {
            try
            {
                Osoba osoba = _repository.EntitetPoId(id);

                if (osoba == null)
                {
                    return NotFound();
                }

                var mozda = (from nn in _osobaUredjajRepository.DajSveEntitete()
                    where nn.Osoba == osoba
                    select new {Osoba = nn.Osoba.Ime, Uredjaj = nn.Uredjaj.Naziv}).FirstOrDefault();

                if (mozda != null)
                {
                    return BadRequest("Osoba nije izbrisana iz baze zato sto je vec koristila/koristi neki od uredjaja.");
                }

                _repository.ObrisiEntitet(osoba);
                _unitOfWork.Sacuvaj();

                return Ok("Osoba je izbrisana iz baze podataka.");
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
        /// Akcija koja pretrazuje entitet po imenu osobe
        /// </summary>
        /// <param name="ime"></param>
        /// <returns></returns>
        [HttpGet("{ime}")]
        public IActionResult PretragaPoImenuOsobe(string ime)
        {
            var osoba = _repository.PretragaPoImenu(ime);

            if (osoba == null)
            {
                return NotFound("Nema te osobe");
            }
            return Ok(_mapper.Map<OsobaDTO>(osoba));
        }

        /// <summary>
        /// Akcija koja pretrazuje entitete po nazivu kancelarije
        /// </summary>
        /// <param name="opisKancelarije"></param>
        /// <returns></returns>
        [HttpGet("{opisKancelarije}")]
        public IActionResult PretragaPoOpisuKancelarije(string opisKancelarije)
        {
            var osoba = _repository.DajSveEntitete().Where(o => o.Kancelarija.Opis == opisKancelarije)
                                                    .Select(osobaa => _mapper.Map<OsobaDTO>(osobaa));
            if (osoba == null)
            {
                return NotFound();
            }
            return Ok(osoba.ToList());
        }
    }
}