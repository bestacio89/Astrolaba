using Franz.Common.Mediator.Messages;

namespace PilotageForfaits.Contracts.Suivi;

/// <summary>Liste IA à modifier dans les paramètres du projet.</summary>
public enum ListeIaType { Levier, Cause }

/// <summary>Commande de mutation du suivi de projet.</summary>
public sealed record DefinirDateAtelierCommand(Guid ProjetId, string Uid, AtelierDateCommande Donnees) : ICommand<ResultatSaisieDto>;

/// <summary>Commande de mutation du suivi de projet.</summary>
public sealed record DefinirHoraireAtelierCommand(Guid ProjetId, string Uid, AtelierHoraireCommande Donnees) : ICommand<ResultatSaisieDto>;

/// <summary>Commande de mutation du suivi de projet.</summary>
public sealed record MarquerInvitationAtelierCommand(Guid ProjetId, string Uid, InvitationCommande Donnees) : ICommand<ResultatSaisieDto>;

/// <summary>Commande de mutation du suivi de projet.</summary>
public sealed record DefinirDateInvitationAtelierCommand(Guid ProjetId, string Uid, AtelierDateCommande Donnees) : ICommand<ResultatSaisieDto>;

/// <summary>Commande de mutation du suivi de projet.</summary>
public sealed record DefinirDestinatairesAtelierCommand(Guid ProjetId, string Uid, DestinatairesCommande Donnees) : ICommand<ResultatSaisieDto>;

/// <summary>Commande de mutation du suivi de projet.</summary>
public sealed record DefinirValidationClientLivrableCommand(Guid ProjetId, string Uid, ValidationClientCommande Donnees) : ICommand<ResultatSaisieDto>;

/// <summary>Commande de mutation du suivi de projet.</summary>
public sealed record DefinirLienLivrableCommand(Guid ProjetId, string Uid, LienLivrableCommande Donnees) : ICommand<ResultatSaisieDto>;

/// <summary>Commande de mutation du suivi de projet.</summary>
public sealed record EnregistrerPointHebdoSemaineCommand(Guid ProjetId, string CleSemaine, PointHebdoCommande Donnees) : ICommand<ResultatSaisieDto>;

/// <summary>Commande de mutation du suivi de projet.</summary>
public sealed record EnregistrerPointHebdoMoisCommand(Guid ProjetId, string CleMois, PointHebdoCommande Donnees) : ICommand<ResultatSaisieDto>;

/// <summary>Commande de mutation du suivi de projet.</summary>
public sealed record ReglerPointHebdoCommand(Guid ProjetId, string Champ, ReglagePointHebdoCommande Donnees) : ICommand<ResultatSaisieDto>;

/// <summary>Commande de mutation du suivi de projet.</summary>
public sealed record AjouterMiseAJourTacheCommand(Guid ProjetId, string Uid, MiseAJourCommande Donnees) : ICommand<ResultatSaisieDto>;

/// <summary>Commande de mutation du suivi de projet.</summary>
public sealed record FigerReferenceProjetCommand(Guid ProjetId, FigerReferenceCommande Donnees) : ICommand<ResultatSaisieDto>;

/// <summary>Commande de mutation du suivi de projet.</summary>
public sealed record DefinirDeriveReferenceCommand(Guid ProjetId, Guid ReferenceId, string Uid, DeriveCommande Donnees) : ICommand<ResultatSaisieDto>;

/// <summary>Commande de mutation du suivi de projet.</summary>
public sealed record DefinirDateEcheanceCommand(Guid ProjetId, int Numero, EcheanceDateCommande Donnees) : ICommand<ResultatSaisieDto>;

/// <summary>Commande de mutation du suivi de projet.</summary>
public sealed record LierPerimetreEcheanceCommand(Guid ProjetId, int Numero, PerimetreEcheanceCommande Donnees) : ICommand<ResultatSaisieDto>;

/// <summary>Commande de mutation du suivi de projet.</summary>
public sealed record DefinirEcheancePhaseCommand(Guid ProjetId, string Code, EcheancePhaseCommande Donnees) : ICommand<ResultatSaisieDto>;

/// <summary>Commande de mutation du suivi de projet.</summary>
public sealed record CreerActionRegistreCommand(Guid ProjetId) : ICommand<ResultatSaisieDto>;

/// <summary>Commande de mutation du suivi de projet.</summary>
public sealed record ModifierActionRegistreCommand(Guid ProjetId, Guid ActionId, ChampCommande Donnees) : ICommand<ResultatSaisieDto>;

/// <summary>Commande de mutation du suivi de projet.</summary>
public sealed record RestaurerActionRegistreCommand(Guid ProjetId, Guid ActionId, RestaurerActionCommande Donnees) : ICommand<ResultatSaisieDto>;

/// <summary>Commande de mutation du suivi de projet.</summary>
public sealed record SupprimerActionRegistreCommand(Guid ProjetId, Guid ActionId) : ICommand<ResultatSaisieDto>;

/// <summary>Commande de mutation du suivi de projet.</summary>
public sealed record CreerDecisionRegistreCommand(Guid ProjetId) : ICommand<ResultatSaisieDto>;

/// <summary>Commande de mutation du suivi de projet.</summary>
public sealed record ModifierDecisionRegistreCommand(Guid ProjetId, Guid DecisionId, ChampCommande Donnees) : ICommand<ResultatSaisieDto>;

/// <summary>Commande de mutation du suivi de projet.</summary>
public sealed record SupprimerDecisionRegistreCommand(Guid ProjetId, Guid DecisionId) : ICommand<ResultatSaisieDto>;

/// <summary>Commande de mutation du suivi de projet.</summary>
public sealed record CreerRisqueRegistreCommand(Guid ProjetId) : ICommand<ResultatSaisieDto>;

/// <summary>Commande de mutation du suivi de projet.</summary>
public sealed record ModifierRisqueRegistreCommand(Guid ProjetId, Guid RisqueId, ChampCommande Donnees) : ICommand<ResultatSaisieDto>;

/// <summary>Commande de mutation du suivi de projet.</summary>
public sealed record SupprimerRisqueRegistreCommand(Guid ProjetId, Guid RisqueId) : ICommand<ResultatSaisieDto>;

/// <summary>Commande de mutation du suivi de projet.</summary>
public sealed record CreerAchatReelCommand(Guid ProjetId, NouvelAchatCommande Donnees) : ICommand<ResultatSaisieDto>;

/// <summary>Commande de mutation du suivi de projet.</summary>
public sealed record ModifierAchatReelCommand(Guid ProjetId, Guid AchatId, ChampCommande Donnees) : ICommand<ResultatSaisieDto>;

/// <summary>Commande de mutation du suivi de projet.</summary>
public sealed record SupprimerAchatReelCommand(Guid ProjetId, Guid AchatId) : ICommand<ResultatSaisieDto>;

/// <summary>Commande de mutation du suivi de projet.</summary>
public sealed record CreerFraisReelCommand(Guid ProjetId, NouveauFraisCommande Donnees) : ICommand<ResultatSaisieDto>;

/// <summary>Commande de mutation du suivi de projet.</summary>
public sealed record ModifierFraisReelCommand(Guid ProjetId, Guid FraisId, ChampCommande Donnees) : ICommand<ResultatSaisieDto>;

/// <summary>Commande de mutation du suivi de projet.</summary>
public sealed record SupprimerFraisReelCommand(Guid ProjetId, Guid FraisId) : ICommand<ResultatSaisieDto>;

/// <summary>Commande de mutation du suivi de projet.</summary>
public sealed record DefinirRegroupementProjetCommand(Guid ProjetId, RegroupementCommande Donnees) : ICommand<ResultatSaisieDto>;

/// <summary>Commande de mutation du suivi de projet.</summary>
public sealed record DefinirAlerteProjetCommand(Guid ProjetId, AlerteCommande Donnees) : ICommand<ResultatSaisieDto>;

/// <summary>Commande de mutation du suivi de projet.</summary>
public sealed record DefinirProfilIaProjetCommand(Guid ProjetId, ProfilIaCommande Donnees) : ICommand<ResultatSaisieDto>;

/// <summary>Commande de mutation du suivi de projet.</summary>
public sealed record DefinirListeIaProjetCommand(Guid ProjetId, ListeIaType Type, ListeIaCommande Donnees) : ICommand<ResultatSaisieDto>;

/// <summary>Commande de mutation du suivi de projet.</summary>
public sealed record DefinirDatesProjetCommand(Guid ProjetId, ChampCommande Donnees) : ICommand<ResultatSaisieDto>;

/// <summary>Commande de mutation du suivi de projet.</summary>
public sealed record NommerAxeProjetCommand(Guid ProjetId, string Code, NomAxeCommande Donnees) : ICommand<ResultatSaisieDto>;

/// <summary>Commande de mutation du suivi de projet.</summary>
public sealed record DefinirFicheProjetCommand(Guid ProjetId, FicheCommande Donnees) : ICommand<ResultatSaisieDto>;

/// <summary>Commande de mutation du suivi de projet.</summary>
public sealed record AjouterTypeProjetCommand(Guid ProjetId, TypeCommande Donnees) : ICommand<ResultatSaisieDto>;

/// <summary>Commande de mutation du suivi de projet.</summary>
public sealed record RenommerTypeProjetCommand(Guid ProjetId, string Code, TypeCommande Donnees) : ICommand<ResultatSaisieDto>;

/// <summary>Commande de mutation du suivi de projet.</summary>
public sealed record SupprimerTypeProjetCommand(Guid ProjetId, string Code) : ICommand<ResultatSaisieDto>;
