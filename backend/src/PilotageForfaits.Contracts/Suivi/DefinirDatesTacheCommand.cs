using Franz.Common.Mediator.Messages;

namespace PilotageForfaits.Contracts.Suivi;

/// <summary>Commande de saisie sur une tâche du suivi.</summary>
public sealed record DefinirDatesTacheCommand(Guid ProjetId, string Uid, DatesTacheCommande Donnees) : ICommand<ResultatSaisieDto>;
