using Refit;

namespace RefitAPIClient.Services.Interface;

[Headers("accept:application/json", "Authorization: Bearer")]
public interface ITmdbAPI
{
    [Get("/search/person?query={name}")]
    Task<ActorList> GetActors(string name);
}