using Franz.Common.Mediator.Messages;

namespace PilotageForfaits.Contracts.Suivi;

/// <summary>Commande de modification du périmètre d'un projet.</summary>
public sealed record AjouterProfilTacheCommand(Guid ProjetId, string Uid, AjoutProfilCommande Donnees) : ICommand<ResultatSaisieDto>;
