using Application.Dtos;
using Application.IRepository;
using Domain.Entities;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace ApiEcommerce.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationRepository _reservationRepository;
        public ReservationController(IReservationRepository reservationRepository)
        {
            _reservationRepository = reservationRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateReservationDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reservation = dto.Adapt<Reservation>();
            await _reservationRepository.AddAsync(reservation);
            return Ok(reservation);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var reservations = await _reservationRepository.GetAllAsync();
            return Ok(reservations);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var reservation = await _reservationRepository.GetByIdAsync(id);
            if (reservation == null)
                return NotFound();
            return Ok(reservation);
        }
    }
}