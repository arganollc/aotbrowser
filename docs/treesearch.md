# Tree Search

When browsing a large AOT object such as a table with hundreds of fields or a form with a deep control hierarchy, finding a specific node in the tree can be tedious.

## How to use

1. Open an AOT object in the AOT Browser.
2. The **Search in tree** box is always visible below the tree — just start typing to search.
3. The first matching node is selected automatically as you type.
4. Use the **▲** (Previous) and **▼** (Next) chevron buttons to navigate between matches.

## Details

- The search is **case-insensitive** and matches any substring of the node text.
- All nodes in the tree are searched, including collapsed branches.
- The search state is cleared whenever a new AOT object is loaded.
- This feature searches within the currently loaded object tree only — for searching across all AOT objects, use the main [Search/Filter](search.md) feature.
