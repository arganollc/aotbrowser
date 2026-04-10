# URL Deep Linking

You can share a direct URL that opens the AOT Browser to a specific object. This is useful for linking from documentation, chat messages, or bookmarks.

## URL Format

```
https://<environment>/?mi=ARBAOTBrowser&ObjectType=<type>&ObjectName=<name>&TreePath=<path>
```

### Parameters

| Parameter | Required | Description | Example |
|-----------|----------|-------------|---------|
| `ObjectType` | Yes | The AOT object type name | `Table`, `Form`, `Class`, `DataEntity` |
| `ObjectName` | Yes | The AOT object name | `CustTable`, `SalesTable` |
| `TreePath` | No | Slash-delimited path to a specific tree node | `Fields/AccountNum` |

### Examples

Open the CustTable table:
```
https://myenv.operations.dynamics.com/?mi=ARBAOTBrowser&ObjectType=Table&ObjectName=CustTable
```

Open the CustTable table and navigate to the AccountNum field:
```
https://myenv.operations.dynamics.com/?mi=ARBAOTBrowser&ObjectType=Table&ObjectName=CustTable&TreePath=Fields/AccountNum
```

Open the SalesTable form:
```
https://myenv.operations.dynamics.com/?mi=ARBAOTBrowser&ObjectType=Form&ObjectName=SalesTable
```

## Copy Link Button

The **Copy link** button in the action pane generates a deep link URL for the currently displayed object and selected tree node, and **copies it directly to your clipboard**. An "Link copied to clipboard" info message confirms the copy. You can then paste the URL anywhere to share it.

The button is enabled only when an AOT object is loaded.

## Supported Object Types

All object types available in the AOT Browser are supported, including: Table, View, Form, Class, DataEntity, EDT, BaseEnum, Query, SecurityRole, SecurityDuty, SecurityPrivilege, and more.
