namespace PilotageForfaits.Contracts.Suivi;

/// <summary>Résultat d'une saisie : le suivi recalculé et les messages à afficher.</summary>
public sealed class ResultatSaisieDto
{
    public required object Suivi { get; init; }
    public List<MessageDto> Messages { get; init; } = new();
    /// <summary>Tâche à ouvrir dans le panneau de détail (ex. tâche créée).</summary>
    public string? Ouvrir { get; init; }
}

/// <summary>
/// Message court affiché après une saisie. « Action » propose une suite (ex. « valider-tache », « bilan-ia ») sur « Cible ».
/// </summary>
public sealed record MessageDto(string Texte, string Ton = "info", string? Action = null, string? Cible = null, string? LibelleAction = null);

