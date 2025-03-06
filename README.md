# DropTheBombs (ID: 405238 на Яндекс Играх)

Ранер с элементами шутера. Игрок управляет сначала самолетом, после падающими бомбами. Во время полета необходимо собирать бонусы увеличивая мощность и количество бомб. В конце уровня собранные бомбы уничтожают противников, в зависимости от количества побежденных
врагов игрок получает награду. В случае уничтожения главного босса происходит переход на следующий уровень сложности. Каждый 3 уровень игра переключается в режим шутера, где игрок может расстреливать противников, взрывающиеся объекты и финального босса.

Игра реализована на фреймворке HECS (гибридный ECS). В основе глобальная стейт система управляющая переходами между состояниями игры. Разрабатывалась в течении 1,5 месяцев командой из двух человек. В данном проекте я по большей части занимался внутренней логикой игры, практически не касаясь работы с UI. Для аналитики использовалась GameAnalytics.

- [YandexFeature](Features/YandexSystem) - Системы, jslib и компоненты для работы Яндекса
- [BombPositionSystem](Systems/Bombs/BombsPositionSystem.cs) - Система позиционирования бомб на основе разворота матрицы в ступенчаты массив.
- [BombsFlatteningSystem](Systems/Bombs/BombsFlatteningSystem.cs) - Система изменения положения (сплющивания) массива бомб при приближении к краям и при выгоде на финишный стейт уровня
- [LaunchSystem](Systems/Bombs/LaunchBombsSystem.cs) - система рассчета количества и запуска ракет в цель\
- [SteeringSystem](Systems/Bombs/BombSteeringSystem.cs) - система управляющая полетом ракеты, подворачивающая её согласно дуге движения, работает через три AnimationCurve, задающих отклонения для каждой оси (+их рандомные модификаторы для рандомизации кривых полета).
- [SetAndSwapBombsSystem](Systems/Bombs/BombsSetAndSwapSystem.cs) - система установки новых и обновления (при апгрейде) уже установленных бомб, срабатывает при сборе бонусов/попадании в ловушки
- [ChangeViewSystem](Systems/Plane/ChangePlaneViewSystem.cs) - система смены View самолета
- [GameStates](Systems/GameStates) - игровые стейты (реализацией которых я занимался)
- [SceneObjectsSystems](Systems/SceneObjects) - системы связанные с работой объектов на сцене (ворота, ловушки, пилоны врагов)
- [Components](Components) - часть реализованных компонентов и конфигов
