using Franz.Common.Mediator.Messages;

namespace PilotageForfaits.Contracts.Suivi;

/// <summary>Commande de saisie sur une tâche du suivi.</summary>
public sealed record DefinirCommentaireTacheCommand(Guid ProjetId, string Uid, CommentaireCommande Donnees) : ICommand<ResultatSaisieDto>;
