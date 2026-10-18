using Franz.Common.Mediator.Messages;

namespace PilotageForfaits.Contracts.Brouillons;

/// <summary>Liste les brouillons ouverts par l'utilisateur connecté.</summary>
public sealed record ListerBrouillonsQuery : IQuery<IReadOnlyList<BrouillonResumeDto>>;
