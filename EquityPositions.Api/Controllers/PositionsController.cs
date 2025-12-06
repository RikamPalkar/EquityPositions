using EquityPositions.Api.DTOs;
using EquityPositions.Domain.Entities;
using EquityPositions.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EquityPositions.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PositionsController : ControllerBase
    {
        private readonly IPositionRepository _positionRepository;
        private readonly ILogger<PositionsController> _logger;

        public PositionsController(
            IPositionRepository positionRepository,
            ILogger<PositionsController> logger)
        {
            _positionRepository = positionRepository;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<PositionResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var positions = await _positionRepository.GetAllAsync();
            var response = positions.Select(MapToResponse);
            return Ok(ApiResponse<IEnumerable<PositionResponse>>.SuccessResponse(response));
        }

        [HttpGet("{securityCode}")]
        [ProducesResponseType(typeof(ApiResponse<PositionResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<PositionResponse>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetBySecurityCode(string securityCode)
        {
            var position = await _positionRepository.GetBySecurityCodeAsync(securityCode.ToUpperInvariant());

            if (position == null)
            {
                return NotFound(ApiResponse<PositionResponse>.ErrorResponse($"Position for security {securityCode} not found"));
            }

            return Ok(ApiResponse<PositionResponse>.SuccessResponse(MapToResponse(position)));
        }

        private static PositionResponse MapToResponse(Position position)
        {
            return new PositionResponse
            {
                SecurityCode = position.SecurityCode,
                Quantity = position.Quantity,
                LastUpdatedAt = position.LastUpdatedAt
            };
        }
    }
}

