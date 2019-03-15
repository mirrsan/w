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
    [Route("api/[controller]/[action]")]
    public class OsobaUredjajController : BaseController<OsobaUredjaj, OsobaUredjajDTO>
    {
        private readonly IMapper _mapper;
        private readonly IOsobaUredjajRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOsobaRepository _osobaRepository;
        private readonly IKancelarijaRepository _kancelarijaRepository;
        private readonly IUredjajRepository _uredjajRepository;

        public OsobaUredjajController(IMapper mapper, IOsobaUredjajRepository repository, IUnitOfWork unitOfWork, IOsobaRepository osobaRepository, IKancelarijaRepository kancelarijaRepository, IUredjajRepository uredjajRepository) : base(mapper, repository, unitOfWork)
        {
            _mapper = mapper;
            _repository = repository;
            _unitOfWork = unitOfWork;
            _osobaRepository = osobaRepository;
            _kancelarijaRepository = kancelarijaRepository;
            _uredjajRepository = uredjajRepository;
        }

        /// <summary>
        /// Akcija koja ispisuje sve OsobaUredjaj objecte iz baze
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Sve()
        {
            return base.DajSve();
        }

        /// <summary>
        ///  akcija koja vraca entitet koji ima dati ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult PoId(long id)
        {
            return base.PoId(id);
        }

        /// <summary>
        ///  Upisivanje novi OsobaUredjaj object u bazy
        /// </summary>
        /// <param name="ouNovi"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult DodatiNovi(OsobaUredjajDTO ouNovi)
        {
            try
            {
                if (ouNovi == null)
                {
                    return Ok("aha ne moze to tako e");
                }
                _unitOfWork.PocniTransakciju();

                //ako se uredjaj vec koristi 
                var akoSeKorsiti = _repository.DajSveEntitete().Where(vec =>
                                   vec.Uredjaj.Naziv == ouNovi.Uredjaj.Naziv &&
                                   vec.KrajKoriscenja > DateTime.Now).FirstOrDefault();
                if (akoSeKorsiti != null)
                {
                    return BadRequest(
                        $"{ouNovi.Osoba.Ime} ne moze koristiti ovaj uredjaj zato sto ga vec neko koristi.");
                }

                OsobaUredjaj osobaUredjaj = _mapper.Map<OsobaUredjaj>(ouNovi);

                // ako osoba vec postoji
                Osoba osoba = (from nn in _osobaRepository.DajSveEntitete()
                    where nn.Ime == osobaUredjaj.Osoba.Ime && nn.Prezime == osobaUredjaj.Osoba.Prezime
                    select nn).FirstOrDefault();
                if (osoba != null)
                {
                    osobaUredjaj.Osoba = osoba;
                }

                // ako kancelarija vec postoji
                if (osoba == null)
                {
                    var kancelarija = _kancelarijaRepository.PretragaPoNazivu(ouNovi.Osoba.Kancelarija.Opis);
                    if (kancelarija != null)
                    {
                        osobaUredjaj.Osoba.Kancelarija = kancelarija;
                    }
                }

                // ako uredjaj vec postoji
                Uredjaj uredjaj = _uredjajRepository.PretragaPoImenu(ouNovi.Uredjaj.Naziv);
                if (uredjaj != null)
                {
                    osobaUredjaj.Uredjaj = uredjaj;
                }

                _repository.DodajEntitet(osobaUredjaj);
                _unitOfWork.Sacuvaj();
                _unitOfWork.ZavrsiTransakciju();

                return Ok("Sacuvano je ;)");
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
        /// akcija koja menja entitet sa datim ID sa novi  info
        /// </summary>
        /// <param name="id"></param>
        /// <param name="novi"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public IActionResult MenjanjeEntiteta(long id, OsobaUredjajDTO novi)
        {
            try
            {
                _unitOfWork.PocniTransakciju();

                OsobaUredjaj osobaUredjaj = _repository.EntitetPoId(id);

                Osoba osoba = _mapper.Map<Osoba>(novi.Osoba);
                Uredjaj uredjaj = _mapper.Map<Uredjaj>(novi.Uredjaj);

                //ako se uredjaj vec koristi 
                var akoSeKorsiti = _repository.DajSveEntitete().Where(vec => vec.Uredjaj.Naziv == novi.Uredjaj.Naziv &&
                                                                             vec.KrajKoriscenja > DateTime.Now)
                    .FirstOrDefault();
                if (akoSeKorsiti != null)
                {
                    return BadRequest($"{novi.Osoba.Ime} ne moze koristiti ovaj uredjaj zato sto ga vec neko koristi.");
                }

                // ako osobe ima u bazi
                var osoba2 = (from nn in _osobaRepository.DajSveEntitete()
                    where nn.Ime == osoba.Ime && nn.Prezime == osoba.Prezime
                    select nn).FirstOrDefault();
                if (osoba2 != null)
                {
                    osobaUredjaj.Osoba = osoba2;
                }
                else
                {
                    osobaUredjaj.Osoba = osoba;
                }

                // ako kancelarije ima u bazi
                if (osoba2 == null)
                {
                    var kancelarija = _kancelarijaRepository.PretragaPoNazivu(novi.Osoba.Kancelarija.Opis);
                    if (kancelarija != null)
                    {
                        osobaUredjaj.Osoba.Kancelarija = kancelarija;
                    }
                }

                // ako uredjaj ima u bazi
                Uredjaj uredjaj2 = _uredjajRepository.PretragaPoImenu(novi.Uredjaj.Naziv);
                if (uredjaj2 != null)
                {
                    osobaUredjaj.Uredjaj = uredjaj2;
                }
                else
                {
                    osobaUredjaj.Uredjaj = uredjaj;
                }

                osobaUredjaj.PocetakKoriscenja = novi.PocetakKoriscenja;
                osobaUredjaj.KrajKoriscenja = novi.KrajKoriscenja;
                _unitOfWork.Sacuvaj();
                _unitOfWork.ZavrsiTransakciju();

                return Ok("Zamenjemo.");
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
        /// akcija koja brise entitet iz baze
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public IActionResult BrisanjeEntiteta(long id)
        {
            return base.Brisanje(id);
        }

        /// <summary>
        /// akcija koja vraca entitete koji imaju pocetakKoriscenja veci i jednak od unetog
        /// </summary>
        /// <param name="pocetak"></param>
        /// <returns></returns>
        [HttpGet("pocetak")]
        public IActionResult PretragaPoPocetku(DateTime pocetak)
        {
            var niz = _repository.DajSveEntitete().Where(ou => ou.PocetakKoriscenja >= pocetak)
                                                  .Select(osobaUredjaj => _mapper.Map<OsobaUredjajDTO>(osobaUredjaj));

            return Ok(niz.ToList());
        }

        /// <summary>
        /// akcija koja vraca entitet koji ima dat pocetak i kraj koriscenja
        /// </summary>
        /// <param name="pocetak"></param>
        /// <param name="kraj"></param>
        /// <returns></returns>
        [HttpGet("{pocetak}/{kraj}")]
        public IActionResult PretragaPocetakKraj(DateTime pocetak, DateTime kraj)
        {
            var niz = _repository.DajSveEntitete()
                     .Where(ou => ou.PocetakKoriscenja == pocetak && ou.KrajKoriscenja == kraj)
                     .Select(osobaUredjaj => _mapper.Map<OsobaUredjajDTO>(osobaUredjaj));

            return Ok(niz.ToList());
        }
    }
}