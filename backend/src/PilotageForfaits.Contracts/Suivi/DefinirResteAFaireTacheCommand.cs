using Franz.Common.Mediator.Messages;

namespace PilotageForfaits.Contracts.Suivi;

/// <summary>Commande de saisie sur une tâche du suivi.</summary>
public sealed record DefinirResteAFaireTacheCommand(Guid ProjetId, string Uid, ResteAFaireCommande Donnees) : ICommand<ResultatSaisieDto>;
