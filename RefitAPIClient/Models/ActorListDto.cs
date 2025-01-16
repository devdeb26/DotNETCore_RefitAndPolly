using System.Text.Json.Serialization;

public class ActorList
{
    [JsonPropertyName("results")]
    public List<Actor> Actors { get; set; }
}