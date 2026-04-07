# Copilot instructions

## Build, test, and validation

- Treat `Projects\build-aotbrowser.yml` as the source of truth for CI. The pipeline builds `Projects\BuildSolution\BuildSolution.sln`; `Projects\AOTBrowser\AOTBrowser.sln` is the local developer solution with the same X++ model plus the support library.
- Restore the D365FO build tool packages and C# NuGet packages before building:

```powershell
nuget install "Projects\packages.config" -ExcludeVersion -OutputDirectory "<NuGetsPath>" -ConfigFile "Projects\nuget.config"
nuget restore "Projects\BuildSolution\BuildSolution.sln"
```

- Full build, following the same shape as CI:

```powershell
msbuild "Projects\BuildSolution\BuildSolution.sln" /p:BuildTasksDirectory="<NuGetsPath>\Microsoft.Dynamics.AX.Platform.CompilerPackage\DevAlm" /p:MetadataDirectory="<repo>\Metadata" /p:FrameworkDirectory="<NuGetsPath>\Microsoft.Dynamics.AX.Platform.CompilerPackage" /p:ReferenceFolder="<NuGetsPath>\Microsoft.Dynamics.AX.Platform.DevALM.BuildXpp\ref\net40;<NuGetsPath>\Microsoft.Dynamics.AX.Application.DevALM.BuildXpp\ref\net40;<NuGetsPath>\Microsoft.Dynamics.AX.ApplicationSuite.DevALM.BuildXpp\ref\net40;<repo>\Metadata;<BinariesPath>" /p:ReferencePath="<NuGetsPath>\Microsoft.Dynamics.AX.Platform.CompilerPackage" /p:OutputDirectory="<BinariesPath>"
```

- If you only changed the C# helper library, build it directly:

```powershell
msbuild "Projects\AOTBrowser\Arbela.Dynamics.AX.Xpp.Support\Arbela.Dynamics.AX.Xpp.Support.csproj" /p:Configuration=Debug
```

- For local onebox development, mount the repo metadata into `PackagesLocalDirectory` with:

```powershell
powershell -ExecutionPolicy Bypass -File "Scripts\Mount.ps1"
```

  Run it as admin with Visual Studio closed. The script creates symbolic links for each model under `Metadata\` and restarts the D365FO environment.

- No automated unit-test, single-test, or lint command is checked in. Validation is feature-level in a D365FO environment: repopulate AOT data, then exercise the AOT browser form, the form-personalization entry point, the data-entity entry point, table browser, source-code toggle, inline extensions toggle, and jump-to-reference flows.

## High-level architecture

- This repository is a D365FO ISV model plus a C# interop library. The checked-in source of truth is under `Metadata\AOTBrowser\AOTBrowser\Ax*`; the `.rnrproj` files in `Projects\` are Visual Studio/build wrappers around that metadata.
- The main product surface is `Metadata\AOTBrowser\AOTBrowser\AxForm\ARBAOTBrowser.xml`. It builds a tree/detail view for a selected AOT object and switches behavior by object type through `ARBAOTObjectTree`.
- `ARBAOTObjectTree` is the core metadata-browser engine. It reads metadata through Dynamics metadata providers, builds the tree UI, merges extensions into the tree when requested, and preserves selection with tree paths.
- Object-type-specific tree behavior is split into subclasses such as `ARBAOTObjectTreeForm` and `ARBAOTObjectTreeTable`. Those subclasses handle details like form-control selection and relation rendering that do not belong in the base class.
- `ARBAOTObjectPopulateService` populates `ARBAOTObjects`, which is the persistent cache of browsable AOT root objects. `ARBEntityFieldPopulateService` populates `ARBEntityFields`, and `ARBMenuItemRefPopulateService` populates `ARBMenuItemReferences`. Those two tables drive the "open from entity field" and "jump from menu item" workflows.
- `ARBAOTObjectProperties` and `ARBAOTObjectPropertiesMatrix` are `InMemory` tables used for the current selection and comparison-style details grid; they are not persistent metadata storage.
- The C# project in `Projects\AOTBrowser\Arbela.Dynamics.AX.Xpp.Support\` fills gaps in the X++ metadata APIs. `MetadataSupport.cs` exposes extension and source-text helpers, while `HtmlFormatHelper.cs` and `XPlusPlusLexer.cs` provide syntax-highlighted X++ rendering for the source-code pane.
- Entry points are intentionally spread across menu extensions and form extensions: `CommonMenu.AOTBrowser`, `navpanemenu.AOTBrowser`, `DMFEntity.AOTBrowser`, `FormControlPersonalization.AOTBrowser`, and `FormControlPersonalizationForm_ARB_Extension`. Changes to launch/navigation behavior usually touch more than one artifact.
- The model descriptor is `Metadata\AOTBrowser\Descriptor\AOTBrowser.xml`. It allows customization and references `ApplicationFoundation`, `ApplicationPlatform`, `ApplicationSuite`, and `GeneralLedger`.

## Key conventions

- X++ artifacts use the `ARB` / `ARBAOT` prefix. The helper library namespaces are `Arbela.Dynamics.AX.Xpp.Support` and `Arbela.Dynamics.Ax.Xpp`.
- Add object-type-specific browser behavior by creating an `ARBAOTObjectTree` subclass with an `[ARBAOTObjectType(...)]` attribute. The base class resolves implementations through `SysExtensionAppClassFactory`; prefer that extension point over adding more type-specific branching in the form.
- Preserve the dual-provider pattern in `ARBAOTObjectTree`: the browser uses the runtime provider by default, switches to the disk provider when source viewing is enabled, and falls back to `BackupMetadataProvider` when source-backed metadata is missing.
- Inline extension rendering is a first-class behavior, not a visual afterthought. The tree code uses `ARBObjectExtension`, `ARBObjectModification`, collection maps, and extension-position metadata to merge extensions into their base nodes and mark modified nodes in bold.
- When you add a new persisted AOT browser toggle or preference, update `#CurrentVersion`, `#CurrentList`, `pack`, and `unpack` together. `extensionsInline` and `showCode` are stored through `xSysLastValue`.
- Keep the helper-table pattern for precomputed navigation data: long-running discovery is implemented as `SysOperationServiceBase` + controller pairs, and the corresponding list forms stay read-only with populate actions in the action pane.
- `ARBEntityFieldPopulateService` resolves labels recursively through mapped entity fields, EDTs, enums, views, and data sources. `ARBMenuItemRefPopulateService` walks many metadata types and stores `RefPath` values so `ARBAOTBrowser` can reopen the exact node path later.
- `ARBAOTBrowser` expects caller context to be passed through `Args` (`parmEnumType`, `parmEnum`, `parm`, and sometimes `parmObject`). If you add a new launch point, keep that contract aligned with the browser form's `init` and `updateControls` logic.
- The docs under `docs\` describe expected product behavior for search/filter, source viewing, table browser, inline extensions, open-from-form, open-from-entity, and jump-to-reference flows. Update those docs when a feature changes.
- After install or metadata changes that affect discovery, the intended workflow is to run the "Populate AOT objects" action as a batch job and usually schedule it on a recurrence so the cache stays current.

## Table metadata tables

Four persisted tables store D365FO table/field/relation/enum metadata. The primary purpose is export via Synapse Link or Fabric Link to a data lake so that AI agents can use the metadata to understand the other D365FO tables that are also exported to the lake (field labels, data types, relationships, enum value meanings, etc.) without needing access to a development environment. They are populated by the `ARBTableMetadataPopulateService` batch job (controller: `ARBTableMetadataPopulateController`) and exposed via read-only OData data entities. All four tables have `SaveDataPerCompany = No` and `CacheLookup = Found`. See `docs/tablemetadata.md` for full documentation.

- **ARBTableMetadata** — One record per table. Key: `TableName`. Fields: `TableName`, `TableNum`, `TableLabel`, `TableGroup`, `Description`, `SqlTableName`, `SaveDataPerCompany` (NoYes), `TableType`.
- **ARBTableFieldMetadata** — One record per field on each table. Key: `TableName` + `FieldName`. Fields: `TableName`, `FieldName`, `FieldId`, `FieldLabel`, `FieldDataType`, `ExtendedDataType`, `EnumTypeName`, `StringSize`, `IsMandatory` (NoYes), `HelpText`. Has a relation to `ARBTableMetadata`.
- **ARBTableRelationMetadata** — One record per field constraint on each table relation. Key: `TableName` + `RelationName` + `FieldName`. Fields: `TableName`, `RelationName`, `RelatedTableName`, `RelationshipType`, `FieldName`, `RelatedFieldName`, `Cardinality`, `RelatedTableCardinality`. Has a relation to `ARBTableMetadata` and a non-unique index on `RelatedTableName`.
- **ARBEnumMetadata** — One record per value on each base enumeration. Key: `EnumName` + `ValueName`. Fields: `EnumName`, `EnumLabel`, `ValueName`, `ValueLabel`, `IntegerValue`.
- **ARBTableKeyFieldMetadata** — One record per field in each primary index and replacement key. Key: `TableName` + `IndexName` + `FieldName`. Fields: `TableName`, `IndexName`, `FieldName`, `KeyType` (PrimaryIndex or ReplacementKey), `FieldPosition`. Has a relation to `ARBTableMetadata`.

OData entities: `ARBTableMetadataEntity`, `ARBTableFieldMetadataEntity`, `ARBTableRelationMetadataEntity`, `ARBEnumMetadataEntity`, `ARBTableKeyFieldMetadataEntity`. Each has a corresponding staging table for data management.

Forms: **Table metadata** (`ARBTableMetadata` — Task Double pattern with fields/relations tabs) and **Enum metadata** (`ARBEnumMetadata` — SimpleList pattern). Both are under **Common > Common**.

Security: `ARBTableMetadataEntityMaintain` and `ARBTableMetadataEntityView` privileges control data entity access; populate actions are covered by `ARBAOTBrowserMaintain`.
