namespace PilotageForfaits.Contracts.Import;

/// <summary>Ce que l'import va reprendre, montré avant de confirmer.</summary>
public sealed record ApercuImportDto(
    string NomFichier,
    string Client,
    string Projet,
    string? Reference,
    DateOnly? Debut,
    DateOnly? FinContractuelle,
    int NombreTaches,
    int NombreProfils,
    int NombreEcheances,
    int NombreAxes,
    string? SourcePlanning,
    bool BudgetsAchatsFrais,
    IReadOnlyList<string> PointsReleves,
    // projet qui suit déjà ce forfait (l'import sera refusé), ou null
    ProjetExistantDto? ProjetExistant);

