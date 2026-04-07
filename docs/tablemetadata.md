# Table and Enum Metadata

The AOT Browser includes a set of metadata tables that store detailed information about all tables, fields, relations, and enumerations in the AOT. This metadata is stored in regular D365FO tables and exposed via OData data entities, making it accessible to AI agents, Synapse Link, Power Platform, and other external consumers without requiring access to a development environment.

## Metadata Tables

### ARBTableMetadata

Stores one record per table in the AOT.

| Field | Type | Description |
|-------|------|-------------|
| TableName | String | AOT name of the table |
| TableNum | Int | Numeric table ID |
| TableLabel | String | Resolved display label |
| TableGroup | String | Table group classification (e.g., Main, Group, Transaction) |
| Description | String | Table description |
| SqlTableName | String | Physical SQL table name |
| SaveDataPerCompany | Enum (NoYes) | Whether the table stores data per legal entity |
| TableType | String | Table type (e.g., Regular, InMemory, TempDB) |

### ARBTableFieldMetadata

Stores one record per field on each table.

| Field | Type | Description |
|-------|------|-------------|
| TableName | String | Parent table name |
| FieldName | String | Field name |
| FieldId | Int | Numeric field ID |
| FieldLabel | String | Resolved display label (walks EDT inheritance chain) |
| FieldDataType | String | Base data type (e.g., String, Int32, Real, Enum) |
| ExtendedDataType | String | EDT name, if the field uses one |
| EnumTypeName | String | Base enum name, for enum-type fields |
| StringSize | Int | Maximum string length, for string-type fields |
| IsMandatory | Enum (NoYes) | Whether the field is mandatory |
| HelpText | String | Resolved help text (walks EDT inheritance chain) |

### ARBTableRelationMetadata

Stores one record per field constraint on each table relation.

| Field | Type | Description |
|-------|------|-------------|
| TableName | String | Source table name |
| RelationName | String | Relation name |
| RelatedTableName | String | Target (related) table name |
| RelationshipType | String | Relationship type (e.g., Association, Composition) |
| FieldName | String | Source field name |
| RelatedFieldName | String | Target field name |
| Cardinality | String | Source table cardinality |
| RelatedTableCardinality | String | Target table cardinality |

### ARBEnumMetadata

Stores one record per value on each base enumeration.

| Field | Type | Description |
|-------|------|-------------|
| EnumName | String | Base enum name |
| EnumLabel | String | Resolved enum label |
| ValueName | String | Enum value name |
| ValueLabel | String | Resolved value label |
| IntegerValue | Int | Numeric integer value |

## Populating the Metadata

The metadata tables must be populated after installation and whenever the AOT changes. Navigate to **Common > Common > Table metadata** and click the **Populate table metadata** button.

It is recommended to run this as a batch job as it processes all tables, fields, relations, and enumerations in the system. Scheduling the batch job with recurrence keeps the metadata up-to-date.

## OData Data Entities

All four metadata tables are exposed as public OData data entities for external consumption:

| Data Entity | OData Collection |
|-------------|-----------------|
| ARBTableMetadataEntity | ARBTableMetadata |
| ARBTableFieldMetadataEntity | ARBTableFieldMetadata |
| ARBTableRelationMetadataEntity | ARBTableRelationMetadata |
| ARBEnumMetadataEntity | ARBEnumMetadata |

These entities can be accessed at `https://<environment>/data/ARBTableMetadata` (and similar for the other collections).

## Viewing the Metadata

Two forms are available under **Common > Common** to browse the metadata directly in the D365FO web interface:

- **Table metadata** — Shows tables, with drill-down grids for fields and relations
- **Enum metadata** — Shows base enumerations with their values

## Use Cases

- **AI Agents / Copilot** — Provide AI agents with schema knowledge so they can understand table structures, field types, and relationships without needing development environment access.
- **Synapse Link / Data Lake** — Enrich exported data with metadata context such as field labels, help text, and relationships.
- **Power Platform** — Query table and field metadata via OData to build dynamic integrations.
- **Documentation** — Auto-generate data dictionaries from the metadata tables.
