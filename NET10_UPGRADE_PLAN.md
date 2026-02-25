# Implementation Plan - Upgrade to .NET 10

This plan outlines the steps required to upgrade the `play-microservices` workspace from .NET 9 to .NET 10.

## User Review Required

> [!IMPORTANT]
> .NET 10 is a future/preview release. Ensure you have the .NET 10 SDK installed before proceeding with builds.

- **TargetFramework**: All projects will be updated from `net9.0` to `net10.0`.
- **Package Updates**: Official Microsoft packages (AspNetCore, Extensions) will be updated to `10.x` versions.
- **Aspire Update**: .NET Aspire hosting packages will be updated to compatible versions if available.

## Proposed Changes

### Project Files (.csproj)
1.  **Play.AppHost**: Update TFM to `net10.0` and update Aspire packages.
2.  **Play.Catalog**: Update TFM to `net10.0` and update Microsoft packages to `10.0.x`.
3.  **Play.Identity**: Update TFM to `net10.0` and update Microsoft packages. Check Duende IdentityServer compatibility.
4.  **Play.Payment**: Update TFM to `net10.0` and update Microsoft packages.
5.  **Play.ServiceDefaults**: Update TFM to `net10.0` and update Microsoft.Extensions and OpenTelemetry packages.
6.  **Play.Catalog.Tests**: Update TFM to `net10.0` and update test-related packages.

## Verification Plan

### Automated Tests
- Run `dotnet restore` to verify package compatibility.
- Run `dotnet build` to check for compilation errors.
- Run `dotnet test` (specifically for `Play.Catalog.Tests`) to ensure logic remains intact.

### Manual Verification
- Verify the Aspire Dashboard launches and all services are listed as `net10.0` where applicable (though runtime might still show the installed SDK version).
