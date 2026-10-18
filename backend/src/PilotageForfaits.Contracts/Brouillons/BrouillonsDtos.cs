namespace PilotageForfaits.Contracts.Brouillons;

/// <summary>Une ligne de la page « Mes brouillons ».</summary>
public sealed record BrouillonResumeDto(
    Guid Id,
    string Client,
    string Nom,
    string? Reference,
    string Origine,
    string OrigineCode,
    DateTimeOffset ModifieLe,
    string? ModifiePar,
    string Createur,
    string? CoRedacteur,
    bool EstCreateur);

/// <summary>Personne affichée dans les informations de collaboration d'un brouillon.</summary>
public sealed record PersonneDto(Guid Id, string NomComplet, string Email);

/// <summary>Co-rédacteur après invitation ou retrait, et nouvelle version du brouillon.</summary>
public sealed record CoRedacteurReponse(PersonneDto? CoRedacteur, int Version);
