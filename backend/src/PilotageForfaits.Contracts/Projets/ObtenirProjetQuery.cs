using Franz.Common.Mediator.Messages;

namespace PilotageForfaits.Contracts.Projets;

/// <summary>Obtient la vue d'ensemble d'un projet après contrôle des habilitations.</summary>
public sealed record ObtenirProjetQuery(Guid ProjetId) : IQuery<ProjetDetailDto>;
