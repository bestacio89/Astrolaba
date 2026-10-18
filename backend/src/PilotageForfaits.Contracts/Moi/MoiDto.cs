namespace PilotageForfaits.Contracts.Moi;

/// <summary>Informations de l'utilisateur connecté et capacités utilisées par l'interface.</summary>
public sealed record MoiDto(Guid Id, string Prenom, string Nom, string Email, IReadOnlyList<string> Roles, bool PeutCreerUnProjet, bool VoitTousLesProjets);
