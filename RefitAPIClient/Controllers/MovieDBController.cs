using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using RefitAPIClient.Services.Interface;

namespace RefitAPIClient.Controllers;

public class MovieController: ControllerBase
{
    private readonly ITmdbAPI _tmdbAPI;

    public MovieController(ITmdbAPI tmdbAPI)
    {
        _tmdbAPI = tmdbAPI;
    }

    [HttpGet("actors/")]
    public async Task<ActorList> GetActors([FromQuery][Required]string name)
    {
        var response = await _tmdbAPI.GetActors(name);
        return response;
    }
   
}