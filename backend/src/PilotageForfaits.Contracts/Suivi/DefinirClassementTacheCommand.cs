using Franz.Common.Mediator.Messages;

namespace PilotageForfaits.Contracts.Suivi;

/// <summary>Commande de saisie sur une tâche du suivi.</summary>
public sealed record DefinirClassementTacheCommand(Guid ProjetId, string Uid, ClassementCommande Donnees) : ICommand<ResultatSaisieDto>;
