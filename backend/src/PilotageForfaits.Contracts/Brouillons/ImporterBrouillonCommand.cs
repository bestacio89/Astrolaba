using Franz.Common.Mediator.Messages;

namespace PilotageForfaits.Contracts.Brouillons;

/// <summary>Importe un TSP comme brouillon à relire avant lancement.</summary>
public sealed record ImporterBrouillonCommand(Stream Fichier, string NomFichier) : ICommand<Guid>;
