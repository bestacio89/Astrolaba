namespace PilotageForfaits.Contracts.Brouillons;

/// <summary>Demande de création d'un brouillon (vierge, ou repris d'un projet existant).</summary>
public sealed record CreationBrouillonCommande(string? Client, string? Projet, string? Reference, Guid? ProjetSource);

/// <summary>Invitation d'un co-rédacteur par son adresse e-mail (vide : retirer le co-rédacteur).</summary>
public sealed record CoRedacteurCommande(string? Email);

/// <summary>Lancement : la version lue (on lance exactement ce que l'utilisateur a vu).</summary>
public sealed record LancementCommande(int Version);
