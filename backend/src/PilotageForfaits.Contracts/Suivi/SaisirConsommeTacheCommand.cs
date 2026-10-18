using Franz.Common.Mediator.Messages;

namespace PilotageForfaits.Contracts.Suivi;

/// <summary>Commande de saisie sur une tâche du suivi.</summary>
public sealed record SaisirConsommeTacheCommand(Guid ProjetId, string Uid, ConsommeCommande Donnees) : ICommand<ResultatSaisieDto>;
