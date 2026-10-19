# Astrolabe — pilotage des forfaits

Application du groupe Magellan (Exakis Nelite) pour suivre les projets au forfait, du TSP vendu à l'atterrissage. Dans le code, le nom technique reste `PilotageForfaits` (solution, projets, base de données) : seul le nom affiché a changé.

Application de pilotage des projets au forfait : à partir du TSP vendu, elle suit la charge, le consommé, le reste à faire, l'atterrissage, la marge, la facturation, les ateliers et livrables, la gouvernance et l'optimisation IA.

Elle remplace le prototype (POC) en HTML. La **version de référence du POC (v65, 06/10/2026)** sert de spécification : les calculs et la lecture du TSP en sont une traduction fidèle, vérifiée par des tests automatiques.

---

## Ce que contient cette première version

| Domaine | Contenu |
|---|---|
| Socle | Architecture en couches, base de données complète (modèle de données conceptuel), connexion par compte d'entreprise, rôles, gestion des erreurs, intégration continue |
| Moteur de calcul | Tous les indicateurs du POC : reste à faire profil par profil, atterrissage, écarts, enveloppe IA, coûts, marges, échéancier et FAE, achats et frais, points de vigilance… |
| Import d'un TSP | Même lecture que le POC (mêmes colonnes repérées, mêmes valeurs par défaut, mêmes points relevés) |
| Créer un projet | **Nouveau projet** : importer un TSP Excel, le **rédiger dans l'application** ou repartir d'un projet existant. Le projet reste un **brouillon privé** (auteur et co-rédacteur invité), enregistré au fil de la saisie, avec les chiffres du TSP recalculés en direct, des contrôles avant le lancement et l'**export Excel** du TSP |
| Écrans | Page d'accueil « Mes projets » (brouillons, liste selon les habilitations, points à traiter, premier lancement) |
| Suivi d'un projet | Tous les écrans du POC après l'import du TSP, avec leurs règles de gestion : tableau de bord et « À traiter », workload (liste des tâches, saisie du consommé, optimisation IA), Gantt, ateliers et livrables, actions / décisions / risques, plan de charge, facturation, achats, frais, fiche, synthèse, profils, chiffrage, annexes (planning, métrique, ventilation), paramètres ; panneaux de détail d'une tâche et d'une phase ; exports COPIL et COPROJ |
| Piloter l'agence et l'équipe (réunion du 5 octobre) | **Portefeuille** (point du lundi : tous les projets, les plus fragiles d'abord) ; **Mon travail** (ce qui m'est confié, ma saisie de la semaine, notifications dans l'application) ; **Rentabilité** (de la marge vendue à la marge à terminaison, simulateur de dépense) ; **planning de référence** figé et journal des dérives avec cause et motif (Gantt) ; **document du livrable** et **fil de mises à jour** sur chaque tâche ; **capture datée** du tableau de bord, en PDF |
| À venir | Administration (habilitations, utilisateurs, lien personne ↔ compte) ; envoi des notifications par e-mail ou Teams ; statut des avenants « à refacturer » (règle de calcul à valider) |

---

## Architecture en bref

```
┌──────────────────────────── Interface (Angular) ─────────────────────────────┐
│  core/      connexion, gardes par rôle, barre latérale, HTTP, erreurs         │
│  shared/    composants communs (fenêtre, icônes), formats € / JH / dates      │
│  features/  chargés à la demande :                                            │
│             projets (accueil, import)  ·  suivi (tous les onglets d'un projet)│
│             portefeuille  ·  travail (« Mon travail »)  ·  administration      │
│             (pages → composants d'affichage → état de l'écran → client API)   │
└───────────────────────────────────┬───────────────────────────────────────────┘
                                    │ HTTPS + jeton Entra ID (REST / JSON)
┌───────────────────────────────────▼───────────── Serveur (.NET) ─────────────┐
│  1. Api             points d'entrée REST, validation, authentification, erreurs│
│  2. Application     cas d'usage (services), droits par rôle, transactions,     │
│                     journal                                                    │
│  3. Domaine         entités, règles métier, MOTEUR DE CALCUL, lecture du TSP   │
│  4. Infrastructure  base PostgreSQL (EF Core + Franz.Common), lecture des fichiers Excel      │
└───────────────────────────────────┬───────────────────────────────────────────┘
                                    │
                          PostgreSQL (base relationnelle)
```

Règle d'or : **les dépendances vont de haut en bas uniquement**. Le Domaine ne connaît ni la base, ni le web : c'est ce qui le rend testable et durable. Détails et choix : [docs/architecture.md](docs/architecture.md).

---

## Démarrer en local

### Prérequis

| Outil | Version | Rôle |
|---|---|---|
| [.NET SDK](https://dotnet.microsoft.com/download) | 10 (LTS) | serveur |
| [Node.js](https://nodejs.org) | 22.12 ou plus (LTS) — Angular refuse les versions plus anciennes | interface |
| [Docker Desktop](https://www.docker.com/products/docker-desktop/) | récent | PostgreSQL 17 local via Docker Compose |
| Git | récent | |

Les versions sont **à confirmer avec l'équipe** : voir [docs/a-valider.md](docs/a-valider.md).

### 1. Base de données

La base locale est **PostgreSQL 17**. Démarrez-la avec Docker Compose depuis la racine du dépôt :

```bash
docker compose up -d
```

La chaîne de connexion n'est pas stockée dans les fichiers de configuration suivis par Git. Pour le développement local, définissez-la avec User Secrets (depuis la racine du dépôt) :

```bash
dotnet user-secrets set "ConnectionStrings:Pilotage" "Host=localhost;Port=5432;Database=PilotageForfaits;Username=postgres;Password=Dev_Pilotage_2026!;Include Error Detail=true" --project backend/src/PilotageForfaits.Api
```

Cette valeur correspond uniquement au compte du conteneur PostgreSQL local défini dans `docker-compose.yml`. Ne réutilisez jamais ces identifiants en recette ou en production. En déploiement, fournissez `ConnectionStrings__Pilotage` via l'environnement ou un gestionnaire de secrets.

Une seule fois, faites confiance au certificat HTTPS de développement : `dotnet dev-certs https --trust`.

### 2. Serveur (API)

```bash
cd backend
dotnet build
dotnet run --project src/PilotageForfaits.Api
```

L'API écoute sur `https://localhost:5081` (et `http://localhost:5080`, redirigé vers HTTPS). Au démarrage en environnement Development, elle applique les migrations EF Core à la base PostgreSQL de développement.

> Le dépôt contient une migration initiale PostgreSQL. L'API applique les migrations en environnement Development et échoue au démarrage si le modèle a changé sans migration revue. Une base SQL Server existante n'est pas convertie automatiquement : la migration du fournisseur nécessite une base PostgreSQL neuve. Pour réinitialiser uniquement les données locales, arrêtez l'API puis exécutez `docker compose down -v` et `docker compose up -d` (cette opération supprime les données du volume local).

Test rapide : `https://localhost:5081/api/sante`. Le fichier `PilotageForfaits.Api.http` contient des requêtes d'essai.

En développement, **pas de connexion Entra ID** : trois utilisateurs fictifs sont déclarés dans `appsettings.Development.json` (chef de projet, consultant, pilotage). On en change depuis l'en-tête de l'interface (« Utilisateur de test »). Ce mode est refusé au démarrage hors développement.

### 3. Interface (Angular)

```bash
cd frontend
npm install
npm start
```

Ouvrir `http://localhost:4200`. Les appels `/api` sont redirigés vers l'API locale (`proxy.conf.json`).
Après le premier `npm install`, enregistrez le fichier `package-lock.json` créé dans Git (installations identiques pour tous).
Pas besoin d'Angular CLI installé globalement : `npm start`, `npm run build` et `npm test` utilisent la version du projet.

Le fichier `frontend/.npmrc` contourne un défaut de npm 10 (erreur « Cannot read properties of null (reading 'edgesOut') ») : un simple `npm install` suffit. Si une installation précédente a laissé des restes, supprimez `node_modules` et `package-lock.json` dans `frontend`, puis relancez `npm install`.

Premier essai : « Importer un TSP » avec `backend/tests/Donnees/tsp-exemple-a.xlsx` (TSP anonymisé).

---

## Tests

```bash
cd backend && dotnet test      # moteur, lecture du TSP, API de bout en bout
cd frontend && npm test        # logique des écrans
```

| Tests | Ce qu'ils garantissent |
|---|---|
| `Domain.Tests` › moteur de référence | Sur 6 scénarios (TSP importé, suivi complet, regroupements, TSP sans dates, projet vide), le moteur C# donne **exactement** les résultats du moteur du POC |
| `Domain.Tests` › règles | Validé ⇔ 100 %, 1 JH par personne et par jour, montant figé à la facturation, enveloppe IA… |
| `Infrastructure.Tests` | La lecture des deux TSP d'exemple donne exactement les mêmes données que le POC |
| `Api.Tests` | Import d'un TSP → base → moteur = mêmes indicateurs que le POC ; habilitations ; montants jamais envoyés à un rôle qui ne doit pas les voir |

Les résultats attendus sont produits par le moteur du POC lui-même (`outils/reference-poc`), et l'intégration continue vérifie qu'ils n'ont pas été modifiés à la main.

---

## Migrations EF Core

Les migrations de schéma sont versionnées dans `backend/src/PilotageForfaits.Infrastructure/Migrations`. Après toute modification du modèle, créez puis relisez une migration :

```bash
./scripts/creer-migration.sh NomDeLEvolution        # macOS / Linux
.\\scripts\\creer-migration.ps1 -Nom NomDeLEvolution  # Windows
```

Les migrations sont appliquées automatiquement uniquement en environnement Development. Les déploiements supérieurs doivent appliquer les migrations via leur procédure de livraison, après revue. Ne supprimez pas l'historique de migrations pour une évolution normale. Les scripts `reinitialiser-base` sont destructifs et réservés à une remise à zéro complète de l'environnement local.

---

## Organisation du dépôt

```
backend/                  serveur .NET (solution PilotageForfaits.sln)
  src/                    Api · Application · Domain · Infrastructure
  tests/                  tests + Donnees (scénarios de référence, TSP anonymisés)
frontend/                 interface Angular
docs/                     architecture, règles métier, points à valider
outils/reference-poc/     moteur du POC v65 et génération des résultats attendus
scripts/                  migrations de base de données
.github/workflows/        intégration continue (GitHub Actions) — azure-pipelines.yml pour Azure DevOps
```

## Règles de travail

- Une modification = une branche + une demande de fusion ; on ne fusionne que si l'intégration continue est verte.
- Toute règle de calcul modifiée doit l'être d'abord dans les scénarios de référence (voir `outils/reference-poc/LISEZMOI.md`).
- **Jamais de données client réelles dans le dépôt** : les TSP de test sont anonymisés (`outils/reference-poc/anonymise.py`).
- Pas de secret dans le code : en recette et en production, la chaîne de connexion et les identifiants Entra ID passent par la configuration de l'hébergement.

## Documentation

- [docs/architecture.md](docs/architecture.md) — couches, flux, choix techniques
- [docs/regles-metier.md](docs/regles-metier.md) — règles du POC et où elles sont garanties
- [docs/design-system.md](docs/design-system.md) — couleurs, statuts, typographie, composants, navigation
- [docs/correspondance-poc-api-base.md](docs/correspondance-poc-api-base.md) — chaque écran du POC : où il est, quels appels, quelles tables
- [docs/a-valider.md](docs/a-valider.md) — versions, hébergement, Entra ID, droits par rôle
- [docs/etat-de-la-livraison.md](docs/etat-de-la-livraison.md) — ce qui a été vérifié avant la livraison
