# Microsoft Entra ID authentication and project authorization

## Trust boundaries

- Microsoft Entra ID authenticates the caller and issues the API access token.
- The API validates the bearer token through Microsoft.Identity.Web against the configured tenant and API audience.
- Entra group object IDs, not group display names and not application-role claims, are the source of the user's configured view type.
- Astrolaba's PostgreSQL database remains the source of project assignment. Group membership alone never grants access to a project.
- The API enforces the project assignment and the group-derived role on every operation that calls the application access service. Angular screen visibility is not an authorization boundary.

## Required deployment configuration

Set these values through the deployment environment, secret store, or another non-committed configuration provider:

- AzureAd__TenantId: the single Entra tenant ID.
- AzureAd__ClientId: the API application's client ID.
- AzureAd__Audience: the API's configured Application ID URI/audience (for example, the API URI registered in Entra ID).

The API intentionally fails at startup outside the test environment if these values are blank or still contain template placeholders. Do not put tenant-specific values or credentials in the committed settings file.

Configure the group object IDs under Authentification:GroupesEntra:

- Consultant
- ChefDeProjet
- Pilotage
- Lecteur
- Administrateur

Each entry is an array of Entra group object IDs. In environment-variable configuration, indexed entries use names such as Authentification__GroupesEntra__ChefDeProjet__0. Empty arrays grant no corresponding role. The current mapping covers Astrolaba's existing RoleProjet values; new views such as Developer or Architect must be added deliberately after the screen/permission matrix is agreed with the frontend developer.

Entra ID must be configured to emit group membership in the API access token for the relevant security groups. The backend consumes the immutable groups claim (including its standard URI claim-type form where applicable). It does not query Microsoft Graph. If Entra emits a group-overage token without the actual group IDs, the caller will not match a configured group and authorization fails closed; Graph-based overage resolution is a separate future task if the tenant's group count requires it.

## Authorization rules

1. The token must be valid for the configured tenant and API audience.
2. The user must have exactly one configured project-view role. If group membership maps to zero or multiple view roles, project access is denied until the mapping is unambiguous.
3. The user must have an explicit Astrolaba assignment (Habilitations) for the requested project.
4. The effective view role comes from the configured Entra group. The legacy role value stored on the assignment is not used to override group membership.
5. Project-level business rules (read-only archived projects, mutation restrictions, financial visibility) remain enforced by the application access service.
6. Global group membership does not bypass project assignment. The existing VoitTousLesProjets response field is retained for contract compatibility and returns false.

## Local development

The Development environment keeps a dedicated fake identity provider, enabled only when Authentification:Mode is Dev. Its configured identities emit synthetic groups claims whose values are mapped in appsettings.Development.json. This does not create a bypass in production: Mode=Dev is rejected outside Development and Tests, and a request without X-Utilisateur-Dev remains anonymous.

## Operational checklist

- Register an API application in Entra ID and expose the intended API audience.
- Configure the tenant ID, API client ID and audience using deployment configuration.
- Enable the required group claims for the API access token.
- Replace the empty production group-ID arrays with the correct group object IDs.
- Ensure each user is explicitly assigned to the relevant Astrolaba project in PostgreSQL.
- Run the API against PostgreSQL and the real production startup pipeline before considering authentication rollout complete.
