# Матриця простежуваності вимог (RTM) — v1.0

Для ЛР №3 матриця доходить до критерію приймання та проєктної моделі;
реалізація і результати тестів додаються в ЛР №4 без зміни вже
погоджених вимог (окрема процедура зміни baseline).

| Вимога | Use Case | Критерій приймання | Проєктний артефакт |
|---|---|---|---|
| FR-01 Зареєструвати учня | UC-01 | AC-01 | Use Case Diagram; Activity UC-01; Class Diagram |
| FR-02 Редагувати дані учня | UC-02 | AC-02 | Use Case Diagram; Class Diagram |
| FR-03 Створити запис на консультацію | UC-03 | AC-03 | Use Case Diagram; Activity UC-03; Class Diagram |
| FR-04 Перевірити перетин часу | UC-03 (include) | AC-04 | Activity UC-03 |
| FR-05 Скасувати запис | UC-04 | AC-05 | Use Case Diagram; State Machine Diagram (Consultation) |
| FR-06 Позначити проведеним | UC-05 | AC-06 | Use Case Diagram; Activity UC-05; State Machine Diagram |
| FR-07 Переглянути історію занять | UC-06 | AC-07 | Use Case Diagram |
| FR-08 Переглянути розклад | UC-07 | AC-08 | Use Case Diagram |
| NFR-01 Один користувач, без мережі | — | AC NFR-01 | Архітектурне рішення (ЛР №1–2, SQLite локально) |
| NFR-02 Відгук ≤300 мс | — | AC NFR-02 | План перевірки — ЛР №4 |
| NFR-03 Необмежена історія | — | AC NFR-03 | Бізнес-правило в `ConsultationService` (ЛР №1–2) |

Приклад прочитання: FR-04 простежується до UC-03 (крок 3 специфікації —
include «Перевірити перетин часу»), критерію AC-04 і першого вузла
рішення в Activity Diagram UC-03. FR-06 простежується до UC-05,
критерію AC-06, класу `Consultation` і його State Machine Diagram
(перехід `Scheduled → Completed`).
