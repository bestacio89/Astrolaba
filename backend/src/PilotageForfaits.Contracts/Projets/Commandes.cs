namespace PilotageForfaits.Contracts.Projets;

/// <summary>Archiver (true) ou restaurer (false) un projet.</summary>
public sealed record ArchivageCommande(bool? Archive);

/// <summary>Suppression définitive : le nom du projet, retapé par l'utilisateur, confirme qu'il sait ce qu'il supprime.</summary>
public sealed record SuppressionCommande(string? Confirmation);

/// <summary>Demande de création d'un projet vide (V1 : la fiche seule).</summary>
public sealed record CreerProjetVideCommande(
    string Client,
    string Nom,
    string? Reference,
    DateOnly Debut,
    DateOnly FinContractuelle,
    string? TypeContrat);
