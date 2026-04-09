# Tree Search

When browsing a large AOT object such as a table with hundreds of fields or a form with a deep control hierarchy, finding a specific node in the tree can be tedious.

## How to use

1. Open an AOT object in the AOT Browser.
2. Click the **Search tree** button in the action pane.
3. Enter a search term in the dialog and click **OK**.
4. Matching nodes are found and the first match is selected automatically. The infolog shows your position (e.g., "1 / 12 matches").
5. Use the **Next** and **Prev** buttons to navigate between matches.

## Details

- The search is **case-insensitive** and matches any substring of the node text.
- All nodes in the tree are searched, including collapsed branches.
- The search state is cleared whenever a new AOT object is loaded.
- This feature searches within the currently loaded object tree only — for searching across all AOT objects, use the main [Search/Filter](search.md) feature.
