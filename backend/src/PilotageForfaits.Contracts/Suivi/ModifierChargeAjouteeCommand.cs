using Franz.Common.Mediator.Messages;

namespace PilotageForfaits.Contracts.Suivi;

/// <summary>Commande de modification du périmètre d'un projet.</summary>
public sealed record ModifierChargeAjouteeCommand(Guid ProjetId, string Uid, Guid ProfilId, ChargeAjouteeCommande Donnees) : ICommand<ResultatSaisieDto>;
