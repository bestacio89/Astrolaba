using Franz.Common.Mediator.Messages;

namespace PilotageForfaits.Contracts.Projets;

/// <summary>Liste les projets visibles, éventuellement archivés.</summary>
public sealed record ListerProjetsQuery(bool Archives) : IQuery<IReadOnlyList<ProjetResumeDto>>;
