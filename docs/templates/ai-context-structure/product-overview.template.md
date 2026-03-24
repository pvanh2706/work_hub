# {Product_Name} AI Context - Overview

## Project Identity

**Name:** {Product_Name}  
**Type:** {Product_Type}  
**Stage:** {Development_Stage}  
**Team Size:** {Team_Size}  
**Primary Language:** {Programming_Language}

## Architecture Type

{Architecture_Description}
- Example: "Modular Monolith following Clean Architecture"
- Example: "Microservices với event-driven architecture"

## Tech Stack

### Backend
- **Framework:** {Backend_Framework}
- **Language:** {Backend_Language}  
- **Database:** {Database_System}
- **Cache:** {Cache_System}
- **Message Queue:** {MQ_System} (if applicable)

### Frontend
- **Framework:** {Frontend_Framework}
- **Language:** {Frontend_Language}
- **State Management:** {State_Mgmt}
- **UI Library:** {UI_Library}

## Project Structure

```
{Root_Folder}/
├── {Backend_Folder}/
│   ├── src/
│   │   ├── {Core_Projects}
│   │   └── {Module_Structure}
│   └── tests/
└── {Frontend_Folder}/
    ├── src/
    │   ├── components/
    │   ├── views/
    │   └── ...
    └── tests/
```

## Key Modules

### Module 1: {Module_Name}
**Purpose:** {Module_Purpose}  
**Owns:** {Module_Entities}  
**Does NOT own:** {Module_Boundaries}

### Module 2: {Module_Name}
...

## Common Patterns

### {Pattern_Name_1}
**When:** {When_To_Use}  
**Example:**
```{language}
{Code_Example}
```

### {Pattern_Name_2}
...

## Module Boundaries

| Module | Owns | Does NOT Own |
|--------|------|--------------|
| {Module} | {entities} | {boundaries} |

## Configuration

- **Database:** {Connection_Info}
- **Cache:** {Cache_Config}
- **External APIs:** {API_List}

## Development Commands

See: `/memories/repo/development-commands.md`

## Key Decisions (ADRs)

1. **{Decision_Name}:** {Brief_Reason}
2. **{Decision_Name}:** {Brief_Reason}

---

**Template Version:** 1.0  
**For:** AI assistants (GitHub Copilot, Claude, etc.)
