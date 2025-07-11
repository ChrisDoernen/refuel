namespace Core.Shared;

public class ReadModel<T>
    where T : IReplayable<T>
{
  // Alle events subscriben, für die sich das ReadModel interessiert
  
  // Viele Instanzen verwalten -> Checken ob es schon eine Inszanz mit der Id gibt
  // Evtl. subject parsen
}
