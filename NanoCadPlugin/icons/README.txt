Каталог иконок
==============

index.html                 — открыть в браузере: галерея с поиском и именами иконок
sheet_mcsmenu.png          — общий лист всех иконок
mcsmenu\<имя>.png         — отдельные PNG

Как поставить свою иконку
-------------------------
1. Открыть index.html в браузере, найти подходящую иконку и запомнить имя (например I285).
2. В проекте открыть PluginInfo.cs и задать:
       internal const string IconName = "I285";
3. Собрать проект и загрузить DLL в nanoCAD командой NETLOAD.
   Иконка и подписи читаются из user_ribbon.cfg ТОЛЬКО при запуске nanoCAD,
   поэтому после первого NETLOAD перезапустите nanoCAD.
4. Секцию в профиле плагин создаёт сам, вручную писать ничего не нужно:
       [\configman\commands\MYPLUGIN_RUN]
       BitmapDll=smcsmenu.dll |Icon=sI285
   (имя команды зависит от PluginInfo.IdPrefix).

Другие библиотеки иконок в папке nanoCAD:
  CADLibEnabler\CADLibRes.dll  — иконки CADLib (IDI_...)
  bin\newbtns.dll              — растровые кнопки интерфейса
