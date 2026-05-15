---
name: thesis-screenshots
description: Complete screenshot inventory for DrugstoreSystem BMI thesis. Specifies what to capture, when to capture it, and where to save it. Open during every UI sprint.
---

# Thesis Screenshots — DrugstoreSystem

All screenshots go to `docs/thesis/images/`. Naming: `{chapter}.{section}.{order}-{slug}.png`.

---

## Sprint-by-Sprint Capture Schedule

### DEV-00 (Bootstrap)
| File | What to capture | When |
|---|---|---|
| `2.1.1-solution-explorer.png` | Solution Explorer showing all 5 projects | After `dotnet build` succeeds |

### DEV-02 (Database)
| File | What to capture |
|---|---|
| `2.2.2-connection-string.png` | `appsettings.json` snippet (NO real password visible) |
| `2.2.3-dbcontext.png` | `DrugstoreDbContext.cs` class in IDE |
| `2.2.4-entity-model.png` | `Medicine.cs` entity + configuration class |
| `2.2.5-gin-index-migration.png` | Migration file showing `CREATE INDEX ... USING GIN` |
| `2.2.6-migration-output.png` | Terminal: `dotnet ef migrations add` + `database update` output |

### DEV-03 (Auth)
| File | What to capture |
|---|---|
| `3.1.1-login.png` | Login page in browser (clean, no errors) |

### DEV-04 (Admin)
| File | What to capture |
|---|---|
| `3.1.2-admin-dashboard.png` | Admin dashboard with stat cards |
| `3.1.3-pharmacy-list.png` | Pharmacy list with active/inactive status |
| `3.1.4-pharmacy-create.png` | Pharmacy create form (Step 1: pharmacy info) |

### DEV-05 (Pharmacist)
| File | What to capture |
|---|---|
| `3.1.5-pharmacist-profile.png` | Pharmacist profile edit form |
| `3.1.6-inventory-list.png` | Inventory list with low-stock badges |

### DEV-06 (Catalog)
| File | What to capture |
|---|---|
| `3.1.7-inventory-add-autocomplete.png` | Autocomplete dropdown showing medicine suggestions |
| `3.1.8-create-new-medicine.png` | New medicine creation form (expanded with synonym chips) |

### DEV-07 (Public Search)
| File | What to capture |
|---|---|
| `3.1.9-search-results.png` | Search results page with expansion panels and pharmacy table |
| `3.1.10-medicine-detail.png` | Medicine detail page with synonyms list |
| `3.1.11-pharmacy-detail.png` | Pharmacy detail with "Xaritada ko'rish" button |

### DEV-08 (Algorithm)
| File | What to capture |
|---|---|
| `3.2.1-haversine-code.png` | `HaversineCalculator.cs` full class in IDE |
| `3.2.2-sort-distance.png` | Search results sorted by distance (km column visible) |
| `3.2.3-sort-price.png` | Same results sorted by price (toggle switched to "Narx") |
| `3.2.4-unit-tests.png` | Test runner (green checkmarks on all Haversine tests) |

### DEV-09 (Seed Data)
| File | What to capture |
|---|---|
| `3.1.12-demo-data-search.png` | Search for "para" showing real seeded results with distances |

### DEV-10 (QA)
| File | What to capture |
|---|---|
| `3.2.5-sort-comparison.png` | Side-by-side: same search sorted by distance vs price |
| `3.2.6-typo-search.png` | Searching "paratsetamol" → shows Paracetamol (fuzzy match demo) |
| `3.2.7-russian-search.png` | Searching "Парацетамол" → shows Paracetamol (Russian name demo) |

### Manual diagrams (draw.io / dbdiagram.io)
| File | Tool | What |
|---|---|---|
| `2.1.2-architecture.png` | draw.io | 4-layer architecture diagram (colored boxes) |
| `2.1.4-search-flowchart.png` | draw.io | Search algorithm 5-stage flowchart |
| `2.2.1-er-diagram.png` | dbdiagram.io | ER diagram (all tables + relationships) |
| `1.1.1-platform-concept.png` | draw.io | Pharmacy ↔ Platform ↔ User triangle |
| `1.2.1-trigram-diagram.png` | draw.io | String → trigrams → similarity score |
| `3.2.1-haversine-diagram.png` | draw.io | Earth sphere with two points + arc |

---

## Screenshot Quality Rules

1. **Window size**: Full browser width (1280px+). No toolbar clutter.
2. **Data**: Use real seeded data. No "Lorem ipsum" or placeholder text.
3. **UI language**: UI must show Uzbek strings (not English hardcoded text).
4. **No passwords visible**: Blur or crop any password fields.
5. **File format**: PNG, not JPEG (no compression artifacts).
6. **Resolution**: At least 1x (96 DPI). Retina screenshots are fine.

---

## Thesis Figure Captions

Use this naming in the thesis Markdown:

```markdown
![2.1.1-rasm. DrugstoreSystem loyihasining Solution Explorer ko'rinishi](images/2.1.1-solution-explorer.png)

*2.1.1-rasm. DrugstoreSystem loyihasining Solution Explorer ko'rinishi.*
```

Note: Every figure caption ends with a period. Figure number format: `X.Y.Z-rasm.`

---

## Reminder Checklist (before marking a sprint DONE)

- [ ] All screenshots for this sprint captured
- [ ] Files named correctly (`X.Y.Z-slug.png`)
- [ ] Saved to `docs/thesis/images/`
- [ ] No sensitive data (passwords, real email addresses) visible
- [ ] Screenshots show Uzbek UI text
