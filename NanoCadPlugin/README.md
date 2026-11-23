# Шаблон плагина для ленты nanoCAD 25 (x64)

Готовая заготовка плагина: **пользователь подставляет свои расчёты и свой интерфейс, а установка
вкладки на ленту, команды, иконка и cfg-секция делаются автоматически при `NETLOAD`.**

Проект автономен: сборки SDK nanoCAD (`hostmgd.dll`, `hostdbmgd.dll`) лежат в папке `lib\`,
внешние пути не нужны. Скачал папку → добавил код → собрал → `NETLOAD` → на ленте появилась вкладка.

---

## 1. Что умеет шаблон «из коробки»

| Что | Как реализовано |
|---|---|
| Вкладка на ленте | `RibbonTabsAndPanels.xml` в профиле nanoCAD |
| Видимость вкладки на ленте | `RibbonWS.xml` (рабочее пространство) |
| Кликабельная кнопка | `RibbonCmds.xml` (без неё кнопка рисуется, но «мёртвая») |
| Иконка, подписи, тултип | `user_ribbon.cfg` |
| Открытие окна по кнопке | `[CommandMethod]` + `PluginForm` |
| Автоустановка при загрузке | `IExtensionApplication.Initialize()` → установщик → `RELOADRIBBON` |
| Идемпотентность | Повторный `NETLOAD` не плодит дубликаты (удаление по UID + проверка «ровно 1») |
| Резервные копии профиля | `*.before-MYPLUGIN.bak`, создаются один раз |
| Удаление всех своих записей | команда `MYPLUGIN_UNINSTALL` |

Заглушка расчёта рабочая: сумма и произведение двух чисел. Сразу после сборки видно всю цепочку
«кнопка → окно → ввод → расчёт → вывод», и только потом её заменяют своим кодом.

---

## 2. Требования

- **nanoCAD x64 25.0**, установленный в `C:\Program Files\Nanosoft\nanoCAD x64 25.0\`.
  Запускать нужно именно установленную копию: копия установки с другого диска стартовать может и не дать.
- **.NET SDK 6+** (`dotnet --list-sdks`). Проект собирается под `net6.0-windows` — это рантайм nanoCAD 25.
- **Visual Studio** — только чтобы редактировать форму в конструкторе (сборка возможна и из консоли).
- Права администратора **не нужны**: всё пишется в `%APPDATA%` пользователя.

---

## 3. Структура проекта

```
NanoCadPluginTemplate\
  NanoCadPluginTemplate.slnx
  NanoCadPlugin\
    NanoCadPlugin.csproj     — net6.0-windows, x64, ссылки на локальный lib\
    README.md                — этот файл
    lib\                     — hostmgd.dll, hostdbmgd.dll и их .xml-документация (SDK nanoCAD)
    icons\                   — галерея иконок из mcsmenu.dll: index.html + PNG
    PluginInfo.cs            — ★ единственное место для имён команды/вкладки/кнопки и иконки
    Commands.cs              — [CommandMethod] + точка входа IExtensionApplication
    RibbonInstaller.cs       — установка/удаление записей в профиле nanoCAD
    Calculator.cs            — ★ ваш расчёт: PluginInput / PluginResult / Calculate()
    PluginForm.cs            — ★ логика окна: чтение полей, вызов расчёта, вывод
    PluginForm.Designer.cs   — ★ ваш интерфейс (открывается в конструкторе VS)
    PluginForm.resx
    FormLauncher.cs          — показ окна поверх nanoCAD (единственное место с HostMgd)
```

★ — файлы, которые пользователь меняет. Остальное трогать не нужно.

### Почему `lib\` и `<Private>False</Private>`

`lib\` — **время сборки**: компилятор берёт оттуда `hostmgd.dll`/`hostdbmgd.dll`, никакие пути вида
`Z:\...\SDK25\...` не нужны.

`<Private>False</Private>` — **время работы**: эти DLL **не копируются** в выходную папку. В nanoCAD
плагину их отдаёт сам хост. Если положить копию рядом со своей DLL, плагин может подхватить чужую
версию сборок nanoCAD — классическое «у меня работает, у тебя нет». Не включайте копирование.

---

## 4. Быстрый старт: пять шагов

### Шаг 1. Свои имена и иконка — `PluginInfo.cs`

```csharp
internal const string IdPrefix   = "MYPLUGIN";                 // уникальный префикс (UID, имена команд)
internal const string TabText    = "Мой плагин";               // вкладка на ленте
internal const string ButtonText = "Открыть окно расчёта";     // подпись кнопки
internal const string WindowTitle= "Мой плагин: расчёт";       // заголовок окна
internal const string KeyTip     = "M";                        // Alt+M
internal const string IconName   = "I285";                     // иконка из mcsmenu.dll
```

Из этих констант автоматически получаются: `MYPLUGIN_RUN`, `MYPLUGIN_RIBBON`, `MYPLUGIN_UNINSTALL`,
`ID_PanelMYPLUGIN`, `ID_TabMYPLUGIN` и все записи в профиле. **Вручную в профиль писать ничего не надо.**

### Шаг 2. Свой расчёт — `Calculator.cs`

Замените поля `PluginInput`/`PluginResult` и тело `PluginCalculator.Calculate()`. Соберите
многострочный `Report` — форма покажет его в окне. Вывод чисел — через `Fmt()` (точка как разделитель,
не зависит от локали Windows).

### Шаг 3. Свой интерфейс — `PluginForm.Designer.cs`

Откройте `PluginForm.cs` двойным щелчком в VS (или «Просмотр кода» → «Конструктор»): сейчас там группа
«Исходные данные» с двумя полями-примерами, кнопка и область результатов. Добавляйте/удаляйте контролы
в дизайнере, затем в `PluginForm.cs`:

- `FillDefaults()` — значения по умолчанию;
- `Calculate()` — прочитать свои поля, заполнить `PluginInput`, вызвать `PluginCalculator.Calculate()`;
- для вывода по отдельным полям используйте готовый `Set(box, value, format)`.

Требования к форме (иначе конструктор её не откроет): `public partial : Form`, конструктор без
параметров с `InitializeComponent()` первой строкой, все контролы — поля в `.Designer.cs`, **ноль**
упоминаний `HostMgd` в `PluginForm.cs`/`.Designer.cs`, `AutoScaleMode.Font`.

### Шаг 4. Собрать

```powershell
# вариант 1 (рекомендуется) — проект напрямую:
dotnet build "C:\Users\inc\source\repos\NanoCadPluginTemplate\NanoCadPlugin\NanoCadPlugin.csproj" -c Debug -p:Platform=x64 --nologo

# вариант 2 — решение целиком (без -p:Platform, в .slnx нет конфигурации x64):
dotnet build "C:\Users\inc\source\repos\NanoCadPluginTemplate\NanoCadPluginTemplate.slnx" -c Debug --nologo
```

В обоих случаях DLL всё равно собирается **x64**: в csproj жёстко задан `<PlatformTarget>x64</PlatformTarget>`.
Не передавайте `-p:Platform=x64` решению `.slnx` — там такой конфигурации нет (MSB4126); используйте один
из вариантов выше.

Результат: `NanoCadPlugin\bin\Debug\NanoCadPlugin.dll` (без подпапок `net6.0-windows\x64`).
В комплекте уже лежит собранная заглушка — её можно загрузить через `NETLOAD` сразу, до первой сборки.
В `bin\Debug` **не должно быть** `hostmgd.dll`/`hostdbmgd.dll` — так и задумано.

Если DLL не перезаписывается — её держит запущенный nanoCAD: закройте его штатно и соберите снова.

### Шаг 5. Загрузить в nanoCAD

В командной строке nanoCAD:

```
NETLOAD
```

и выбрать `...\bin\Debug\NanoCadPlugin.dll`. Сразу после загрузки:

1. плагин дописывает себя в четыре файла профиля;
2. вызывает `RELOADRIBBON`;
3. на ленте появляется вкладка `Мой плагин` с кнопкой.

**Иконка и подписи подхватятся только после перезапуска nanoCAD** — `RELOADRIBBON` перечитывает
только XML, а `user_ribbon.cfg` читается при старте. Это не баг плагина.

Альтернатива ручному `NETLOAD` — из LISP (обратные слэши обязательны, иначе `0x80131047`):

```lisp
(command "NETLOAD" "C:\\Users\\inc\\source\\repos\\NanoCadPluginTemplate\\NanoCadPlugin\\bin\\Debug\\NanoCadPlugin.dll")
```

---

## 5. Команды

| Команда | Назначение |
|---|---|
| `MYPLUGIN_RUN` | открыть окно (её вызывает кнопка ленты) |
| `MYPLUGIN_RIBBON` | переустановить вкладку на ленте (если профиль почистили вручную) |
| `MYPLUGIN_UNINSTALL` | удалить панель, вкладку, WS-ссылку, команду и cfg-секцию из профиля |

Имена собираются из `IdPrefix`: при смене префикса меняются все три команды.

---

## 6. Иконки

1. Откройте `NanoCadPlugin\icons\index.html` в браузере — это галерея иконок из `mcsmenu.dll`
   с поиском и готовой строкой для cfg.
2. Выберите иконку и подставьте её имя в `PluginInfo.IconName` (например `I285`).
3. `IconDll` менять не нужно (`mcsmenu.dll` — штатные ресурсы nanoCAD, путь не указывается).

В `user_ribbon.cfg` значения-строки помечаются служебным префиксом `s`, поэтому для `IconDll =
"mcsmenu.dll"` и `IconName = "I285"` плагин сам пишет:

```ini
BitmapDll=smcsmenu.dll |Icon=sI285
```

Если иконка не появилась — почти всегда `BitmapDll` указывает на несуществующий файл (например,
случайно записано `ssmcsmenu.dll`). Проверьте строку в `%APPDATA%\Nanosoft\nanoCAD x64 25.0\config\user_ribbon.cfg`.

Иконки насоса в ресурсах нет — это нормально, берите ближайшую по смыслу. Подписи в галерее взяты
из `mcsmenu.cfg`, поэтому они описывают штатные команды nanoCAD, а не ваш плагин.

---

## 7. Как это работает внутри (кратко)

Лента nanoCAD 25 — **не CUIX и не .NET API**. Методы `Application.LoadPartialMenu`,
`ReloadAllMenus`, `MenuGroups` и т.п. в `hostmgd.dll` — пустые заглушки (проверено декомпиляцией).
Реальная лента — данные в профиле:

```
%APPDATA%\Nanosoft\nanoCAD x64 25.0\config\
    RibbonTabsAndPanels.xml   — панель, вкладка, кнопка
    RibbonWS.xml              — какие вкладки показывать
    RibbonCmds.xml            — таблица команд ленты
    user_ribbon.cfg           — команда, иконка, подписи
```

Установщик (`RibbonInstaller`) при каждом `NETLOAD`:

1. находит папку профиля (по версии процесса nanoCAD, а не по угаданному имени папки);
2. удаляет свои прежние записи **по UID** (не по комментарию-маркеру — так не остаётся дублей);
3. вставляет панель и вкладку в конец коллекций, WS-ссылку, запись в `RibbonCmds`, секцию в `user_ribbon.cfg`;
4. сохраняет BOM и перевод строки исходных файлов (кириллица не портится);
5. один раз делает резервные копии `*.before-MYPLUGIN.bak`;
6. вызывает `RELOADRIBBON`.

---

## 8. Переименование плагина под себя

`TabText`, `ButtonText`, `WindowTitle`, `KeyTip`, `IconName` — меняйте свободно, старые записи при
следующем `NETLOAD` будут заменены.

`IdPrefix` — меняйте, если делаете **второй** плагин. Если плагин уже был установлен со старым
префиксом, сначала выполните `<СТАРЫЙ_ПРЕФИКС>_UNINSTALL` (например `MYPLUGIN_UNINSTALL`), иначе в
профиле останутся записи со старыми UID и на ленте будет две вкладки. То же относится к переименованию
самого проекта/сборки: путь к DLL при `NETLOAD` меняется.

---

## 9. Переустановка, откат, удаление

- **Переустановить вкладку:** `MYPLUGIN_RIBBON`. Дубликатов не будет.
- **Удалить плагин из профиля:** `MYPLUGIN_UNINSTALL`, затем перезапустить nanoCAD.
- **Полный откат:** закрыть nanoCAD и вернуть на место файлы `*.before-MYPLUGIN.bak` (снять с них
  суффикс) либо удалить `user_ribbon.cfg`, если он создавался только плагином.

Проверка, что всё внедрилось ровно по одному разу (PowerShell):

```powershell
$d = "$env:APPDATA\Nanosoft\nanoCAD x64 25.0\config"
$checks = @{
  'RibbonTabsAndPanels.xml' = @('UID="ID_PanelMYPLUGIN"','UID="ID_TabMYPLUGIN"','PanelId="ID_PanelMYPLUGIN"')
  'RibbonWS.xml'            = @('TabId="ID_TabMYPLUGIN"')
  'RibbonCmds.xml'          = @('UID="MYPLUGIN_RUN"')
}
foreach ($f in $checks.Keys) {
  $s = Get-Content -Raw -Encoding UTF8 (Join-Path $d $f)
  $counts = $checks[$f] | ForEach-Object { [regex]::Matches($s, [regex]::Escape($_)).Count }
  "{0}: {1}" -f $f, ($counts -join ', ')
}
```

Каждый счётчик должен быть **ровно 1**. Имена в проверке подставьте свои, если меняли `IdPrefix`.

---

## 10. Проверка формы без nanoCAD

Полезно, если конструктор Visual Studio капризничает: форма должна создаваться **без**
`hostmgd.dll` рядом. Отдельный консольный проект (`net6.0-windows`, `[STAThread]`):

```csharp
Assembly asm = Assembly.LoadFrom(@"...\bin\Debug\NanoCadPlugin.dll");
Type t = asm.GetType("NanoCadPlugin.PluginForm", throwOnError: true);
using (var form = (Form)Activator.CreateInstance(t))
    Console.WriteLine("OK: " + form.Text + " / " + form.ClientSize);
```

Ожидаемо: `OK: Мой плагин: расчёт / 760x560` (или ваши значения). Если падает — в форме появилась
ссылка на `HostMgd`; вынесите её в `FormLauncher`.

---

## 11. Грабли (проверено на живой системе)

| Симптом | Причина | Решение |
|---|---|---|
| `NETLOAD`: `0x80131047` | прямые слэши в LISP-пути | `(command "NETLOAD" "C:\\path\\DLL")` — двойные обратные |
| Вкладка не появилась | нет WS-ссылки в `RibbonWS.xml` | должен был добавить установщик; проверьте `MYPLUGIN_RIBBON` |
| Кнопка нарисована, клик не работает | нет записи в `RibbonCmds.xml` | проверьте `RibbonCmds.xml`, повторите `MYPLUGIN_RIBBON` |
| Иконка/подписи не обновились | `RELOADRIBBON` не читает cfg | перезапустить nanoCAD |
| Кнопка без иконки (пустое место) | `BitmapDll` указывает на несуществующий файл | в `user_ribbon.cfg` должно быть `BitmapDll=smcsmenu.dll` и `Icon=sI285` |
| Две вкладки | меняли `IdPrefix` без `UNINSTALL` | старый `<ПРЕФИКС>_UNINSTALL`, затем `NETLOAD` |
| `dotnet build`: файл занят | DLL загружена в nanoCAD | закрыть nanoCAD, собрать снова |
| Форма не открывается в дизайнере | нет `Designer.cs`/публичного ctor/`Platforms` | см. раздел 4, шаг 3; в csproj уже `Platforms=x64;AnyCPU` |
| Кириллица в исходниках «кракозябрами» | нет BOM | сохранять `.cs` как UTF-8 **с BOM** |
| Ошибка CS0104 `Exception` неоднозначен | `Teigha.Runtime` содержит свой `Exception` | всегда `catch (System.Exception ex)` |
| `Application` неоднозначен | WinForms vs `HostMgd...Application` | квалифицировать полностью |
| В узком окне вкладка обрезана («Мой пл…») | мало места на ленте | это нормально, не баг |

---

## 12. Что заведомо не работает (не тратьте время)

| Подход | Результат |
|---|---|
| `Application.LoadPartialMenu` / `ReloadAllMenus` / `MenuGroups` / `MenuBar` | пустые тела в `hostmgd.dll` |
| Отдельный CUIX с вкладкой | подхватывается, но требует ручной регистрации и легко даёт пустые/дублирующиеся вкладки |
| Частичный CUIX с тем же UID, что у продукта | **две** вкладки: слияние добавляет, а не заменяет |
| Правка `C:\Program Files\...\*.cuix` | нужны права администратора, для плагина не годится |
| `SentKeys` в область чертежа | текст уходит в динамический ввод как координата |
| UI Automation для кнопок ленты nanoCAD | лента Qt, кнопок в дереве нет |

---

## 13. Чеклист «свой плагин за 15 минут»

1. `PluginInfo.cs` — `TabText`, `ButtonText`, `IconName`.
2. `Calculator.cs` — свои поля и свой расчёт, `Report` для окна.
3. `PluginForm.Designer.cs` + `PluginForm.cs` — свои поля и чтение значений.
4. `dotnet build ... -p:Platform=x64`.
5. `NETLOAD` → вкладка появилась.
6. Перезапустить nanoCAD → подтянулись иконка и подписи.
7. Повторный `NETLOAD` → дубликатов нет.
8. Своя кнопка → своё окно → свой расчёт.
