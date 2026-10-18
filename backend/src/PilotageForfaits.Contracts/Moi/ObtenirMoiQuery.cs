using Franz.Common.Mediator.Messages;

namespace PilotageForfaits.Contracts.Moi;

/// <summary>Obtient l'identité et les capacités de l'utilisateur connecté.</summary>
public sealed record ObtenirMoiQuery : IQuery<MoiDto>;
