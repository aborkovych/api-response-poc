using ApiResponse.Poc.Extensions;
using ApiResponse.Poc.Models;
using ApiResponse.Poc.Responses;
using Microsoft.AspNetCore.Mvc;

namespace ApiResponse.Poc.Controllers;

[ApiController]
[Route("[controller]")]
public class AppointmentController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<AppointmentDto>>), StatusCodes.Status200OK)]
    [ProducesErrorResponseType(typeof(ErrorResponse))]
    public IActionResult Get([FromQuery] int page = 1, [FromQuery] int pageSize = 5)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 5;
        if (pageSize > 100) pageSize = 100;

        var random = new Random();
        if (random.Next(0, 10) <= 5) // 50% chance to return an error
        {
            var fail = ApiResponse<List<AppointmentDto>>.Failure(new ErrorResponse
            {
                Message = "Failed to fetch one of the appointments from the database.",
                ErrorCode = ErrorCode.DatabaseError
            });
            return fail.ToActionResult();
        }

        var allAppointments = GenerateAppointments();

        var totalItems = allAppointments.Count;
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        var paginatedAppointments = allAppointments
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var response = ApiResponse<List<AppointmentDto>>.Success(paginatedAppointments, new MetaInfo
        {
            Page = new PageMeta
            {
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages
            }
        });
        
        return response.ToActionResult();
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<AppointmentDto>), StatusCodes.Status200OK)]
    [ProducesErrorResponseType(typeof(ErrorResponse))]
    public IActionResult GetById(int id)
    {
        var allAppointments = GenerateAppointments(count: 100);
        var appointment = allAppointments.FirstOrDefault(a => a.Id == id);
        if (appointment is null)
        {
            var apiResponse = ApiResponse<AppointmentDto>.NotFound(id, message: "Appointment not found.");
            return apiResponse.ToActionResult();
        }

        var response = ApiResponse<AppointmentDto>.Success(appointment);
        return response.ToActionResult();
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<AppointmentDto>), StatusCodes.Status200OK)]
    [ProducesErrorResponseType(typeof(ErrorResponse))]
    public IActionResult Create([FromBody] CreateAppointmentDto createDto)
    {
        var random = new Random();
        if (random.Next(0, 10) < 2)
        {
            var dbError = ApiResponse<AppointmentDto>.Failure(new ErrorResponse
            {
                Message = "Failed to create appointment in the database.",
                ErrorCode = ErrorCode.DatabaseError
            });
            return dbError.ToActionResult();
        }

        // Create new appointment
        var newAppointment = new AppointmentDto
        {
            Id = new Random().Next(101, 10000),
            Date = createDto.Date,
            CustomerName = createDto.CustomerName
        };

        var response = ApiResponse<AppointmentDto>.Success(newAppointment);
        return response.ToActionResult();
    }

    private static List<AppointmentDto> GenerateAppointments(int count = 25)
    {
        var allAppointments = new List<AppointmentDto>();
        for (var i = 1; i <= count; i++)
        {
            allAppointments.Add(new AppointmentDto 
            { 
                Id = i, 
                Date = DateTime.UtcNow.AddDays(i), 
                CustomerName = $"Customer {i}" 
            });
        }

        return allAppointments;
    }
}
