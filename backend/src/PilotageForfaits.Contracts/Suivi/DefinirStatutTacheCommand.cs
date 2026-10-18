using Franz.Common.Mediator.Messages;

namespace PilotageForfaits.Contracts.Suivi;

/// <summary>Commande de saisie sur une tâche du suivi.</summary>
public sealed record DefinirStatutTacheCommand(Guid ProjetId, string Uid, StatutCommande Donnees) : ICommand<ResultatSaisieDto>;
