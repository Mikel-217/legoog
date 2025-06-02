using System.ComponentModel.DataAnnotations;

namespace legoog.Models;
// save for later --> Class which will be used do store data at DB
public class Data
{
    [Key]
    public string? title { get; set; }
    public List<string>? keyword { get; set; }
    public string? url { get; set; }
    public int? keywordCount { get; set; } = 0;  // highest keyword count --> higher placement at search
}