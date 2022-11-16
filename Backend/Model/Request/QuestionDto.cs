using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Backend.Model.Request;

public class QuestionDto
{
    [Required]
    [JsonPropertyName("transcribed_text")]
    public string TranscribedText { get; set; } = null!;

    public Questions Question { get; set; }

    [Required]
    [JsonPropertyName("result_question")]
    public string ResultQuestion { get; init; } = null!;
}