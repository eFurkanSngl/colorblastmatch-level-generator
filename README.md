# Level Generator

Unity icin gelistirilmis bu modul, level tasarimini editor tarafinda hizli ve duzenli sekilde yapmanizi saglar.
Bu repoda sadece **authoring (level uretim) tarafi** bulunur.

## Ne Yapar?

- Grid cell uretir
- Inspector uzerinden cell haritasi isaretlemeye izin verir (`Empty / Tile / Obstacle`)
- Isaretlenen alanlara obstacle uretir
- Son duzeni `LevelData + Prefab` olarak kaydeder
- Runtime'da tile spawn edilmesine uygun temel veriyi hazirlar

## Klasor Yapisi

- `Assets/Scripts/LevelAuthoring/LevelAuthoringTool.cs`
- `Assets/Scripts/LevelAuthoring/Editor/LevelAuthoringWindow.cs`
- `Assets/Scripts/Core/GridManager.cs`
- `Assets/Scripts/Pooling/CellPool.cs`
- `Assets/Scripts/Core/CellView.cs`
- `Assets/Scripts/CellData/CellData.cs`
- `Assets/Scripts/RuntimeData/GridruntimeData.cs`
- `Assets/Scripts/LevelData/LevelData.cs`
- `Assets/Scripts/Enum/*`

## Gereksinimler

- Unity 2021+ (veya ustu)
- Odin Inspector

## Kurulum (Scene)

1. Sahneye bir `GenerateGrid` objesi olustur.
2. Ayni objeye su componentleri ekle:
   - `GridManager`
   - `LevelAuthoringTool`
3. Ayrica sahnede bir `PoolManager` objesi olustur ve `CellPool` ekle.
4. `LevelAuthoringTool` icindeki referanslari ata:
   - `Level Data`
   - `Grid Manager`
   - `Cell Pool`
   - `Level Root` (LevelParent)
   - `Cells Root`
   - `Obstacles Root`
   - `Obstacle Prefab`
5. `CellPool` icinde:
   - `Cell Prefab`
   - `Cell Root`
   alanlarini ata.

## Adim Adim Kullanim

1. `LevelAuthoringTool` icinden hedef `LevelData` sec.
2. `Generate Cells` butonuna bas.
3. Grid map uzerinden gereksiz hucreleri `Empty` yap.
4. Obstacle olacak hucreleri `Obstacle` olarak isaretle.
5. `Generate Obstacle` butonuna bas.
6. Sahnedeki duzeni kontrol et.
7. `Save To LevelData + Prefab` butonuna bas.
8. Sonraki level icin yeni `LevelData` secip ayni adimlari tekrarla.

## Runtime Akisi (Onerilen)

- `Cells + Obstacles` prefabdan gelir.
- `Tiles` runtime'da pool ile spawn edilir.
- Bu sayede match/destroy/gravity sistemi daha moduler kalir.

## Demo (GIF/Video)

README'de GIF gostermek icin:

```md
![Level Generator Demo](docs/media/level-generator.gif)
```

Video paylasmak icin (link):

```md
[Demo Video](docs/media/level-generator-demo.mp4)
```

Ornek ekran goruntusu:

<img width="534" height="1115" alt="image" src="https://github.com/user-attachments/assets/c7287a02-3c6c-49eb-a3f4-b64171e4c082" />
