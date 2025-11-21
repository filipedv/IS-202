namespace OBLIG1.Models;

// representerer informasjon om feil
public class ErrorViewModel
{
    //request id brukes til å spore feil i loggen
    public string? RequestId { get; set; }

    //showRequestID returnere true dersom det inneholder en verdi
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}