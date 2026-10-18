using Franz.Common.Mediator.Messages;

namespace PilotageForfaits.Contracts.Suivi;

/// <summary>Commande de saisie sur une tâche du suivi.</summary>
public sealed record DefinirLibelleTacheCommand(Guid ProjetId, string Uid, LibelleCommande Donnees) : ICommand<ResultatSaisieDto>;
