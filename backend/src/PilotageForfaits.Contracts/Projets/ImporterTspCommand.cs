using Franz.Common.Mediator.Messages;

namespace PilotageForfaits.Contracts.Projets;

/// <summary>Commande d'import d'un TSP déjà ouvert par la couche HTTP.</summary>
public sealed record ImporterTspCommand(Stream Fichier, string NomFichier) : ICommand<Guid>;
