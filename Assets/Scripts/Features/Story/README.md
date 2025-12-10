# Story Trigger System

Система управління тригерами для реалізації сюжетної прогресії в Unity.

## Огляд

Система підтримує:
- ✅ Послідовність подій (квести один за одним)
- ✅ Умовну активацію (перевірка стану гри)
- ✅ Одноразові тригери (спрацьовують лише один раз)
- ✅ Прості булеві флаги для відстеження прогресу
- ✅ Дії: діалоги, переходи між сценами, зміна стану, контроль інших тригерів
- ✅ Автоматичне збереження прогресу

## Початок роботи

### 1. Створення StoryState Asset

1. В Unity: `Assets → Create → ScriptableObjects → StoryState`
2. Назвіть його "StoryState"
3. Додайте його до `GlobalLifetimeScope` в поле `storyState`

### 2. Створення StoryTrigger

1. Додайте до GameObject компонент `StoryTrigger`
2. Налаштуйте параметри:
   - **Trigger ID**: Унікальний ідентифікатор (наприклад, "boss_initial_talk")
   - **One Time Only**: Чи спрацьовує лише раз
   - **Start Enabled**: Чи активний на старті
   - **Visual Cue**: GameObject з підказкою для гравця
   - **Conditions**: Умови для активації (масив)
   - **Actions**: Дії при активації (масив)

### 3. Додавання умов (Conditions)

Натисніть "+" в розділі Conditions та оберіть тип:

- **RequireFlagCondition**: Перевіряє, що флаг встановлений
- **RequireNotFlagCondition**: Перевіряє, що флаг НЕ встановлений
- **RequireAllFlagsCondition**: Всі флаги з масиву встановлені
- **RequireAnyFlagCondition**: Хоча б один флаг встановлений

### 4. Додавання дій (Actions)

Натисніть "+" в розділі Actions та оберіть тип:

- **StartDialogueAction**: Запускає Ink діалог
- **SetFlagAction**: Встановлює флаг сюжету
- **TransitionSceneAction**: Перехід до іншої сцени
- **EnableTriggerAction**: Активує інший тригер
- **DisableTriggerAction**: Деактивує інший тригер

## Приклади використання

### Простий ланцюг квестів

**Крок 1: Розмова з Босом**
```
GameObject: BossDeskTrigger
StoryTrigger:
  - triggerID: "boss_initial_talk"
  - oneTimeOnly: true
  - conditions: [] (порожньо)
  - actions:
    - StartDialogueAction (boss_intro.ink)
    - SetFlagAction ("boss_met", true)
```

**Крок 2: Міні-гра (доступна після розмови)**
```
GameObject: ComputerTrigger
StoryTrigger:
  - triggerID: "minigame_start"
  - oneTimeOnly: false
  - conditions:
    - RequireFlagCondition ("boss_met")
    - RequireNotFlagCondition ("minigame_completed")
  - actions:
    - TransitionSceneAction ("VirusMinigame", "spawn_player")
```

**Крок 3: Повернення до Боса**
```
GameObject: BossDeskTrigger
StoryTrigger:
  - triggerID: "boss_completion_talk"
  - oneTimeOnly: true
  - conditions:
    - RequireFlagCondition ("minigame_completed")
  - actions:
    - StartDialogueAction (boss_thanks.ink)
    - SetFlagAction ("boss_thanked", true)
```

### Система відкриття локацій

**Заблоковані двері:**
```
GameObject: SecurityDoor
StoryTrigger:
  - triggerID: "security_door_locked"
  - conditions:
    - RequireNotFlagCondition ("has_keycard")
  - actions:
    - StartDialogueAction (door_locked.ink)
```

**Відкриті двері:**
```
GameObject: SecurityDoor (інший компонент)
StoryTrigger:
  - triggerID: "security_door_unlocked"
  - conditions:
    - RequireFlagCondition ("has_keycard")
  - actions:
    - TransitionSceneAction ("SecureArea", "door_entrance")
```

### Динамічна активація тригерів

```
GameObject: TaskComplete
StoryTrigger:
  - triggerID: "task_a_complete"
  - actions:
    - SetFlagAction ("task_a_done", true)
    - EnableTriggerAction ("npc_new_quest")
```

## Використання в коді

### Встановлення флагу з міні-гри

```csharp
using Core.Abstractions;
using VContainer;

public class CatchManager : MonoBehaviour
{
    [Inject] private IStoryManager _storyManager;

    private void OnMinigameComplete()
    {
        _storyManager.SetFlag("minigame_virus_completed");
    }
}
```

### Перевірка флагу

```csharp
if (_storyManager.CheckFlag("boss_met"))
{
    // Логіка для випадку, коли гравець зустрів боса
}
```

### Використання констант

```csharp
using Features.Story;

_storyManager.SetFlag(StoryFlags.BossMet);
if (_storyManager.CheckFlag(StoryFlags.MinigameCompleted))
{
    // ...
}
```

## Підписка на події

```csharp
using Core.Events;

private void OnEnable()
{
    GameEvents.Story.OnFlagSet += OnFlagSet;
    GameEvents.Story.OnTriggerActivated += OnTriggerActivated;
}

private void OnDisable()
{
    GameEvents.Story.OnFlagSet -= OnFlagSet;
    GameEvents.Story.OnTriggerActivated -= OnTriggerActivated;
}

private void OnFlagSet(string flagName)
{
    Debug.Log($"Flag set: {flagName}");
}

private void OnTriggerActivated(string triggerID)
{
    Debug.Log($"Trigger activated: {triggerID}");
}
```

## Архітектура

### Компоненти системи

1. **StoryState (ScriptableObject)** - зберігає флаги та історію тригерів
2. **IStoryManager / StoryManager** - центральний контролер
3. **StoryCondition** - базовий клас для умов
4. **StoryAction** - базовий клас для дій
5. **StoryTrigger (MonoBehaviour)** - компонент тригера

### Потік даних

```
Гравець входить у зону → StoryTrigger
                             ↓
                   Перевірка умов
                             ↓
                   Умови виконані?
                             ↓ ТАК
                    Виконання дій
                             ↓
                Оновлення StoryState
                             ↓
                  Автоматичне збереження
```

## Налаштування та налагодження

### Скидання прогресу

В StoryState є метод `ResetAll()` для скидання всіх флагів та тригерів. Використовуйте для тестування.

### Перегляд активних флагів

В Inspector StoryState ви можете побачити списки:
- `Active Flags List` - всі встановлені флаги
- `Fired Triggers List` - всі спрацьовані тригери

### Валідація

Система автоматично логує попередження:
- Дублікати triggerID
- Порожні triggerID
- Спроби активувати незареєстровані тригери
- Порожні назви флагів

## Розширення системи

### Створення нової умови

```csharp
using System;
using Core.Abstractions;
using UnityEngine;

namespace Features.Story.Conditions
{
    [Serializable]
    public class MyCustomCondition : StoryCondition
    {
        [SerializeField] private string myParameter;

        public override bool Evaluate(IStoryManager storyManager)
        {
            // Ваша логіка
            return true;
        }
    }
}
```

### Створення нової дії

```csharp
using System;
using UnityEngine;

namespace Features.Story.Actions
{
    [Serializable]
    public class MyCustomAction : StoryAction
    {
        [SerializeField] private string myParameter;

        public override void Execute(StoryActionContext context)
        {
            // Ваша логіка
        }
    }
}
```

## Поради

1. **Використовуйте StoryFlags констanti** замість рядків для уникнення помилок
2. **Плануйте ланцюги квестів** на папері перед імплементацією
3. **Тестуйте послідовно** - створіть кілька тригерів і перевірте їх взаємодію
4. **Використовуйте описові triggerID** - наприклад, "boss_day1_morning_talk"
5. **Комбінуйте умови** для складної логіки (RequireAll + RequireNot)

## Troubleshooting

**Тригер не активується:**
- Перевірте, чи всі умови виконані
- Перевірте, чи не спрацював тригер раніше (якщо oneTimeOnly = true)
- Переконайтесь, що triggerID унікальний
- Переконайтесь, що у GameObject є Collider2D з Is Trigger = true

**Флаги не зберігаються:**
- Переконайтесь, що StoryState прив'язаний до GlobalLifetimeScope
- Перевірте, чи викликається SaveGame() при переходах між сценами

**Тригер спрацьовує кілька разів:**
- Встановіть oneTimeOnly = true для одноразових тригерів

## Контакти та підтримка

При виникненні проблем перевірте:
1. Console на наявність попереджень/помилок
2. StoryState Inspector для поточних флагів
3. Чи всі залежності ін'єктовані через VContainer
